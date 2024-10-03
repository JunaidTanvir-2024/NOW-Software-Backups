using Effortless.Api.Features.Auth.Otp.Request;
using Effortless.Api.Features.Auth.Otp.Response;
using Effortless.Api.Features.User;
using Effortless.Core.Domain.Entities;
using Effortless.Core.Extensions;
using Effortless.Core.Services.Otp;
using RW;

using Microsoft.Extensions.Options;
using Effortless.Core.Services.TimeWrap;

namespace Effortless.Api.Features.Auth.Otp;

public interface IOtpHandler
{
    Task<IResultWrapper<Response.Response>> GenerateOtpAsync(GenerateOtpRequest request);
    Task<IResultWrapper<VerifyOtpResponse>> VerifyOtpAsync(VerifyOtpRequest request);
}

internal sealed class OtpHandler : IOtpHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IOtpRepository _otpRepository;
    private readonly IOtpService _otpService;
    private readonly ITimeWrapService _dateTimeService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly OtpSetting _otpSettings;

    public OtpHandler(
        IUserRepository userRepository,
        IOtpRepository otpRepository,
        IOtpService otpService,
        IOptions<OtpSetting> options,
        ITimeWrapService dateTimeService,
        IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository;
        _otpRepository = otpRepository;
        _otpService = otpService;
        _dateTimeService = dateTimeService;
        _httpContextAccessor = httpContextAccessor;
        _otpSettings = options.Value;
    }

    public async Task<IResultWrapper<Response.Response>> GenerateOtpAsync(GenerateOtpRequest request)
    {
        // Getting user info against phone number
        var userInfo = await _userRepository.GetByPhoneNumberAsync(request.PhoneNumber);
        if (userInfo is null)
        {
            return ResultWrapper.Failure<Response>();
        }

        // Getting otp info against user
        var otpInfo = await _otpRepository.GetOtpByTypeAndUserIdAsync(userInfo.Id, request.Type);

        if (otpInfo is not null)
        {
            // Update otp info against user before creating new otp. It is required to get latest otp status against user
            await UpdateOtpInfo(otpInfo, userInfo.Id);
        }

        var lastesOtpInfo = await _otpRepository.GetOtpByTypeAndUserIdAsync(userInfo.Id, request.Type);


        if (lastesOtpInfo?.IsBlocked == true)
        {
            return ResultWrapper.Failure<Response>();
        }

        if (lastesOtpInfo?.IsRetryLimitExceeded == true)
        {
            return ResultWrapper.Failure<Response>();
        }

        var otpCode = _otpService.GenerateOtp();

        var otpEntity = new OtpEntity
        {
            Code = otpCode,
            ExpiryTime = _dateTimeService.UtcNow.AddMinutes(_otpSettings.ExpiryTimeInMinutes),
            Type = request.Type,
            UserId = userInfo.Id,
        };

        await _otpRepository.AddOtpAsync(new OtpEntity
        {
            Code = otpCode,
            ExpiryTime = _dateTimeService.UtcNow.AddMinutes(_otpSettings.ExpiryTimeInMinutes),
            Type = request.Type,
            UserId = userInfo.Id,
        });

        return ResultWrapper.Success(new Response
        {
            Code = otpEntity.Code,
            Type = otpEntity.Type,
            ExpiryTime = otpEntity.ExpiryTime,
            UserId = userInfo.Id,
            PhoneNumber = userInfo.PhoneNumber,
        });
    }

    public async Task<IResultWrapper<VerifyOtpResponse>> VerifyOtpAsync(VerifyOtpRequest request)
    {
        var phoneNumber = _httpContextAccessor.HttpContext?.User.GetPhoneNumber();

        var otpInfo = await _otpRepository.GetOtpByCodeAndTypeAsync(request.Code, request.Type, phoneNumber);

        var otpUnblockTime = GetOtpUnblockTime(otpInfo?.BlockTime);

        if (otpInfo is null)
        {
            return ResultWrapper.Failure<VerifyOtpResponse>();
        }

        if (otpInfo.Code != request.Code
            || otpUnblockTime.UnblockTime > _dateTimeService.UtcNow
            || otpInfo.IsAlreadyUsed
            || otpInfo.UsageCount > _otpSettings.CreationLimit)
        {
            otpInfo.UsageCount++;

            await _otpRepository.UpdateOtpAsync(otpInfo);

            return ResultWrapper.Failure<VerifyOtpResponse>();
        }
        // As Otp verified, Making it IsAlreadyUsed true to update in database. 

        otpInfo.IsAlreadyUsed = true;

        await _otpRepository.UpdateOtpAsync(otpInfo);

        return ResultWrapper.Success<VerifyOtpResponse>();
    }

    private (DateTime? UnblockTime, bool IsUnblocked) GetOtpUnblockTime(DateTime? blockTime)
    {
        var remainingBlockTime = (blockTime?.AddMinutes(_otpSettings.BlockTimeInMinutes) - _dateTimeService.UtcNow);

        var retryAfterTime = remainingBlockTime?.TotalMinutes ?? 0;

        return (_dateTimeService.UtcNow.AddMinutes(retryAfterTime), retryAfterTime <= 0);
    }

    private async Task UpdateOtpInfo(OtpEntity? otpInfo, string? userId)
    {
        var unblockTime = GetOtpUnblockTime(otpInfo?.BlockTime);

        var otpCounts = await _otpRepository.GetOtpCountByTypeAndUserIdAsync(userId, otpInfo?.Type);

        if (otpInfo is not null)
        {
            if (otpCounts >= _otpSettings.CreationLimit)
            {
                if (unblockTime.UnblockTime <= _dateTimeService.UtcNow && otpInfo.IsBlocked == true)
                {
                    otpInfo.IsBlocked = false;
                    otpInfo.IsRetryLimitExceeded = false;
                    otpInfo.BlockTime = null;
                    await _otpRepository.DeleteOtpListAsync(otpInfo);
                }

                otpInfo.IsRetryLimitExceeded = true;
                otpInfo.IsBlocked = true;
                otpInfo.BlockTime = _dateTimeService.UtcNow.AddMinutes(_otpSettings.BlockTimeInMinutes);
            }
            await _otpRepository.UpdateOtpAsync(otpInfo);
        }
    }
}

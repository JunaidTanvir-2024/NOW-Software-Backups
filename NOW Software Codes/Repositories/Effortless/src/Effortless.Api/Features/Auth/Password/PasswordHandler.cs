using Effortless.Api.Features.Auth.Password.Request;
using Effortless.Api.Features.Auth.Password.Response;
using Effortless.Api.Features.User;
using Effortless.Core.Services.Identity;

using RW;

namespace Effortless.Api.Features.Auth.Password;

internal interface IPasswordHandler
{
    Task<IResultWrapper<SetNewPasswordResponse>> SetNewPasswordAsync(SetNewPasswordRequest request);
}

internal sealed class PasswordHandler : IPasswordHandler
{
    private readonly IAuthService _authIdentityService;
    private readonly IUserRepository _userRepository;

    public PasswordHandler(
        IAuthService authIdentityService,
        IUserRepository userRepository)
    {
        _authIdentityService = authIdentityService;
        _userRepository = userRepository;
    }

    public async Task<IResultWrapper<SetNewPasswordResponse>> SetNewPasswordAsync(SetNewPasswordRequest request)
    {
        var user = await _userRepository.GetByPhoneNumberAsync(request.PhoneNumber);

        if (user == null)
        {
            return ResultWrapper.Failure<SetNewPasswordResponse>();
        }

        var userWithResetPassword = await _authIdentityService.PasswordResetAsync(user, request.Password);

        if (userWithResetPassword?.Succeeded != true)
        {
            return ResultWrapper.Failure<SetNewPasswordResponse>();

        }

        return ResultWrapper.Success(new SetNewPasswordResponse() { IsPasswordReset = userWithResetPassword.Succeeded });
    }
}

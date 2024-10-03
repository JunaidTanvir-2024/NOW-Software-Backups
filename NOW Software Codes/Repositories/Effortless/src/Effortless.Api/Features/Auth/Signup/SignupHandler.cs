using Effortless.Api.Features.Auth.Signup.Request;
using Effortless.Api.Features.Auth.Signup.Response;
using Effortless.Core.Domain.Entities;
using Effortless.Core.Services.Identity;

using Microsoft.Extensions.Localization;

using RW;

using static Effortless.Core.Domain.Defination.AppConstant.Status;

namespace Effortless.Api.Features.Auth.Signup;

public interface ISignupHandler
{
    Task<IResultWrapper<SignupResponse>> SignupAsync(SignupRequest request);
}
internal sealed class SignupHandler : ISignupHandler
{
    private readonly IAuthService _authService;
    private readonly IStringLocalizer<SignupHandler> _localizer;

    public SignupHandler(
        IAuthService authService,
        IStringLocalizer<SignupHandler> localizer)
    {
        _authService = authService;
        _localizer = localizer;
    }
    public async Task<IResultWrapper<SignupResponse>> SignupAsync(SignupRequest request)
    {
        // Register User
        var appUser = new UserEntity()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.UserEmail,
            UserName = request.UserName,
            PhoneNumber = request.PhoneNumber,
        };

        var registerResult = await _authService.RegisterAsync(appUser, request.Password);

        if (registerResult?.Succeeded == true)
        {
            return ResultWrapper.Success(new SignupResponse(request.PhoneNumber));
        }
        return ResultWrapper.Failure<SignupResponse>(_localizer["ChutMarwa"], Code.UserNotCreated);
    }
}

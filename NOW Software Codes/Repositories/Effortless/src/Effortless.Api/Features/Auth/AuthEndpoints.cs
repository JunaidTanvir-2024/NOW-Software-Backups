using Effortless.Api.Features.Auth.Otp;
using Effortless.Api.Features.Auth.Otp.Request;
using Effortless.Api.Features.Auth.Password;
using Effortless.Api.Features.Auth.Password.Request;
using Effortless.Api.Features.Auth.Signup;
using Effortless.Api.Features.Auth.Signup.Request;
using Effortless.Api.Features.Auth.Token;
using Effortless.Api.Features.Auth.Token.Request;
using Effortless.Core.Domain.Defination;
using Effortless.Core.Filters;

using Microsoft.AspNetCore.Mvc;

namespace Effortless.Api.Features.Auth;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app, AuthFeatureSetting authFeatureSetting)
    {
        if (authFeatureSetting.Module.IsEnable)
        {
            var authApi = app
                .MapGroup(authFeatureSetting.Module.Prefix)
                .WithTags(authFeatureSetting.Module.Name)
                .RequireAuthorization();

            if (authFeatureSetting.Endpoints.Login.IsEnable)
            {
                authApi.MapPost(authFeatureSetting.Endpoints.Login.Name, Login)
                    .AddEndpointFilter<FluentValidationFilter<LoginRequest>>()
                    .Produces(StatusCodes.Status400BadRequest)
                    .Produces(StatusCodes.Status200OK)
                    .Produces(StatusCodes.Status401Unauthorized)
                    .AllowAnonymous();
            }
            if (authFeatureSetting.Endpoints.Signup.IsEnable)
            {
                authApi.MapPost(authFeatureSetting.Endpoints.Signup.Name, Signup)
                .AddEndpointFilter<FluentValidationFilter<SignupRequest>>()
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status403Forbidden)
                .AllowAnonymous();
            }
            if (authFeatureSetting.Endpoints.ForgotPassword.IsEnable)
            {
                authApi.MapPost(authFeatureSetting.Endpoints.ForgotPassword.Name, ForgotPassword)
               .AddEndpointFilter<FluentValidationFilter<SetNewPasswordRequest>>()
               .Produces(StatusCodes.Status400BadRequest)
               .Produces(StatusCodes.Status403Forbidden)
               .Produces(StatusCodes.Status200OK);
            }
            if (authFeatureSetting.Endpoints.Otp.IsEnable)
            {
                authApi.MapPost(authFeatureSetting.Endpoints.Otp.Name, GenerateOtp)
               .AddEndpointFilter<FluentValidationFilter<GenerateOtpRequest>>()
               .Produces(StatusCodes.Status400BadRequest)
               .Produces(StatusCodes.Status403Forbidden)
               .Produces(StatusCodes.Status200OK);
            }
            if (authFeatureSetting.Endpoints.OtpVerify.IsEnable)
            {
                authApi.MapPost(authFeatureSetting.Endpoints.OtpVerify.Name, VerifyOtp)
              .AddEndpointFilter<FluentValidationFilter<VerifyOtpRequest>>()
              .Produces(StatusCodes.Status400BadRequest)
              .Produces(StatusCodes.Status200OK);
            }
            if (authFeatureSetting.Endpoints.RefreshToken.IsEnable)
            {
                authApi.MapPost(authFeatureSetting.Endpoints.RefreshToken.Name, RefreshToken)
              .AddEndpointFilter<FluentValidationFilter<RefreshTokenRequest>>()
              .Produces(StatusCodes.Status400BadRequest)
              .Produces(StatusCodes.Status200OK)
              .AllowAnonymous();
            }
        }
    }
    private static async Task<IResult> Login(ILoginHandler loginHandler, [FromForm] LoginRequest request)
    {
        var result = await loginHandler.LoginAsync(request);
        return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
    }
    private static async Task<IResult> Signup(ISignupHandler signupHandler, [FromBody] SignupRequest request)
    {
        var result = await signupHandler.SignupAsync(request);
        return result.IsSuccess ? Results.Created(AppConstant.Domain.Name, result.Payload?.PhoneNumber) : Results.BadRequest(result);
    }
    private static async Task<IResult> ForgotPassword(IPasswordHandler passwordHandler, [FromBody] SetNewPasswordRequest request)
    {
        var result = await passwordHandler.SetNewPasswordAsync(request);
        return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
    }
    private static async Task<IResult> GenerateOtp(IOtpHandler otpHandler, [FromBody] GenerateOtpRequest request)
    {
        var result = await otpHandler.GenerateOtpAsync(request);
        return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
    }
    private static async Task<IResult> VerifyOtp(IOtpHandler otpHandler, [FromBody] VerifyOtpRequest request)
    {
        var result = await otpHandler.VerifyOtpAsync(request);
        return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
    }
    private static async Task<IResult> RefreshToken(ITokenHandler tokenHandler, [FromBody] RefreshTokenRequest request)
    {
        var result = await tokenHandler.RenewTokensAsync(request);
        return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
    }
}

using Effortless.Api.Features.Auth.Otp.Request;
using Effortless.Api.Features.Auth.Otp;
using Effortless.Api.Features.Auth.Password.Request;
using Effortless.Api.Features.Auth.Password;
using Effortless.Api.Features.Auth.Signup.Request;
using Effortless.Api.Features.Auth.Signup;
using Effortless.Core.Filters;
using Microsoft.AspNetCore.Mvc;
using Effortless.Api.Features.User.Requests;

namespace Effortless.Api.Features.User;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app, UserFeatureSetting userFeatureSetting)
    {
        if (userFeatureSetting.Module.IsEnable)
        {
            var authApi = app
                .MapGroup(userFeatureSetting.Module.Prefix)
                .WithTags(userFeatureSetting.Module.Name);

            if (userFeatureSetting.Endpoints.Users.IsEnable)
            {
                authApi.MapGet("/users", GetUsers)
                    .Produces(StatusCodes.Status404NotFound)
                    .Produces(StatusCodes.Status200OK)
                    .AllowAnonymous();
            }
            if (userFeatureSetting.Endpoints.UserByPhoneNumber.IsEnable)
            {
                authApi.MapGet("/users/{PhoneNumber}", GetByPhoneNumber)
                .AddEndpointFilter<FluentValidationFilter<GetByPhoneNumberRequest>>()
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status200OK);
            }
            if (userFeatureSetting.Endpoints.Add.IsEnable)
            {
                authApi.MapPost("/users", AddUser)
               .AddEndpointFilter<FluentValidationFilter<SetNewPasswordRequest>>()
               .Produces(StatusCodes.Status400BadRequest)
               .Produces(StatusCodes.Status200OK);
            }
            if (userFeatureSetting.Endpoints.Update.IsEnable)
            {
                authApi.MapPut("/users", UpdateUser)
               .AddEndpointFilter<FluentValidationFilter<GenerateOtpRequest>>()
               .Produces(StatusCodes.Status400BadRequest)
               .Produces(StatusCodes.Status200OK);
            }
            if (userFeatureSetting.Endpoints.Remove.IsEnable)
            {
                authApi.MapDelete("/users/{PhoneNumber}", RemoveUser)
              .AddEndpointFilter<FluentValidationFilter<VerifyOtpRequest>>()
              .Produces(StatusCodes.Status400BadRequest)
              .Produces(StatusCodes.Status200OK);
            }
        }
    }
    private static async Task<IResult> GetUsers(ILoginHandler loginHandler, [FromBody] LoginRequest request)
    {
        var result = await loginHandler.LoginAsync(request);
        return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
    }
    private static async Task<IResult> GetByPhoneNumber(ISignupHandler signupHandler, [FromBody] SignupRequest request)
    {
        var result = await signupHandler.SignupAsync(request);
        return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
    }
    private static async Task<IResult> AddUser(IPasswordHandler passwordHandler, [FromBody] SetNewPasswordRequest request)
    {
        var result = await passwordHandler.SetNewPasswordAsync(request);
        return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
    }
    private static async Task<IResult> UpdateUser(IOtpHandler otpHandler, [FromBody] GenerateOtpRequest request)
    {
        var result = await otpHandler.GenerateOtpAsync(request);
        return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
    }
    private static async Task<IResult> RemoveUser(IOtpHandler otpHandler, [FromBody] VerifyOtpRequest request)
    {
        var result = await otpHandler.VerifyOtpAsync(request);
        return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
    }
}

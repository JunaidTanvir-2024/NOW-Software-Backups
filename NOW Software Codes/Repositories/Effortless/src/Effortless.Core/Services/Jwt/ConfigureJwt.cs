using System.Security.Claims;
using System.Text.Json;

using Effortless.Core.Domain.Definitions;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using RW;

namespace Effortless.Core.Services.Jwt;

internal static class JwtConfiguration
{
    internal static IServiceCollection AddJwtConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        // Get IJwtTokenService
        var jwtService = services.BuildServiceProvider().GetRequiredService<IJwtService>();

        // Bind jwt settings
        var jwtSettings = services.BuildServiceProvider().GetRequiredService<IOptions<JwtSetting>>().Value;

        string publicKeyPath = string.Empty;

        // Get Secret Key Path;
        if (jwtSettings.AsymmetricFiles.SecretKeyFile is not null)
        {
            publicKeyPath = Path.Combine(Directory.GetCurrentDirectory(), jwtSettings.AsymmetricFiles.SecretKeyFile);
        }

        // Authentication scheme
        services.AddAuthentication(authentication =>
        {
            authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        // Jwt Bearer with events
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, bearer =>
        {
            bearer.IncludeErrorDetails = true; // => true for easy debugging else remove it
            bearer.TokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new ECDsaSecurityKey(jwtService.GetSecurityKeyViaFile(publicKeyPath)),
                ValidAudience = jwtSettings.Audience,
                ValidIssuer = jwtSettings.Issuer,
                RequireSignedTokens = true,
                RequireExpirationTime = true,
                ValidateLifetime = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                RoleClaimType = ClaimTypes.Role,
                ClockSkew = TimeSpan.Zero // Add zero tolerence on token expiration date/time
            };
            bearer.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context => HandleAuthenticationFailed(context),
                OnChallenge = context => HandleChallenge(context),
                OnForbidden = context => HandleForbidden(context)
            };
        });
        services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .Build();
        });
        return services;
    }
    private static Task HandleAuthenticationFailed(AuthenticationFailedContext context)
    {
        if (context.Exception is SecurityTokenExpiredException)
        {
            return WriteJsonResponseAsync(context.Response, AppConstant.Status.Code.Unauthorized, AppConstant.Status.Key.JwtTokenExpired);
        }
        if (context.Exception is SecurityTokenValidationException)
        {
            return WriteJsonResponseAsync(context.Response, AppConstant.Status.Code.Unauthorized, AppConstant.Status.Key.JwtTokenInvalid);
        }
        return Task.CompletedTask;
    }

    private static Task HandleChallenge(JwtBearerChallengeContext context)
    {
        context.HandleResponse();

        if (!context.Request.Headers.ContainsKey("Authorization"))
        {
            return WriteJsonResponseAsync(context.Response, AppConstant.Status.Code.Unauthorized, AppConstant.Status.Key.JwtTokenMissing);
        }
        if (!context.Response.HasStarted)
        {
            return WriteJsonResponseAsync(context.Response, AppConstant.Status.Code.Unauthorized, AppConstant.Status.Key.JwtTokenInvalid);
        }

        return Task.CompletedTask;
    }

    private static Task HandleForbidden(ForbiddenContext context)
    {
        return WriteJsonResponseAsync(context.Response, AppConstant.Status.Code.Forbidden, AppConstant.Status.Key.Forbidden);
    }

    private static Task WriteJsonResponseAsync(HttpResponse response, int statusCode, string errorMessage)
    {
        response.ContentType = AppConstant.ContentType.ApplicationJson;
        response.StatusCode = statusCode;

        var jsonResponse = JsonSerializer.Serialize(ResultWrapper.Failure(errorMessage, statusCode));

        return response.WriteAsync(jsonResponse);
    }
}

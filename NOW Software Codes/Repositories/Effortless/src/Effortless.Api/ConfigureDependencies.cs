using System.Globalization;

using Effortless.Api.Common.Settings;
using Effortless.Api.Features.Auth.Otp;
using Effortless.Api.Features.Auth.Otp.Request;
using Effortless.Api.Features.Auth.Password;
using Effortless.Api.Features.Auth.Password.Request;
using Effortless.Api.Features.Auth.Signup;
using Effortless.Api.Features.Auth.Signup.Request;
using Effortless.Api.Features.Auth.Token;
using Effortless.Api.Features.Auth.Token.Request;
using Effortless.Api.Features.User;
using Effortless.Core;

using FluentValidation;

using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Localization;

namespace Effortless.Api;

public static class ConfigureDependencies
{
    public static IServiceCollection AddApiDependencies(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddHttpContextAccessor();
        services.AddCookiePolicy(options =>
        {
            options.HttpOnly = HttpOnlyPolicy.Always;
            options.ConsentCookie = new CookieBuilder()
            {
                Expiration = TimeSpan.FromDays(365),
                HttpOnly = true,
                IsEssential = true,
                SecurePolicy = CookieSecurePolicy.Always,
            };
        });
        services.AddLocalization(options =>
        {
            options.ResourcesPath = "Localizations";
        });

        services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new List<CultureInfo>
        {
                new CultureInfo("en-US"),
                new CultureInfo("fr-FR"),
        };

            options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
        });
        services.Configure<FeaturesSetting>(configuration.GetSection(FeaturesSetting.SectionName));
        services.AddCoreDependencies(configuration);
        services.AddEndpointsApiExplorer();
        services.AddScoped<ILoginHandler, Login>();
        services.AddScoped<IOtpHandler, OtpHandler>();
        services.AddScoped<IPasswordHandler, PasswordHandler>();
        services.AddScoped<ISignupHandler, SignupHandler>();
        services.AddScoped<ITokenHandler, TokenHandler>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IOtpRepository, OtpRepository>();
        services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();
        services.AddScoped<IValidator<SignupRequest>, SignupRequestValidator>();
        services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();
        services.AddScoped<IValidator<SetNewPasswordRequest>, SetNewPasswordRequestValidator>();
        services.AddScoped<IValidator<RefreshTokenRequest>, RefreshTokenRequestValidator>();
        services.AddScoped<IValidator<GenerateOtpRequest>, GenerateOtpRequestValidator>();
        services.AddScoped<IValidator<VerifyOtpRequest>, VerifyOtpRequestValidator>();


        services.Configure<FeaturesSetting>(configuration.GetSection(FeaturesSetting.SectionName));
        return services;
    }
}

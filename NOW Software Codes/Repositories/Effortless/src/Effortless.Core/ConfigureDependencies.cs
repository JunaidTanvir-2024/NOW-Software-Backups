using System.Diagnostics.CodeAnalysis;

using Effortless.Core.Domain.Definitions;
using Effortless.Core.Domain.Entities;
using Effortless.Core.Persistence;
using Effortless.Core.Services.Identity;
using Effortless.Core.Services.Jwt;
using Effortless.Core.Services.OpenApi;
using Effortless.Core.Services.Otp;
using Effortless.Core.Services.TimeWrap;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Effortless.Core;

public static class ConfigureDependencies
{
    public static IServiceCollection AddCoreDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AppSettingConfigurations(configuration);
        services.AddHttpClient();
        services.AddHttpContextAccessor();
        services.CustomServices();
        services.AddOpenApiConfiguration();
        services.AddJwtConfiguration(configuration);

        services
            .AddDbContextPool<EffortlessDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString(AppConstant.Database.Effortless));
            });

        services.AddIdentity<UserEntity, IdentityRole>().AddEntityFrameworkStores<EffortlessDbContext>().AddDefaultTokenProviders();

        // Authentication Configurations Options
        services.Configure<IdentityOptions>(options =>
        {
            // Password settings.
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 3;
            options.Password.RequiredUniqueChars = 0;

            // Lockout settings.
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings.
            //options.User.AllowedUserNameCharacters = AppConstants.AllowedUserNameCharacters;
            options.User.RequireUniqueEmail = true;
        });
        return services;
    }

    private static IServiceCollection CustomServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddSingleton<ITimeWrapService, TimeWrapService>();
        services.AddSingleton<IJwtService, JwtService>();
        services.AddScoped<IOtpService, OtpService>();

        return services;
    }

    private static IServiceCollection AppSettingConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSetting>(configuration.GetSection(JwtSetting.SectionName));
        services.Configure<OtpSetting>(configuration.GetSection(OtpSetting.SectionName));
        services.Configure<OpenApiSetting>(configuration.GetSection(OpenApiSetting.SectionName));
        return services;
    }

    public static IApplicationBuilder UseCoreDependencies(this IApplicationBuilder builder, IWebHostEnvironment environment)
    {
        EffortlessDbMigrator.Migrate(builder.ApplicationServices);

        if (environment.IsDevelopment())
        {
            builder.UseSwagger();
            builder.UseSwaggerUI();
        }
        return builder;
    }
}

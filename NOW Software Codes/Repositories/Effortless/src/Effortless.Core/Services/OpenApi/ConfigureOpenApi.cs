using Effortless.Core.Services.OpenApi.Filters;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace Effortless.Core.Services.OpenApi;

internal static class ConfigureOpenApi
{
    internal static IServiceCollection AddOpenApiConfiguration(this IServiceCollection services)
    {
        // Get Open Api setting
        var openApiSettings = services.BuildServiceProvider().GetRequiredService<IOptions<OpenApiSetting>>().Value;

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(openApiSettings.Version, new OpenApiInfo
            {
                Version = openApiSettings.Version,

                Title = openApiSettings.Title,

                Description = openApiSettings.Description,

                TermsOfService = new Uri(openApiSettings.TermsOfService),

                Contact = new OpenApiContact
                {
                    Name = openApiSettings.ContactName,
                    Url = new Uri(openApiSettings.ContactUrl)
                },

                License = new OpenApiLicense
                {
                    Name = openApiSettings.LicenseName,
                    Url = new Uri(openApiSettings.LicenseUrl)
                }
            });
            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                Name = openApiSettings.JwtSecurityDefinitionName,
                Description = openApiSettings.JwtSecurityDefinitionDescription,
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                BearerFormat = openApiSettings.JwtSecurityDefinitionBearerFormat,
            });

            options.OperationFilter<GlobalAuthFilter>();
        });
        return services;
    }
}

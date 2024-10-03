using Effortless.Api.Common.Settings;
using Effortless.Api.Features.Auth;
using Effortless.Api.Features.User;
using Effortless.Core;

using Microsoft.Extensions.Options;

namespace Effortless.Api;

public static class ConfigureMiddlewares
{
    public static WebApplication AddApiMiddlewares(this WebApplication app, IWebHostEnvironment environment)
    {
        app.UseCoreDependencies(environment);
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapApiEndpoints();
        return app;
    }

    private static WebApplication MapApiEndpoints(this WebApplication app)
    {
        var authFeatureSetting = app.Services.GetRequiredService<IOptions<FeaturesSetting>>().Value;
        if (authFeatureSetting.Auth.Module.IsEnable)
        {
            app.MapAuthEndpoints(authFeatureSetting.Auth);
        }
        if (authFeatureSetting.User.Module.IsEnable)
        {
            app.MapUserEndpoints(authFeatureSetting.User);
        }
        return app;
    }
}

using Effortless.Api;
using Effortless.Core.Domain.Defination;
using Effortless.Core.Services.Logger;

using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.Server;

using Serilog;

LoggerService.EnsureInitialized();
Log.Information($"{AppConstant.Assembly.Name} Booting Up..");
try
{
    var builder = WebApplication.CreateBuilder(args);
    {
        builder.Host.UseSerilog((hostingContext, config) => config.ReadFrom.Configuration(hostingContext.Configuration));

        // Add services to the container.
        builder.Services.AddApiDependencies(builder.Configuration);
    }

    var app = builder.Build();
    {
        app.AddApiMiddlewares(app.Environment);
        app.Run();
    }
}
catch (Exception ex) when (!ex.GetType().Name.Equals("StopTheHostException", StringComparison.Ordinal))
{
    LoggerService.EnsureInitialized();
    Log.Error($"{AppConstant.Assembly.Name} Shutting down...");
    Log.Fatal(ex, "Unhandled exception");
    throw;
}
finally
{
    LoggerService.EnsureInitialized();
    Log.Error($"{AppConstant.Assembly.Name} Shutting down...");
    Log.CloseAndFlush();
}

// Get Application Running
static object? GetPortInfo(WebApplication app)
{
    var server = app.Services.GetRequiredService<IServer>();
    var addresses = server.Features.Get<IServerAddressesFeature>()?.Addresses;
    var address = addresses?.FirstOrDefault();
    return address?.Split(':').LastOrDefault();
}

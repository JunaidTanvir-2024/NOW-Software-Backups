using Microsoft.Extensions.DependencyInjection;

namespace Effortless.Core.DependencyResolver;

internal static class ConfigureServicesExtension
{
    public static void AutoResolve(this IServiceCollection services)
    {
        // Configure Services
        ServicesScanner(services, typeof(ServiceType.ISingleton), ServiceLifetime.Singleton);
        ServicesScanner(services, typeof(ServiceType.IScoped), ServiceLifetime.Scoped);
        ServicesScanner(services, typeof(ServiceType.ITransient), ServiceLifetime.Transient);
    }
    internal static IServiceCollection ServicesScanner(IServiceCollection services, Type interfaceType, ServiceLifetime servicelifetime)
    {
        var interfaceTypes =
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(t => interfaceType.IsAssignableFrom(t)
                            && t.IsClass && !t.IsAbstract)
                .Select(t => new
                {
                    Service = t.GetInterfaces().FirstOrDefault(),
                    Implementation = t
                })
                .Where(t => t.Service is not null
                            && interfaceType.IsAssignableFrom(t.Service));

        foreach (var type in interfaceTypes)
        {
            RegisterService(services, type.Service!, type.Implementation, servicelifetime);
        }

        return services;
    }

    internal static IServiceCollection RegisterService(IServiceCollection services, Type serviceType, Type implementationType, ServiceLifetime serviceLifetime)
    {
        return serviceLifetime switch
        {
            ServiceLifetime.Transient => services.AddTransient(serviceType, implementationType),
            ServiceLifetime.Scoped => services.AddScoped(serviceType, implementationType),
            ServiceLifetime.Singleton => services.AddSingleton(serviceType, implementationType),
            _ => throw new ArgumentException("Invalid lifeTime", nameof(serviceLifetime))
        };
    }
}

using CQRSAndMediatrWithFluentValidationSampleApplication.Product.Pipeline;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CQRSAndMediatrWithFluentValidationSampleApplication.Product
{
    public static class ProductModule
    {
        public static IServiceCollection AddProductModule(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddMediatR(typeof(ProductModule).Assembly);
            serviceCollection.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            serviceCollection.AddTransient(serviceType: typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));

            return serviceCollection;
        }
    }
}

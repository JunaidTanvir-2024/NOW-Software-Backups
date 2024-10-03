using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Effortless.Core.Persistence;
public static class EffortlessDbMigrator
{
    public static void Migrate(IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetService<EffortlessDbContext>();

            context?.Database.Migrate();
        }
    }
}

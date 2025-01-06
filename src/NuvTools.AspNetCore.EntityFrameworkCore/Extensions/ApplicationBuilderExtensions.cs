using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace NuvTools.AspNetCore.EntityFrameworkCore.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder DatabaseMigrate<TContext>(this IApplicationBuilder app, TimeSpan? timeout = null) where TContext : DbContext
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var context = serviceScope.ServiceProvider.GetService<TContext>();
        if (context != null && context.Database != null)
        {
            var originalTimeout = context.Database.GetCommandTimeout();

            try
            {
                if (timeout.HasValue)
                    context.Database.SetCommandTimeout(timeout.Value);

                context.Database.Migrate();
            }
            finally
            {
                if (timeout.HasValue)
                    context.Database.SetCommandTimeout(originalTimeout);
            }
        }

        return app;
    }
}
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace NuvTools.AspNetCore.EntityFrameworkCore.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder DatabaseMigrate<TContext>(this IApplicationBuilder app) where TContext : DbContext
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var context = serviceScope.ServiceProvider.GetService<TContext>();
        if (context != null && context.Database != null)
        {
            context.Database.Migrate();
        }

        return app;
    }
}
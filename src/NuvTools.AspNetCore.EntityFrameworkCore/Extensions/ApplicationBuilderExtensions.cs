using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace NuvTools.AspNetCore.EntityFrameworkCore.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IApplicationBuilder"/> to support Entity Framework Core operations.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Applies any pending Entity Framework Core migrations for the specified database context
    /// during application startup.
    /// </summary>
    /// <typeparam name="TContext">The type of the <see cref="DbContext"/> to migrate.</typeparam>
    /// <param name="app">The <see cref="IApplicationBuilder"/> instance.</param>
    /// <param name="timeout">
    /// An optional timeout for the migration command. If specified, the command timeout is temporarily
    /// set to this value during migration and then restored to its original value.
    /// </param>
    /// <returns>The <see cref="IApplicationBuilder"/> so that additional calls can be chained.</returns>
    /// <remarks>
    /// <para>
    /// This method creates a temporary service scope to resolve the database context and applies
    /// any pending migrations by calling the Database.Migrate() method.
    /// </para>
    /// <para>
    /// If a timeout is specified, it is applied only during the migration operation and the original
    /// timeout is restored afterward.
    /// </para>
    /// <para>
    /// If the context or database is not available, the method returns without throwing an exception.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Apply migrations with default timeout
    /// app.DatabaseMigrate&lt;MyDbContext&gt;();
    ///
    /// // Apply migrations with 5-minute timeout
    /// app.DatabaseMigrate&lt;MyDbContext&gt;(TimeSpan.FromMinutes(5));
    /// </code>
    /// </example>
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
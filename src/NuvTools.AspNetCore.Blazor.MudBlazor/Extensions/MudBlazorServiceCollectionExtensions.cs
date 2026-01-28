using Microsoft.Extensions.DependencyInjection;
using NuvTools.AspNetCore.Blazor.MudBlazor.Services;

namespace NuvTools.AspNetCore.Blazor.MudBlazor.Extensions;

/// <summary>
/// Extension methods for registering MudBlazor-related services in the dependency injection container.
/// </summary>
public static class MudBlazorServiceCollectionExtensions
{
    /// <summary>
    /// Adds the <see cref="ILoadingService"/> to the service collection as a scoped service.
    /// </summary>
    /// <param name="services">The service collection to add the service to.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <remarks>
    /// The loading service is registered as scoped to maintain state per Blazor circuit/connection.
    /// </remarks>
    public static IServiceCollection AddLoadingService(this IServiceCollection services)
    {
        return services.AddScoped<ILoadingService, LoadingService>();
    }

    /// <summary>
    /// Adds all MudBlazor-related services to the service collection.
    /// Currently includes <see cref="ILoadingService"/>.
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <remarks>
    /// <para>This method registers the following services:</para>
    /// <list type="bullet">
    /// <item><description><see cref="ILoadingService"/> as scoped</description></item>
    /// </list>
    /// <para>
    /// Note: This method does not call <c>AddMudServices()</c> from MudBlazor.
    /// You should call that separately in your application startup.
    /// </para>
    /// </remarks>
    public static IServiceCollection AddMudBlazorServices(this IServiceCollection services)
    {
        return services.AddLoadingService();
    }
}

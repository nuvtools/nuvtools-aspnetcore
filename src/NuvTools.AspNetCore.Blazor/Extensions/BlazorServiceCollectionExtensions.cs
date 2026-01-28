using Microsoft.Extensions.DependencyInjection;
using NuvTools.AspNetCore.Blazor.JSInterop;
using NuvTools.AspNetCore.Blazor.Services;

namespace NuvTools.AspNetCore.Blazor.Extensions;

/// <summary>
/// Extension methods for registering Blazor services in the dependency injection container.
/// </summary>
public static class BlazorServiceCollectionExtensions
{
    /// <summary>
    /// Adds the <see cref="IClipboardService"/> to the service collection as a scoped service.
    /// </summary>
    /// <param name="services">The service collection to add the service to.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddClipboardService(this IServiceCollection services)
    {
        return services.AddScoped<IClipboardService, ClipboardService>();
    }

    /// <summary>
    /// Adds the <see cref="ILocalStorageService"/> to the service collection as a scoped service.
    /// </summary>
    /// <param name="services">The service collection to add the service to.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddLocalStorageService(this IServiceCollection services)
    {
        return services.AddScoped<ILocalStorageService, LocalStorageService>();
    }

    /// <summary>
    /// Adds the <see cref="ISessionStorageService"/> to the service collection as a scoped service.
    /// </summary>
    /// <param name="services">The service collection to add the service to.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSessionStorageService(this IServiceCollection services)
    {
        return services.AddScoped<ISessionStorageService, SessionStorageService>();
    }

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
    /// Adds all Blazor services to the service collection as scoped services.
    /// This includes <see cref="IClipboardService"/>, <see cref="ILocalStorageService"/>,
    /// <see cref="ISessionStorageService"/>, and <see cref="ILoadingService"/>.
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddBlazorServices(this IServiceCollection services)
    {
        return services
            .AddClipboardService()
            .AddLocalStorageService()
            .AddSessionStorageService()
            .AddLoadingService();
    }
}

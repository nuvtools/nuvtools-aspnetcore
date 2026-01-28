using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace NuvTools.AspNetCore.Localization;

/// <summary>
/// Provides extension methods for configuring composite localization services.
/// </summary>
public static class LocalizationServiceCollectionExtensions
{
    /// <summary>
    /// Adds the composite localizer to the service collection, enabling multiple localization sources
    /// with prefix-based routing and fallback resolution.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="namedResourceTypes">
    /// A dictionary of named resource types where the key is the prefix name (e.g., "Errors")
    /// and the value is the resource type. Named localizers can be targeted using prefix syntax
    /// (e.g., "Errors:ValidationFailed").
    /// </param>
    /// <param name="unnamedResourceTypes">
    /// A collection of resource types to be used as fallback localizers. These are searched
    /// when a key is not found in named localizers.
    /// </param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <remarks>
    /// <para>
    /// This method registers all necessary <see cref="IStringLocalizer{T}"/> instances and
    /// a scoped <see cref="CompositeLocalizer"/> that combines them.
    /// </para>
    /// <para>
    /// Named resource types take precedence when using prefix syntax. Unnamed resource types
    /// are used for fallback resolution when a key is not found in any named localizer.
    /// </para>
    /// <para>
    /// Duplicate types between named and unnamed collections are automatically handled,
    /// with named registrations taking precedence.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// services.AddCompositeLocalizer(
    ///     namedResourceTypes: new Dictionary&lt;string, Type&gt;
    ///     {
    ///         ["Errors"] = typeof(ErrorResources),
    ///         ["Messages"] = typeof(MessageResources)
    ///     },
    ///     unnamedResourceTypes: new[] { typeof(SharedResources) }
    /// );
    /// </code>
    /// </example>
    public static IServiceCollection AddCompositeLocalizer(this IServiceCollection services,
                                                        IDictionary<string, Type>? namedResourceTypes = null,
                                                        IEnumerable<Type>? unnamedResourceTypes = null)
    {
        namedResourceTypes ??= new Dictionary<string, Type>();
        unnamedResourceTypes ??= [];

        var namedTypesSet = new HashSet<Type>(namedResourceTypes.Values);
        var distinctUnnamedTypes = unnamedResourceTypes
                                                    .Where(type => !namedTypesSet.Contains(type))
                                                    .Distinct();

        var allTypes = namedTypesSet.Concat(distinctUnnamedTypes);

        foreach (var type in allTypes)
        {
            var localizerType = typeof(IStringLocalizer<>).MakeGenericType(type);

            if (services.Any(sd => sd.ServiceType == localizerType)) continue;

            services.AddTransient(localizerType, provider =>
            {
                var factory = provider.GetRequiredService<IStringLocalizerFactory>();
                return factory.Create(type);
            });
        }

        services.AddScoped(sp =>
        {
            var dict = new Dictionary<string, IStringLocalizer>();
            var addedTypes = new HashSet<Type>();

            // Add named localizers
            foreach (var (key, type) in namedResourceTypes)
            {
                if (dict.ContainsKey(key)) continue;

                var localizer = (IStringLocalizer)sp.GetRequiredService(
                    typeof(IStringLocalizer<>).MakeGenericType(type));
                dict[key] = localizer;
                addedTypes.Add(type);
            }

            // Add unnamed localizers for fallback only
            int fallbackIndex = 0;
            foreach (var type in distinctUnnamedTypes)
            {
                if (addedTypes.Contains(type)) continue;

                var localizer = (IStringLocalizer)sp.GetRequiredService(
                    typeof(IStringLocalizer<>).MakeGenericType(type));
                dict[$"_fallback_{fallbackIndex++}"] = localizer;
                addedTypes.Add(type);
            }

            var logger = sp.GetService<ILogger<CompositeLocalizer>>();
            return new CompositeLocalizer(dict, logger);
        });

        return services;
    }
}
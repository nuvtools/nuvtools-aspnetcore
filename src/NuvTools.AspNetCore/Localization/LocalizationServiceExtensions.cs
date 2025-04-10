using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace NuvTools.AspNetCore.Localization;

public static class LocalizationServiceExtensions
{
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
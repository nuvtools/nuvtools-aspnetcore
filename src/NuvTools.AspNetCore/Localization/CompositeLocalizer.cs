using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace NuvTools.AspNetCore.Localization;

/// <summary>
/// Provides a composite localization system that combines multiple <see cref="IStringLocalizer"/> instances
/// with support for prefix-based routing and fallback resolution.
/// </summary>
/// <remarks>
/// Keys can be prefixed with a localizer name followed by a colon (e.g., "Errors:ValidationFailed")
/// to target a specific localizer. If the prefix is not found or the key is not found in the specified
/// localizer, the system falls back to searching all registered localizers.
/// </remarks>
public class CompositeLocalizer(Dictionary<string, IStringLocalizer> namedLocalizers,
                                 ILogger<CompositeLocalizer>? logger = null)
{
    private readonly IEnumerable<IStringLocalizer> _allLocalizers = namedLocalizers.Values;

    /// <summary>
    /// Gets the localized string for the specified key.
    /// </summary>
    /// <param name="key">The localization key. Can be prefixed with a localizer name (e.g., "Errors:KeyName").</param>
    /// <returns>The localized string, or the key itself if not found.</returns>
    public LocalizedString this[string key]
    {
        get
        {
            LocalizedString result = TryResolve(key, out LocalizedString? localized)
                ? localized
                : new LocalizedString(key, key, true);

            if (result.ResourceNotFound) LogMissing(key);
            return result;
        }
    }

    /// <summary>
    /// Gets the localized string for the specified key with formatting arguments.
    /// </summary>
    /// <param name="key">The localization key. Can be prefixed with a localizer name (e.g., "Errors:KeyName").</param>
    /// <param name="arguments">The formatting arguments to apply to the localized string.</param>
    /// <returns>The formatted localized string, or the key itself if not found.</returns>
    public LocalizedString this[string key, params object[] arguments]
    {
        get
        {
            LocalizedString result = TryResolve(key, out LocalizedString? localized, arguments)
                ? localized
                : new LocalizedString(key, key, true);

            if (result.ResourceNotFound) LogMissing(key);
            return result;
        }
    }

    /// <summary>
    /// Attempts to retrieve the localized value for the specified key.
    /// </summary>
    /// <param name="key">The localization key. Can be prefixed with a localizer name (e.g., "Errors:KeyName").</param>
    /// <param name="value">When this method returns, contains the localized value if found; otherwise, the key itself.</param>
    /// <param name="arguments">The formatting arguments to apply to the localized string.</param>
    /// <returns><c>true</c> if the key was found and localized; otherwise, <c>false</c>.</returns>
    public bool TryGetValue(string key, out string value, params object[] arguments)
    {
        if (TryResolve(key, out LocalizedString? localized, arguments))
        {
            value = localized.Value;
            return true;
        }

        value = key;
        LogMissing(key);
        return false;
    }

    /// <summary>
    /// Attempts to resolve a localization key using prefix-based routing or fallback search.
    /// </summary>
    /// <param name="key">The localization key to resolve.</param>
    /// <param name="result">The resolved localized string.</param>
    /// <param name="args">Optional formatting arguments.</param>
    /// <returns><c>true</c> if the key was successfully resolved; otherwise, <c>false</c>.</returns>
    private bool TryResolve(string key, out LocalizedString result, params object[] args)
    {
        string[] parts = key.Split(':', 2);

        if (parts.Length == 2)
        {
            string prefix = parts[0];
            string realKey = parts[1];

            if (namedLocalizers.TryGetValue(prefix, out IStringLocalizer? specific))
            {
                result = args.Length > 0 ? specific[realKey, args] : specific[realKey];
                if (!result.ResourceNotFound) return true;
            }
        }

        foreach (IStringLocalizer localizer in _allLocalizers)
        {
            result = args.Length > 0 ? localizer[key, args] : localizer[key];
            if (!result.ResourceNotFound) return true;
        }

        result = new LocalizedString(key, key, true);
        return false;
    }

    /// <summary>
    /// Logs a warning when a localization key is not found.
    /// </summary>
    /// <param name="key">The missing localization key.</param>
    private void LogMissing(string key) =>
        logger?.LogWarning("Missing localization key: {Key}", key);
}



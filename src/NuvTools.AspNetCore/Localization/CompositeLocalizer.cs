using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace NuvTools.AspNetCore.Localization;

public class CompositeLocalizer(Dictionary<string, IStringLocalizer> namedLocalizers,
                                 ILogger<CompositeLocalizer>? logger = null)
{
    private readonly IEnumerable<IStringLocalizer> _allLocalizers = namedLocalizers.Values;

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

    private void LogMissing(string key) =>
        logger?.LogWarning("Missing localization key: {Key}", key);
}



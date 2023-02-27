using Microsoft.AspNetCore.WebUtilities;
using System.Globalization;

namespace NuvTools.AspNetCore.WebUtilities;

public static class ObjectExtensions
{
    public static string GetQueryString<T>(this T value, string uriBase) where T : class
    {
        var properties = from p in value.GetType().GetProperties()
                         where p.GetValue(value, null) != null
                         select
                         new
                         {
                             p.Name,
                             Value = p.GetValue(value, null).GetType() == typeof(DateTime) ?
                                     ((DateTime)p.GetValue(value, null)).ToString("yyyy-MM-ddThh:mm:ss", CultureInfo.InvariantCulture)
                                     : p.GetValue(value, null).ToString()
                         };

        //2021-11-12T00:25:15.723Z
        //yyyy-MM-ddThh:mm:ss.zzz
        var queryString = properties.ToDictionary(a => a.Name, b => b.Value);

        if (queryString.Count == 0)
            return uriBase;

        return QueryHelpers.AddQueryString(uriBase, queryString);
    }
}

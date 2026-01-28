using MudBlazor;

namespace NuvTools.AspNetCore.Blazor.MudBlazor.Converters;

/// <summary>
/// Converter for Vehicle Identification Numbers (VIN).
/// </summary>
/// <remarks>
/// <para>
/// VINs do not use the letters I, O, and Q to avoid confusion with the numbers 1, 0, and 9 respectively.
/// This converter automatically removes these invalid characters and converts the input to uppercase.
/// </para>
/// </remarks>
public sealed class VinConverter : Converter<string?>
{
    /// <summary>
    /// Characters that are not allowed in VINs.
    /// </summary>
    private static readonly char[] InvalidChars = ['I', 'O', 'Q'];

    /// <summary>
    /// Initializes a new instance of the <see cref="VinConverter"/> class.
    /// </summary>
    public VinConverter()
    {
        SetFunc = FormatVin;
        GetFunc = FormatVin;
    }

    /// <summary>
    /// Formats a VIN by removing invalid characters and converting to uppercase.
    /// </summary>
    /// <param name="value">The VIN value to format.</param>
    /// <returns>The formatted VIN, or the original value if null or empty.</returns>
    public static string? FormatVin(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        var upper = value.ToUpperInvariant();

        foreach (var invalidChar in InvalidChars)
        {
            upper = upper.Replace(invalidChar.ToString(), string.Empty);
        }

        return upper;
    }
}

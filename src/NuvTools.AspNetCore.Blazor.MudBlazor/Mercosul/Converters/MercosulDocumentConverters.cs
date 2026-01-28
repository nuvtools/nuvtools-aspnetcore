using NuvTools.AspNetCore.Blazor.MudBlazor.Converters;

namespace NuvTools.AspNetCore.Blazor.MudBlazor.Mercosul.Converters;

/// <summary>
/// Pre-configured converters for Mercosul (Southern Common Market) document formats.
/// </summary>
/// <remarks>
/// <para>Mercosul member countries: Argentina, Brazil, Paraguay, Uruguay.</para>
/// <para>Associate members: Bolivia, Chile, Colombia, Ecuador, Guyana, Peru, Suriname.</para>
/// </remarks>
public static class MercosulDocumentConverters
{
    /// <summary>
    /// Converter for vehicle license plates in Mercosul format (LLL-NANN).
    /// </summary>
    /// <remarks>
    /// The Mercosul license plate format consists of 3 letters, a digit, a letter, and 2 digits.
    /// This format was adopted by Mercosul countries starting in 2018.
    /// </remarks>
    /// <example>ABC-1D23</example>
    public static PatternStringConverter LicensePlate => new("LLL-NANN");

    /// <summary>
    /// Formats a Mercosul license plate value for display.
    /// </summary>
    /// <param name="value">The raw license plate value.</param>
    /// <returns>The formatted license plate.</returns>
    public static string? FormatLicensePlate(string? value) => LicensePlate.Format(value);
}

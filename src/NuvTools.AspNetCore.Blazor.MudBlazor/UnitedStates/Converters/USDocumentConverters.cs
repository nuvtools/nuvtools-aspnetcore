using NuvTools.AspNetCore.Blazor.MudBlazor.Converters;

namespace NuvTools.AspNetCore.Blazor.MudBlazor.UnitedStates.Converters;

/// <summary>
/// Pre-configured converters for United States documents and formats.
/// </summary>
/// <remarks>
/// Provides ready-to-use <see cref="PatternStringConverter"/> instances for common US document formats.
/// </remarks>
public static class USDocumentConverters
{
    /// <summary>
    /// Converter for phone numbers in format (NNN) NNN-NNNN.
    /// </summary>
    /// <remarks>
    /// In the United States, mobile and landline phone numbers share the same 10-digit format.
    /// </remarks>
    /// <example>(555) 123-4567</example>
    public static PatternStringConverter Phone => new("(NNN) NNN-NNNN");

    /// <summary>
    /// Converter for mobile phone numbers in format (NNN) NNN-NNNN.
    /// </summary>
    /// <example>(555) 123-4567</example>
    public static PatternStringConverter MobilePhone => Phone;

    /// <summary>
    /// Converter for landline phone numbers in format (NNN) NNN-NNNN.
    /// </summary>
    /// <example>(555) 123-4567</example>
    public static PatternStringConverter LandlinePhone => Phone;

    /// <summary>
    /// Converter for Social Security Number (SSN) in format NNN-NN-NNNN.
    /// </summary>
    /// <example>123-45-6789</example>
    public static PatternStringConverter Ssn => new("NNN-NN-NNNN");

    /// <summary>
    /// Converter for ZIP code in format NNNNN.
    /// </summary>
    /// <example>90210</example>
    public static PatternStringConverter ZipCode => new("NNNNN");

    /// <summary>
    /// Converter for ZIP+4 code in format NNNNN-NNNN.
    /// </summary>
    /// <example>90210-1234</example>
    public static PatternStringConverter ZipCodePlus4 => new("NNNNN-NNNN");

    /// <summary>
    /// Formats a phone number for display.
    /// </summary>
    /// <param name="value">The raw phone number.</param>
    /// <returns>The formatted phone number.</returns>
    public static string? FormatPhone(string? value) => Phone.Format(value);

    /// <summary>
    /// Formats a mobile phone number for display.
    /// </summary>
    /// <param name="value">The raw phone number.</param>
    /// <returns>The formatted mobile phone number.</returns>
    public static string? FormatMobilePhone(string? value) => MobilePhone.Format(value);

    /// <summary>
    /// Formats a landline phone number for display.
    /// </summary>
    /// <param name="value">The raw phone number.</param>
    /// <returns>The formatted landline phone number.</returns>
    public static string? FormatLandlinePhone(string? value) => LandlinePhone.Format(value);

    /// <summary>
    /// Formats a Social Security Number for display.
    /// </summary>
    /// <param name="value">The raw SSN value.</param>
    /// <returns>The formatted SSN.</returns>
    public static string? FormatSsn(string? value) => Ssn.Format(value);

    /// <summary>
    /// Formats a ZIP code for display.
    /// </summary>
    /// <param name="value">The raw ZIP code value.</param>
    /// <returns>The formatted ZIP code.</returns>
    public static string? FormatZipCode(string? value) => ZipCode.Format(value);

    /// <summary>
    /// Formats a ZIP+4 code for display.
    /// </summary>
    /// <param name="value">The raw ZIP+4 code value.</param>
    /// <returns>The formatted ZIP+4 code.</returns>
    public static string? FormatZipCodePlus4(string? value) => ZipCodePlus4.Format(value);
}

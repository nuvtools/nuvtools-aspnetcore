using NuvTools.AspNetCore.Blazor.MudBlazor.Converters;

namespace NuvTools.AspNetCore.Blazor.MudBlazor.Brazil.Converters;

/// <summary>
/// Pre-configured converters for Brazilian documents and formats.
/// </summary>
/// <remarks>
/// Provides ready-to-use <see cref="PatternStringConverter"/> instances for common Brazilian document formats.
/// </remarks>
public static class BrazilianDocumentConverters
{
    /// <summary>
    /// Converter for mobile phone numbers in format (NN) NNNNN-NNNN.
    /// </summary>
    /// <example>(11) 99999-9999</example>
    public static PatternStringConverter MobilePhone => new("(NN) NNNNN-NNNN");

    /// <summary>
    /// Converter for landline phone numbers in format (NN) NNNN-NNNN.
    /// </summary>
    /// <example>(11) 3333-3333</example>
    public static PatternStringConverter LandlinePhone => new("(NN) NNNN-NNNN");

    /// <summary>
    /// Converter for CPF (individual taxpayer ID) in format NNN.NNN.NNN-NN.
    /// </summary>
    /// <example>123.456.789-00</example>
    public static PatternStringConverter Cpf => new("NNN.NNN.NNN-NN");

    /// <summary>
    /// Converter for CNPJ (corporate taxpayer ID) in format NN.NNN.NNN/NNNN-NN.
    /// </summary>
    /// <example>12.345.678/0001-90</example>
    public static PatternStringConverter Cnpj => new("NN.NNN.NNN/NNNN-NN");

    /// <summary>
    /// Converter for CEP (postal code) in format NNNNN-NNN.
    /// </summary>
    /// <example>01310-100</example>
    public static PatternStringConverter Cep => new("NNNNN-NNN");

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
    /// Formats a CPF for display.
    /// </summary>
    /// <param name="value">The raw CPF value.</param>
    /// <returns>The formatted CPF.</returns>
    public static string? FormatCpf(string? value) => Cpf.Format(value);

    /// <summary>
    /// Formats a CNPJ for display.
    /// </summary>
    /// <param name="value">The raw CNPJ value.</param>
    /// <returns>The formatted CNPJ.</returns>
    public static string? FormatCnpj(string? value) => Cnpj.Format(value);

    /// <summary>
    /// Formats a CEP for display.
    /// </summary>
    /// <param name="value">The raw CEP value.</param>
    /// <returns>The formatted CEP.</returns>
    public static string? FormatCep(string? value) => Cep.Format(value);

    /// <summary>
    /// Formats a CPF or CNPJ for display, automatically detecting the type by length (11 digits for CPF, 14 for CNPJ).
    /// </summary>
    /// <param name="value">The raw CPF or CNPJ value.</param>
    /// <returns>The formatted CPF or CNPJ, or <c>null</c> if the value is empty or has an unrecognized length.</returns>
    public static string? FormatCpfOrCnpj(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return null;

        var digitsOnly = new string(value.Where(char.IsDigit).ToArray());

        return digitsOnly.Length > 11
            ? Cpf.Format(digitsOnly)
            : Cnpj.Format(digitsOnly);
    }

    /// <summary>
    /// Formats a phone number for display, automatically detecting if it's a mobile (11 digits) or landline (10 digits).
    /// </summary>
    /// <param name="value">The raw phone number.</param>
    /// <returns>The formatted phone number in the appropriate format.</returns>
    public static string? FormatPhone(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return null;

        var digitsOnly = new string(value.Where(char.IsDigit).ToArray());

        return digitsOnly.Length >= 11
            ? MobilePhone.Format(digitsOnly)
            : LandlinePhone.Format(digitsOnly);
    }
}

using MudBlazor;

namespace NuvTools.AspNetCore.Blazor.MudBlazor.Converters;

/// <summary>
/// Converter that transforms input text to uppercase.
/// </summary>
/// <remarks>
/// This converter automatically converts all input to uppercase during both
/// set (display) and get (storage) operations, ensuring consistent uppercase formatting.
/// </remarks>
public sealed class UpperCaseConverter : DeferredConverter<string?, string?>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpperCaseConverter"/> class.
    /// </summary>
    public UpperCaseConverter()
    {
        Set(value => value?.ToUpperInvariant(), value => value?.ToUpperInvariant());
    }
}

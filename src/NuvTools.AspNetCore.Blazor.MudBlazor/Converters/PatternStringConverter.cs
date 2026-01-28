using MudBlazor;

namespace NuvTools.AspNetCore.Blazor.MudBlazor.Converters;

/// <summary>
/// Pattern-based string converter for MudBlazor input components.
/// </summary>
/// <remarks>
/// <para>Pattern characters:</para>
/// <list type="bullet">
/// <item><description><c>A</c> - Alphanumeric (letters and digits)</description></item>
/// <item><description><c>N</c> - Numeric (digits only)</description></item>
/// <item><description><c>L</c> - Letter (letters only)</description></item>
/// </list>
/// <para>Any other character in the pattern is treated as a literal separator.</para>
/// </remarks>
/// <example>
/// <code>
/// // Phone number: (NN) NNNNN-NNNN
/// var phoneConverter = new PatternStringConverter("(NN) NNNNN-NNNN");
///
/// // License plate: LLL-NANN
/// var plateConverter = new PatternStringConverter("LLL-NANN");
/// </code>
/// </example>
public class PatternStringConverter : Converter<string?>
{
    private static readonly HashSet<char> PatternChars = ['A', 'N', 'L'];
    private readonly string _pattern;
    private readonly bool _toUpperCase;
    private readonly int _maxLength;

    /// <summary>
    /// Initializes a new instance of the <see cref="PatternStringConverter"/> class.
    /// </summary>
    /// <param name="pattern">The pattern string defining the format. Use A for alphanumeric, N for numeric, L for letter.</param>
    /// <param name="toUpperCase">If true, converts input to uppercase. Default is true.</param>
    /// <exception cref="ArgumentException">Thrown when pattern is null or whitespace.</exception>
    public PatternStringConverter(string pattern, bool toUpperCase = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pattern);
        _pattern = pattern;
        _toUpperCase = toUpperCase;
        _maxLength = pattern.Count(c => PatternChars.Contains(c));
        SetFunc = FormatForDisplay;
        GetFunc = NormalizeForStorage;
    }

    private string? FormatForDisplay(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        var normalized = NormalizeForStorage(value);
        if (string.IsNullOrEmpty(normalized))
            return null;

        var result = new char[_pattern.Length];
        int valueIndex = 0;

        for (int i = 0; i < _pattern.Length && valueIndex < normalized.Length; i++)
        {
            char patternChar = _pattern[i];

            if (PatternChars.Contains(patternChar))
            {
                result[i] = normalized[valueIndex++];
            }
            else
            {
                result[i] = patternChar;
            }
        }

        int actualLength = 0;
        for (int i = 0; i < result.Length; i++)
        {
            if (result[i] == '\0')
                break;
            actualLength = i + 1;
        }

        return new string(result, 0, actualLength);
    }

    private string? NormalizeForStorage(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        var result = new char[_maxLength];
        int resultIndex = 0;
        int patternIndex = 0;

        foreach (char c in value)
        {
            if (resultIndex >= _maxLength)
                break;

            while (patternIndex < _pattern.Length && !PatternChars.Contains(_pattern[patternIndex]))
            {
                patternIndex++;
            }

            if (patternIndex >= _pattern.Length)
                break;

            char patternChar = _pattern[patternIndex];
            char inputChar = _toUpperCase ? char.ToUpperInvariant(c) : c;

            bool isValid = patternChar switch
            {
                'N' => char.IsDigit(c),
                'L' => char.IsLetter(c),
                'A' => char.IsLetterOrDigit(c),
                _ => false
            };

            if (isValid)
            {
                result[resultIndex++] = inputChar;
                patternIndex++;
            }
        }

        return resultIndex == 0 ? null : new string(result, 0, resultIndex);
    }

    /// <summary>
    /// Formats the specified value according to the pattern.
    /// </summary>
    /// <param name="value">The raw value to format.</param>
    /// <returns>The formatted value, or null if the input is empty.</returns>
    public new string? Format(string? value) => FormatForDisplay(value);
}

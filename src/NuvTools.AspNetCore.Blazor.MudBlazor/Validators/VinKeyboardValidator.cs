using Microsoft.AspNetCore.Components.Web;

namespace NuvTools.AspNetCore.Blazor.MudBlazor.Validators;

/// <summary>
/// Keyboard validator for Vehicle Identification Number (VIN) input.
/// </summary>
/// <remarks>
/// <para>
/// VINs do not use the letters I, O, and Q to avoid confusion with the numbers 1, 0, and 9 respectively.
/// This validator blocks these invalid keys during input.
/// </para>
/// <para>
/// Use with MudTextField's OnKeyDown event and PreventDefault property:
/// </para>
/// <code>
/// @code {
///     private readonly VinKeyboardValidator _vinValidator = new();
/// }
///
/// &lt;MudTextField @bind-Value="Vin"
///               OnKeyDown="_vinValidator.OnKeyDown"
///               KeyDownPreventDefault="_vinValidator.IsKeyBlocked" /&gt;
/// </code>
/// </remarks>
public sealed class VinKeyboardValidator
{
    /// <summary>
    /// Keys that are not allowed in VIN input.
    /// </summary>
    private static readonly HashSet<string> InvalidKeys = ["I", "i", "O", "o", "Q", "q"];

    /// <summary>
    /// Gets a value indicating whether the last key pressed should be blocked.
    /// </summary>
    public bool IsKeyBlocked { get; private set; }

    /// <summary>
    /// Handles the key down event and determines if the key should be blocked.
    /// </summary>
    /// <param name="e">The keyboard event arguments.</param>
    public void OnKeyDown(KeyboardEventArgs e)
    {
        IsKeyBlocked = InvalidKeys.Contains(e.Key);
    }

    /// <summary>
    /// Checks if the specified key is a valid VIN character.
    /// </summary>
    /// <param name="key">The key to validate.</param>
    /// <returns><c>true</c> if the key is valid for VIN input; otherwise, <c>false</c>.</returns>
    public static bool IsValidVinKey(string key)
    {
        return !InvalidKeys.Contains(key);
    }
}

using Microsoft.JSInterop;

namespace NuvTools.AspNetCore.Blazor.JSInterop;

/// <summary>
/// Provides JavaScript interop functionality for clipboard operations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ClipboardService"/> class.
/// </remarks>
/// <param name="jsRuntime">The JavaScript runtime for interop operations.</param>
public sealed class ClipboardService(IJSRuntime jsRuntime) : IClipboardService
{
    /// <inheritdoc />
    public ValueTask<string> ReadTextAsync()
    {
        return jsRuntime.InvokeAsync<string>("navigator.clipboard.readText");
    }

    /// <inheritdoc />
    public ValueTask WriteTextAsync(string text)
    {
        return jsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
    }
}

using Microsoft.JSInterop;

namespace NuvTools.AspNetCore.JSInterop;

/// <summary>
/// Provides JavaScript interop functionality for clipboard operations.
/// </summary>
public sealed class ClipboardService
{
    private readonly IJSRuntime _jsRuntime;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClipboardService"/> class.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime for interop operations.</param>
    public ClipboardService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Reads text from the system clipboard asynchronously.
    /// </summary>
    /// <returns>A <see cref="ValueTask{TResult}"/> representing the asynchronous operation, containing the clipboard text.</returns>
    public ValueTask<string> ReadTextAsync()
    {
        return _jsRuntime.InvokeAsync<string>("navigator.clipboard.readText");
    }

    /// <summary>
    /// Writes text to the system clipboard asynchronously.
    /// </summary>
    /// <param name="text">The text to write to the clipboard.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    /// <exception cref="Exception">Thrown when the clipboard write operation fails.</exception>
    public ValueTask WriteTextAsync(string text)
    {
        try
        {
            return _jsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}

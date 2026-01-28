namespace NuvTools.AspNetCore.Blazor.JSInterop;

/// <summary>
/// Provides JavaScript interop functionality for clipboard operations.
/// </summary>
public interface IClipboardService
{
    /// <summary>
    /// Reads text from the system clipboard asynchronously.
    /// </summary>
    /// <returns>A <see cref="ValueTask{TResult}"/> representing the asynchronous operation, containing the clipboard text.</returns>
    ValueTask<string> ReadTextAsync();

    /// <summary>
    /// Writes text to the system clipboard asynchronously.
    /// </summary>
    /// <param name="text">The text to write to the clipboard.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask WriteTextAsync(string text);
}

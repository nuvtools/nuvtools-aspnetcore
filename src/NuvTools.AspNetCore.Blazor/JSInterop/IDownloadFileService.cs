using System.Text;

namespace NuvTools.AspNetCore.Blazor.JSInterop;

/// <summary>
/// Provides functionality for downloading files in Blazor applications via JavaScript interop.
/// </summary>
public interface IDownloadFileService : IAsyncDisposable
{
    /// <summary>
    /// Downloads a file from a byte array.
    /// </summary>
    /// <param name="fileName">The name of the file to download.</param>
    /// <param name="bytes">The file content as a byte array.</param>
    /// <param name="contentType">The MIME content type of the file.</param>
    /// <param name="cancellationToken">A cancellation token to observe.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask DownloadFileAsync(string fileName, byte[] bytes,
        string contentType = "application/octet-stream",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a file from a stream. Large streams are sent in chunks to stay within SignalR message size limits.
    /// </summary>
    /// <param name="fileName">The name of the file to download.</param>
    /// <param name="stream">The stream containing the file content.</param>
    /// <param name="contentType">The MIME content type of the file.</param>
    /// <param name="cancellationToken">A cancellation token to observe.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask DownloadFileAsync(string fileName, Stream stream,
        string contentType = "application/octet-stream",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a file from a text string using the specified encoding.
    /// </summary>
    /// <param name="fileName">The name of the file to download.</param>
    /// <param name="text">The text content of the file.</param>
    /// <param name="encoding">The encoding to use when converting the text to bytes.</param>
    /// <param name="contentType">The MIME content type of the file.</param>
    /// <param name="cancellationToken">A cancellation token to observe.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask DownloadFileFromTextAsync(string fileName, string text, Encoding encoding,
        string contentType = "text/plain",
        CancellationToken cancellationToken = default);
}

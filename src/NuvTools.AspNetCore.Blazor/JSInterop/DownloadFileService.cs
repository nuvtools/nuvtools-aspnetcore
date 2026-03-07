using System.Text;
using Microsoft.JSInterop;

namespace NuvTools.AspNetCore.Blazor.JSInterop;

/// <summary>
/// Provides JavaScript interop functionality for downloading files in Blazor applications.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DownloadFileService"/> class.
/// </remarks>
/// <param name="jsRuntime">The JavaScript runtime for interop operations.</param>
public sealed class DownloadFileService(IJSRuntime jsRuntime) : IDownloadFileService
{
    private const string ModulePath = "./_content/NuvTools.AspNetCore.Blazor/download-file.js";
    private const int ChunkSize = 24576; // 24KB raw → ~32KB base64 (within SignalR default limits)

    private IJSObjectReference? _module;

    private async ValueTask<IJSObjectReference> GetModuleAsync(CancellationToken cancellationToken)
    {
        return _module ??= await jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", cancellationToken, ModulePath).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask DownloadFileAsync(string fileName, byte[] bytes,
        string contentType = "application/octet-stream",
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        ArgumentNullException.ThrowIfNull(bytes);

        var module = await GetModuleAsync(cancellationToken).ConfigureAwait(false);
        var base64 = Convert.ToBase64String(bytes);
        await module.InvokeVoidAsync("downloadBase64", cancellationToken, fileName, base64, contentType).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask DownloadFileAsync(string fileName, Stream stream,
        string contentType = "application/octet-stream",
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        ArgumentNullException.ThrowIfNull(stream);

        var module = await GetModuleAsync(cancellationToken).ConfigureAwait(false);
        var buffer = new byte[ChunkSize];
        int bytesRead;

        while ((bytesRead = await stream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false)) > 0)
        {
            var base64Chunk = Convert.ToBase64String(buffer, 0, bytesRead);
            await module.InvokeVoidAsync("addChunk", cancellationToken, base64Chunk).ConfigureAwait(false);
        }

        await module.InvokeVoidAsync("downloadFromChunks", cancellationToken, fileName, contentType).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask DownloadFileFromTextAsync(string fileName, string text, Encoding encoding,
        string contentType = "text/plain",
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(encoding);

        var bytes = encoding.GetBytes(text);
        await DownloadFileAsync(fileName, bytes, contentType, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_module is not null)
        {
            await _module.DisposeAsync().ConfigureAwait(false);
            _module = null;
        }
    }
}

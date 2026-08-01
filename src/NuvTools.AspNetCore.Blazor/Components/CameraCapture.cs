namespace NuvTools.AspNetCore.Blazor.Components;

/// <summary>
/// A frame captured from the camera.
/// </summary>
public sealed class CameraCapture
{
    private byte[]? _bytes;

    /// <summary>
    /// The full data URL, as produced by the canvas (<c>data:image/jpeg;base64,...</c>).
    /// </summary>
    public required string DataUrl { get; init; }

    /// <summary>The captured width, in pixels.</summary>
    public int Width { get; init; }

    /// <summary>The captured height, in pixels.</summary>
    public int Height { get; init; }

    /// <summary>The MIME type of the captured image, read from <see cref="DataUrl"/>.</summary>
    public string ContentType
    {
        get
        {
            var start = DataUrl.IndexOf(':') + 1;
            var end = DataUrl.IndexOf(';');
            return start > 0 && end > start ? DataUrl[start..end] : string.Empty;
        }
    }

    /// <summary>
    /// The image content in Base64, already without the <c>data:...;base64,</c> prefix.
    /// </summary>
    public string Base64
    {
        get
        {
            var separator = DataUrl.IndexOf(',');
            return separator >= 0 ? DataUrl[(separator + 1)..] : string.Empty;
        }
    }

    /// <summary>The decoded image content.</summary>
    public byte[] Bytes => _bytes ??= Convert.FromBase64String(Base64);
}

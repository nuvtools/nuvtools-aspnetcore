namespace NuvTools.AspNetCore.Blazor.Components;

/// <summary>
/// Image format used when capturing a frame.
/// </summary>
public enum CameraImageFormat
{
    /// <summary>JPEG (<c>image/jpeg</c>) — lossy, honours the quality setting.</summary>
    Jpeg,

    /// <summary>PNG (<c>image/png</c>) — lossless, ignores the quality setting.</summary>
    Png,

    /// <summary>WebP (<c>image/webp</c>) — not supported by every browser.</summary>
    Webp
}

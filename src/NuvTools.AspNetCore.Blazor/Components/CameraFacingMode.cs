namespace NuvTools.AspNetCore.Blazor.Components;

/// <summary>
/// Which camera to request when starting the video stream.
/// </summary>
public enum CameraFacingMode
{
    /// <summary>The camera facing the user (front / selfie camera).</summary>
    User,

    /// <summary>The camera facing away from the user (rear camera), typical for scanning documents.</summary>
    Environment
}

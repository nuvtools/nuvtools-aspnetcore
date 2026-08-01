namespace NuvTools.AspNetCore.Blazor.Components;

/// <summary>
/// Reason why the camera could not be started.
/// </summary>
/// <param name="Name">
/// The DOMException name reported by the browser — typically <c>NotAllowedError</c> (permission denied),
/// <c>NotFoundError</c> (no camera), <c>NotReadableError</c> (camera in use by another application) or
/// <c>OverconstrainedError</c> (no camera matches the requested constraints).
/// </param>
/// <param name="Message">The message reported by the browser, in the browser's own language.</param>
public sealed record CameraError(string Name, string Message);

namespace NuvTools.AspNetCore.Blazor.Components;

/// <summary>
/// A video input device available to the browser.
/// </summary>
/// <param name="DeviceId">The identifier to pass to <c>SwitchDeviceAsync</c>.</param>
/// <param name="Label">
/// The device name. Browsers only fill it in after the user has granted camera permission at least
/// once — before that it comes back empty.
/// </param>
public sealed record CameraDevice(string DeviceId, string? Label);

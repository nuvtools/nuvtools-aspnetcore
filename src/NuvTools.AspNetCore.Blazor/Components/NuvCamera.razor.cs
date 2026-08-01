using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace NuvTools.AspNetCore.Blazor.Components;

/// <summary>
/// Renders a live camera preview and captures frames from it.
/// </summary>
/// <remarks>
/// The component owns the <c>&lt;video&gt;</c> and the capture <c>&lt;canvas&gt;</c>, and keeps its own
/// media stream — several instances can run side by side. It renders no layout and no styling of its
/// own: size it through <see cref="Class"/>/<see cref="Style"/> and draw guides through
/// <see cref="Overlay"/>. Capture the component with <c>@ref</c> to drive it.
/// </remarks>
/// <example>
/// <code>
/// &lt;NuvCamera @ref="_camera" FacingMode="CameraFacingMode.Environment"
///            Style="width:400px;height:300px" OnError="ShowCameraError" /&gt;
///
/// @code {
///     private NuvCamera _camera = default!;
///
///     private async Task ScanAsync()
///     {
///         var capture = await _camera.CaptureAsync();
///         if (capture is not null) await ReadAsync(capture.Base64);
///     }
/// }
/// </code>
/// </example>
public partial class NuvCamera : IAsyncDisposable
{
    private const string ModulePath = "./_content/NuvTools.AspNetCore.Blazor/camera.js";

    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    private ElementReference _video;
    private ElementReference _canvas;
    private IJSObjectReference? _module;
    private IJSObjectReference? _instance;

    /// <summary>Requested capture width, in pixels. Treated as ideal — the browser may pick another size.</summary>
    [Parameter] public int? Width { get; set; }

    /// <summary>Requested capture height, in pixels. Treated as ideal — the browser may pick another size.</summary>
    [Parameter] public int? Height { get; set; }

    /// <summary>Which camera to request. Ignored when <see cref="DeviceId"/> is set.</summary>
    [Parameter] public CameraFacingMode FacingMode { get; set; } = CameraFacingMode.User;

    /// <summary>Identifier of a specific camera, as returned by <see cref="GetDevicesAsync"/>.</summary>
    [Parameter] public string? DeviceId { get; set; }

    /// <summary>
    /// Whether to mirror the image. Applies to both the preview and the captured frame, so what the
    /// user sees is what gets saved. Expected for a selfie camera; leave it off for a rear camera.
    /// </summary>
    [Parameter] public bool Mirrored { get; set; }

    /// <summary>Format of the captured image.</summary>
    [Parameter] public CameraImageFormat Format { get; set; } = CameraImageFormat.Jpeg;

    /// <summary>Compression quality, from 0 to 1. Only used by <see cref="CameraImageFormat.Jpeg"/> and <see cref="CameraImageFormat.Webp"/>.</summary>
    [Parameter] public double Quality { get; set; } = 0.9;

    /// <summary>Width of the captured frame, in pixels. Defaults to the stream's native width.</summary>
    [Parameter] public int? CaptureWidth { get; set; }

    /// <summary>Height of the captured frame, in pixels. Defaults to the stream's native height.</summary>
    [Parameter] public int? CaptureHeight { get; set; }

    /// <summary>Whether to start the camera as soon as the component renders.</summary>
    [Parameter] public bool AutoStart { get; set; } = true;

    /// <summary>CSS class applied to the video element.</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>Inline style applied to the video element.</summary>
    [Parameter] public string? Style { get; set; }

    /// <summary>Content rendered right after the video, for guides, frames or hints.</summary>
    [Parameter] public RenderFragment? Overlay { get; set; }

    /// <summary>Raised once the stream is live.</summary>
    [Parameter] public EventCallback OnStarted { get; set; }

    /// <summary>Raised when the camera cannot be started — permission denied, no camera, and so on.</summary>
    [Parameter] public EventCallback<CameraError> OnError { get; set; }

    /// <summary>Raised for every captured frame.</summary>
    [Parameter] public EventCallback<CameraCapture> OnCaptured { get; set; }

    /// <summary>Any other attribute is applied to the video element.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>Whether the stream is currently live.</summary>
    public bool IsRunning { get; private set; }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && AutoStart)
            await StartAsync();
    }

    /// <summary>
    /// Starts the camera. Raises <see cref="OnStarted"/> on success and <see cref="OnError"/> on failure.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe.</param>
    /// <returns><c>true</c> when the stream is live.</returns>
    public Task<bool> StartAsync(CancellationToken cancellationToken = default)
        => StartAsync(DeviceId, cancellationToken);

    /// <summary>
    /// Starts the camera on a specific device, without changing <see cref="DeviceId"/>.
    /// </summary>
    /// <param name="deviceId">The device to use, as returned by <see cref="GetDevicesAsync"/>.</param>
    /// <param name="cancellationToken">A cancellation token to observe.</param>
    /// <returns><c>true</c> when the stream is live.</returns>
    public Task<bool> SwitchDeviceAsync(string deviceId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(deviceId);
        return StartAsync(deviceId, cancellationToken);
    }

    private async Task<bool> StartAsync(string? deviceId, CancellationToken cancellationToken)
    {
        var instance = await GetInstanceAsync(cancellationToken).ConfigureAwait(false);

        var error = await instance.InvokeAsync<CameraError?>("start", cancellationToken, new
        {
            width = Width,
            height = Height,
            facingMode = FacingMode == CameraFacingMode.Environment ? "environment" : "user",
            deviceId,
            mirrored = Mirrored
        }).ConfigureAwait(false);

        IsRunning = error is null;

        if (error is not null)
        {
            await OnError.InvokeAsync(error).ConfigureAwait(false);
            return false;
        }

        await OnStarted.InvokeAsync().ConfigureAwait(false);
        return true;
    }

    /// <summary>
    /// Stops the camera and releases the device — every track is stopped, so the camera light goes off.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (_instance is null) return;

        await _instance.InvokeVoidAsync("stop", cancellationToken).ConfigureAwait(false);
        IsRunning = false;
    }

    /// <summary>
    /// Captures the current frame. Raises <see cref="OnCaptured"/> when a frame is produced.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe.</param>
    /// <returns>The captured frame, or <c>null</c> when the stream has no image yet.</returns>
    public async Task<CameraCapture?> CaptureAsync(CancellationToken cancellationToken = default)
    {
        if (_instance is null) return null;

        var capture = await _instance.InvokeAsync<CameraCapture?>("capture", cancellationToken, new
        {
            width = CaptureWidth,
            height = CaptureHeight,
            contentType = GetContentType(),
            quality = Quality,
            mirrored = Mirrored
        }).ConfigureAwait(false);

        if (capture is not null)
            await OnCaptured.InvokeAsync(capture).ConfigureAwait(false);

        return capture;
    }

    /// <summary>
    /// Lists the video input devices available to the browser.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe.</param>
    /// <returns>The available cameras.</returns>
    public async Task<IReadOnlyList<CameraDevice>> GetDevicesAsync(CancellationToken cancellationToken = default)
    {
        var module = await GetModuleAsync(cancellationToken).ConfigureAwait(false);
        return await module.InvokeAsync<CameraDevice[]>("listDevices", cancellationToken).ConfigureAwait(false);
    }

    private string GetContentType() => Format switch
    {
        CameraImageFormat.Png => "image/png",
        CameraImageFormat.Webp => "image/webp",
        _ => "image/jpeg"
    };

    private async ValueTask<IJSObjectReference> GetModuleAsync(CancellationToken cancellationToken)
    {
        return _module ??= await JSRuntime.InvokeAsync<IJSObjectReference>(
            "import", cancellationToken, ModulePath).ConfigureAwait(false);
    }

    private async ValueTask<IJSObjectReference> GetInstanceAsync(CancellationToken cancellationToken)
    {
        if (_instance is not null) return _instance;

        var module = await GetModuleAsync(cancellationToken).ConfigureAwait(false);
        return _instance = await module.InvokeAsync<IJSObjectReference>(
            "create", cancellationToken, _video, _canvas).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_instance is not null)
            {
                await _instance.InvokeVoidAsync("dispose").ConfigureAwait(false);
                await _instance.DisposeAsync().ConfigureAwait(false);
            }

            if (_module is not null)
                await _module.DisposeAsync().ConfigureAwait(false);
        }
        catch (JSDisconnectedException) { }
        catch (ObjectDisposedException) { }
        catch (OperationCanceledException) { }
        finally
        {
            _instance = null;
            _module = null;
            IsRunning = false;
        }

        GC.SuppressFinalize(this);
    }
}

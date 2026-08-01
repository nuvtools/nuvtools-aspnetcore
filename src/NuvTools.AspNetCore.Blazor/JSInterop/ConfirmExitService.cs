using Microsoft.JSInterop;

namespace NuvTools.AspNetCore.Blazor.JSInterop;

/// <summary>
/// Provides JavaScript interop functionality for confirming before the user leaves the page in Blazor applications.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ConfirmExitService"/> class.
/// </remarks>
/// <param name="jsRuntime">The JavaScript runtime for interop operations.</param>
public sealed class ConfirmExitService(IJSRuntime jsRuntime) : IConfirmExitService
{
    private const string ModulePath = "./_content/NuvTools.AspNetCore.Blazor/confirm-exit.js";

    private IJSObjectReference? _module;

    /// <inheritdoc />
    public bool IsEnabled { get; private set; }

    private async ValueTask<IJSObjectReference> GetModuleAsync(CancellationToken cancellationToken)
    {
        return _module ??= await jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", cancellationToken, ModulePath).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask EnableAsync(CancellationToken cancellationToken = default)
    {
        if (IsEnabled) return;

        var module = await GetModuleAsync(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("enable", cancellationToken).ConfigureAwait(false);
        IsEnabled = true;
    }

    /// <inheritdoc />
    public async ValueTask DisableAsync(CancellationToken cancellationToken = default)
    {
        if (!IsEnabled || _module is null) return;

        await _module.InvokeVoidAsync("disable", cancellationToken).ConfigureAwait(false);
        IsEnabled = false;
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_module is null) return;

        try
        {
            if (IsEnabled)
                await _module.InvokeVoidAsync("disable").ConfigureAwait(false);

            await _module.DisposeAsync().ConfigureAwait(false);
        }
        catch (JSDisconnectedException) { }
        catch (ObjectDisposedException) { }
        catch (OperationCanceledException) { }
        finally
        {
            _module = null;
            IsEnabled = false;
        }
    }
}

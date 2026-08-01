namespace NuvTools.AspNetCore.Blazor.JSInterop;

/// <summary>
/// Asks the browser to confirm before the user closes or reloads the tab, or navigates away from the application.
/// </summary>
/// <remarks>
/// The confirmation is raised by the browser through the <c>beforeunload</c> event, so the warning text is
/// defined by the browser and cannot be customized. It does not cover in-app (SPA) navigation.
/// </remarks>
/// <example>
/// <code>
/// [Inject] IConfirmExitService ConfirmExit { get; set; } = default!;
///
/// protected override async Task OnAfterRenderAsync(bool firstRender)
/// {
///     if (HasUnsavedChanges())
///         await ConfirmExit.EnableAsync();
///     else
///         await ConfirmExit.DisableAsync();
/// }
///
/// public async ValueTask DisposeAsync() => await ConfirmExit.DisableAsync();
/// </code>
/// </example>
public interface IConfirmExitService : IAsyncDisposable
{
    /// <summary>
    /// Gets a value indicating whether the exit confirmation is currently active.
    /// </summary>
    bool IsEnabled { get; }

    /// <summary>
    /// Activates the exit confirmation. Calling it while already active does nothing.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask EnableAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deactivates the exit confirmation. Calling it while already inactive does nothing.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask DisableAsync(CancellationToken cancellationToken = default);
}

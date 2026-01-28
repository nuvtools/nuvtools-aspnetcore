namespace NuvTools.AspNetCore.Blazor.Services;

/// <summary>
/// Provides a service for managing loading indicator state.
/// </summary>
/// <remarks>
/// <para>
/// Use this service to coordinate loading overlays across components.
/// Register as scoped in DI and inject where needed.
/// </para>
/// <example>
/// <code>
/// @inject ILoadingService LoadingService
///
/// &lt;div class="loading-overlay" style="display: @(LoadingService.IsLoading ? "block" : "none")"&gt;
///     Loading...
/// &lt;/div&gt;
///
/// @code {
///     protected override void OnInitialized()
///     {
///         LoadingService.OnChange += StateHasChanged;
///     }
///
///     public void Dispose()
///     {
///         LoadingService.OnChange -= StateHasChanged;
///     }
/// }
/// </code>
/// </example>
/// </remarks>
public interface ILoadingService
{
    /// <summary>
    /// Gets a value indicating whether the loading indicator should be shown.
    /// </summary>
    bool IsLoading { get; }

    /// <summary>
    /// Shows the loading indicator.
    /// </summary>
    void Show();

    /// <summary>
    /// Hides the loading indicator.
    /// </summary>
    void Hide();

    /// <summary>
    /// Executes an async operation while showing the loading indicator.
    /// Automatically hides the indicator when complete, even if an exception occurs.
    /// </summary>
    /// <param name="work">The async operation to execute.</param>
    /// <returns>A task representing the async operation.</returns>
    Task RunAsync(Func<Task> work);

    /// <summary>
    /// Occurs when the loading state changes.
    /// </summary>
    event Action? OnChange;
}

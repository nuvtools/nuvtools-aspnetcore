namespace NuvTools.AspNetCore.Blazor.MudBlazor.Services;

/// <summary>
/// Default implementation of <see cref="ILoadingService"/> for managing loading indicator state.
/// </summary>
/// <remarks>
/// <para>
/// This service uses a counter to track nested Show/Hide calls, ensuring correct behavior
/// when multiple components request loading state simultaneously.
/// </para>
/// <para>
/// Should be registered as scoped to maintain state per Blazor circuit.
/// </para>
/// </remarks>
public sealed class LoadingService : ILoadingService
{
    private int _counter;

    /// <inheritdoc />
    public bool IsLoading => _counter > 0;

    /// <inheritdoc />
    public event Action? OnChange;

    /// <inheritdoc />
    public void Show()
    {
        Interlocked.Increment(ref _counter);
        OnChange?.Invoke();
    }

    /// <inheritdoc />
    public void Hide()
    {
        var value = Interlocked.Decrement(ref _counter);
        if (value < 0) _counter = 0;
        OnChange?.Invoke();
    }

    /// <inheritdoc />
    public async Task RunAsync(Func<Task> work)
    {
        Show();
        try
        {
            await work().ConfigureAwait(false);
        }
        finally
        {
            Hide();
        }
    }
}

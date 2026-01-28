using Microsoft.AspNetCore.Components;
using MudBlazor;
using NuvTools.AspNetCore.Blazor.JSInterop;
using NuvTools.AspNetCore.Blazor.MudBlazor.Services;
using NuvTools.Common.ResultWrapper;
using NuvTools.Data.Paging;

namespace NuvTools.AspNetCore.Blazor.MudBlazor.Components;

/// <summary>
/// Abstract base class for pages with server-side paged lists using MudBlazor MudTable.
/// </summary>
/// <typeparam name="TItem">The type of DTO displayed in the table.</typeparam>
/// <typeparam name="TFilter">The filter type that inherits from <see cref="PagingFilter{TOrdering}"/>.</typeparam>
/// <typeparam name="TOrdering">The enum type for sorting options.</typeparam>
/// <remarks>
/// <para>This base class provides:</para>
/// <list type="bullet">
/// <item><description>Session storage persistence for filter state</description></item>
/// <item><description>Integration with <see cref="ILoadingService"/> for loading indicators</description></item>
/// <item><description>MudTable server-side data binding</description></item>
/// <item><description>Sort direction mapping between NuvTools and MudBlazor</description></item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// public class UserListPage : MudTablePageBase&lt;UserDto, UserFilter, UserSortColumn&gt;
/// {
///     [Inject] private IUserService UserService { get; set; } = default!;
///
///     protected override string SessionStorageKey =&gt; "user-list-filter";
///
///     protected override UserFilter CreateDefaultFilter() =&gt; new()
///     {
///         SortColumn = UserSortColumn.Name,
///         PageSize = 25
///     };
///
///     protected override async Task&lt;IResult&lt;PagingWithEnumerableList&lt;UserDto&gt;&gt;&gt; FetchDataAsync(
///         UserFilter filter, CancellationToken cancellationToken)
///     {
///         return await UserService.GetUsersAsync(filter, cancellationToken);
///     }
///
///     protected override UserSortColumn MapSortLabelToOrdering(string sortLabel)
///     {
///         return Enum.Parse&lt;UserSortColumn&gt;(sortLabel);
///     }
/// }
/// </code>
/// </example>
public abstract class MudTablePageBase<TItem, TFilter, TOrdering> : ComponentBase
    where TFilter : PagingFilter<TOrdering>
    where TOrdering : struct, Enum
{
    /// <summary>
    /// Gets or sets the session storage service for persisting filter state.
    /// </summary>
    [Inject]
    protected ISessionStorageService SessionStorage { get; set; } = default!;

    /// <summary>
    /// Gets or sets the loading service for managing loading indicator state.
    /// This service is optional and null-safe.
    /// </summary>
    [Inject]
    protected ILoadingService? LoadingService { get; set; }

    /// <summary>
    /// Gets the session storage key for persisting the filter state.
    /// </summary>
    protected abstract string SessionStorageKey { get; }

    /// <summary>
    /// Gets or sets the current filter model.
    /// </summary>
    public TFilter ModelFilter { get; set; } = default!;

    /// <summary>
    /// Gets or sets the paged data returned from the API.
    /// </summary>
    public PagingWithEnumerableList<TItem>? PagedData { get; set; }

    /// <summary>
    /// Gets or sets the reference to the MudTable component.
    /// </summary>
    public MudTable<TItem> Table { get; set; } = default!;

    private bool _firstTime = true;

    /// <summary>
    /// Gets or sets the current page index (0-based, same as MudBlazor and NuvTools.Data).
    /// </summary>
    public int CurrentPage
    {
        get => ModelFilter?.PageIndex ?? 0;
        set
        {
            if (!_firstTime && ModelFilter != null)
                ModelFilter.PageIndex = value;
        }
    }

    /// <summary>
    /// Creates the default filter when no stored filter exists.
    /// </summary>
    /// <returns>A new instance of the filter with default values.</returns>
    protected abstract TFilter CreateDefaultFilter();

    /// <summary>
    /// Fetches data from the API service.
    /// </summary>
    /// <param name="filter">The filter to apply.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The paged result containing the data.</returns>
    protected abstract Task<IResult<PagingWithEnumerableList<TItem>>> FetchDataAsync(
        TFilter filter,
        CancellationToken cancellationToken);

    /// <summary>
    /// Maps a MudBlazor sort label string to the ordering enum value.
    /// </summary>
    /// <param name="sortLabel">The sort label from MudBlazor.</param>
    /// <returns>The corresponding enum value.</returns>
    protected abstract TOrdering MapSortLabelToOrdering(string sortLabel);

    /// <summary>
    /// Converts the current ordering enum value to a sort label string.
    /// Default implementation uses <see cref="Enum.ToString()"/>.
    /// </summary>
    /// <returns>The sort label string.</returns>
    public virtual string GetSortLabel() => ModelFilter?.SortColumn.ToString() ?? string.Empty;

    /// <summary>
    /// Converts NuvTools SortDirection to MudBlazor SortDirection.
    /// </summary>
    /// <returns>The MudBlazor sort direction.</returns>
    public SortDirection GetSortDirection()
    {
        if (ModelFilter == null)
            return SortDirection.Ascending;

        return ModelFilter.SortDirection switch
        {
            NuvTools.Data.Sorting.Enumerations.SortDirection.DESC => SortDirection.Descending,
            _ => SortDirection.Ascending
        };
    }

    /// <summary>
    /// Called during <see cref="OnInitializedAsync"/> before filter initialization.
    /// Override to perform additional setup.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual Task OnInitializingAsync() => Task.CompletedTask;

    /// <summary>
    /// Called during <see cref="OnInitializedAsync"/> after filter initialization.
    /// Override to load lookup data, breadcrumbs, etc.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual Task OnInitializedFilterAsync() => Task.CompletedTask;

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        await OnInitializingAsync().ConfigureAwait(false);

        ModelFilter = await SessionStorage.GetItemAsync<TFilter>(SessionStorageKey).ConfigureAwait(false)
                      ?? CreateDefaultFilter();

        await OnInitializedFilterAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Called after data is successfully loaded.
    /// Override to perform post-load operations.
    /// </summary>
    /// <param name="data">The loaded paged data.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual Task OnDataLoadedAsync(PagingWithEnumerableList<TItem> data)
        => Task.CompletedTask;

    /// <summary>
    /// Called when data fetch fails. Override to customize error handling.
    /// </summary>
    /// <param name="result">The failed result containing error details.</param>
    protected virtual void OnFetchDataFailed(IResult result) { }

    /// <summary>
    /// Called to save the filter to session storage.
    /// Override to customize save behavior (e.g., exclude transient properties from filter).
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual async Task SaveFilterAsync()
        => await SessionStorage.SetItemAsync(SessionStorageKey, ModelFilter).ConfigureAwait(false);

    /// <summary>
    /// Server reload method for MudTable binding.
    /// </summary>
    /// <param name="state">The table state from MudBlazor.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The table data for MudBlazor.</returns>
    public async Task<TableData<TItem>> ServerReload(TableState state, CancellationToken cancellationToken)
    {
        LoadingService?.Show();
        StateHasChanged();

        try
        {
            if (string.IsNullOrEmpty(state.SortLabel))
            {
                state.SortLabel = GetSortLabel();
                state.SortDirection = GetSortDirection();
            }

            ModelFilter.SortColumn = MapSortLabelToOrdering(state.SortLabel);
            ModelFilter.SortDirection = state.SortDirection == SortDirection.Descending
                ? NuvTools.Data.Sorting.Enumerations.SortDirection.DESC
                : NuvTools.Data.Sorting.Enumerations.SortDirection.ASC;

            if (_firstTime) _firstTime = false;

            await SaveFilterAsync().ConfigureAwait(false);

            var result = await FetchDataAsync(ModelFilter, cancellationToken).ConfigureAwait(false);

            if (!result.Succeeded && !result.ContainsNotFound)
            {
                OnFetchDataFailed(result);
                return new TableData<TItem> { TotalItems = 0, Items = [] };
            }

            PagedData = result.Data!;

            await OnDataLoadedAsync(PagedData).ConfigureAwait(false);

            return new TableData<TItem> { TotalItems = PagedData.Total, Items = PagedData.List };
        }
        finally
        {
            LoadingService?.Hide();
            StateHasChanged();
        }
    }

    /// <summary>
    /// Helper method to reload the table data.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected Task ReloadTableAsync() => Table.ReloadServerData();
}

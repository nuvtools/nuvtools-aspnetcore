using Microsoft.JSInterop;
using System.Text.Json;

namespace NuvTools.AspNetCore.Blazor.JSInterop;

/// <summary>
/// Provides JavaScript interop functionality for browser localStorage operations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="LocalStorageService"/> class.
/// </remarks>
/// <param name="jsRuntime">The JavaScript runtime for interop operations.</param>
public sealed class LocalStorageService(IJSRuntime jsRuntime) : ILocalStorageService
{
    /// <inheritdoc />
    public async ValueTask<T?> GetItemAsync<T>(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        var json = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", key).ConfigureAwait(false);
        return json is null ? default : JsonSerializer.Deserialize<T>(json);
    }

    /// <inheritdoc />
    public ValueTask<string?> GetItemAsStringAsync(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        return jsRuntime.InvokeAsync<string?>("localStorage.getItem", key);
    }

    /// <inheritdoc />
    public async ValueTask SetItemAsync<T>(string key, T value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        var json = JsonSerializer.Serialize(value);
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", key, json).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public ValueTask SetItemAsStringAsync(string key, string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        return jsRuntime.InvokeVoidAsync("localStorage.setItem", key, value);
    }

    /// <inheritdoc />
    public ValueTask RemoveItemAsync(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        return jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
    }

    /// <inheritdoc />
    public ValueTask ClearAsync()
    {
        return jsRuntime.InvokeVoidAsync("localStorage.clear");
    }

    /// <inheritdoc />
    public ValueTask<int> LengthAsync()
    {
        return jsRuntime.InvokeAsync<int>("eval", "localStorage.length");
    }

    /// <inheritdoc />
    public ValueTask<string?> KeyAsync(int index)
    {
        return jsRuntime.InvokeAsync<string?>("localStorage.key", index);
    }

    /// <inheritdoc />
    public async ValueTask<bool> ContainKeyAsync(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        var value = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", key).ConfigureAwait(false);
        return value is not null;
    }
}

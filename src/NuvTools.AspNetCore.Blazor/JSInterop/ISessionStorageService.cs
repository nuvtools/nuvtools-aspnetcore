namespace NuvTools.AspNetCore.Blazor.JSInterop;

/// <summary>
/// Provides JavaScript interop functionality for browser sessionStorage operations.
/// </summary>
public interface ISessionStorageService
{
    /// <summary>
    /// Gets an item from sessionStorage and deserializes it from JSON.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the stored value to.</typeparam>
    /// <param name="key">The key of the item to retrieve.</param>
    /// <returns>A <see cref="ValueTask{TResult}"/> representing the asynchronous operation, containing the deserialized value or default if not found.</returns>
    ValueTask<T?> GetItemAsync<T>(string key);

    /// <summary>
    /// Gets an item from sessionStorage as a raw string.
    /// </summary>
    /// <param name="key">The key of the item to retrieve.</param>
    /// <returns>A <see cref="ValueTask{TResult}"/> representing the asynchronous operation, containing the stored string value or null if not found.</returns>
    ValueTask<string?> GetItemAsStringAsync(string key);

    /// <summary>
    /// Sets an item in sessionStorage by serializing the value to JSON.
    /// </summary>
    /// <typeparam name="T">The type of the value to store.</typeparam>
    /// <param name="key">The key of the item to set.</param>
    /// <param name="value">The value to serialize and store.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask SetItemAsync<T>(string key, T value);

    /// <summary>
    /// Sets an item in sessionStorage as a raw string.
    /// </summary>
    /// <param name="key">The key of the item to set.</param>
    /// <param name="value">The string value to store.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask SetItemAsStringAsync(string key, string value);

    /// <summary>
    /// Removes an item from sessionStorage.
    /// </summary>
    /// <param name="key">The key of the item to remove.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask RemoveItemAsync(string key);

    /// <summary>
    /// Clears all items from sessionStorage.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask ClearAsync();

    /// <summary>
    /// Gets the number of items stored in sessionStorage.
    /// </summary>
    /// <returns>A <see cref="ValueTask{TResult}"/> representing the asynchronous operation, containing the number of stored items.</returns>
    ValueTask<int> LengthAsync();

    /// <summary>
    /// Gets the key at the specified index in sessionStorage.
    /// </summary>
    /// <param name="index">The zero-based index of the key to retrieve.</param>
    /// <returns>A <see cref="ValueTask{TResult}"/> representing the asynchronous operation, containing the key at the specified index or null if the index is out of range.</returns>
    ValueTask<string?> KeyAsync(int index);

    /// <summary>
    /// Checks if a key exists in sessionStorage.
    /// </summary>
    /// <param name="key">The key to check for existence.</param>
    /// <returns>A <see cref="ValueTask{TResult}"/> representing the asynchronous operation, containing true if the key exists, otherwise false.</returns>
    ValueTask<bool> ContainKeyAsync(string key);
}

using Microsoft.Extensions.Caching.Distributed;

namespace IpAddressToLocation.Interfaces;

public interface IDistributedObjectCache
{

    Task<T?> GetAsync<T>(string key, CancellationToken token = default);

    /// <summary>
    /// Sets a value with the given key.
    /// </summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="value">The value to set in the cache.</param>
    /// <param name="options">The cache options for the value.</param>
    /// <param name="token"></param>
    Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions options, CancellationToken token = default);
}
using System.Text;
using IpAddressToLocation.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace IpAddressToLocation;

public class DistributedObjectCache : IDistributedObjectCache, IDistributedCache
{
    private readonly IDistributedCache _cache;

    public DistributedObjectCache(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken token = default)
    {
        var data = await _cache.GetAsync(key, token);

        if (data == null)
            return default;

        using MemoryStream ms = new MemoryStream(data);
        using BinaryReader br = new BinaryReader(ms);
        return JsonConvert.DeserializeObject<T>(br.ReadString());// Rather than using JSON I would convert this to Protobuf serialisation to save space and speed up the serialisation/deserialisation of objects
    }

    public async Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions options, CancellationToken token = default)
    {
        using var stream = new MemoryStream();
        await using var writer = new BinaryWriter(stream, Encoding.UTF8, false);
        writer.Write(JsonConvert.SerializeObject(value));
        writer.Close();
        await _cache.SetAsync(key, stream.ToArray(), options, token);

    }

    public byte[]? Get(string key)
    {
        return _cache.Get(key);
    }

    public Task<byte[]?> GetAsync(string key, CancellationToken token = new())
    {
        return _cache.GetAsync(key, token);
    }

    public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
    {
        _cache.Set(key, value, options);
    }

    public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options,
        CancellationToken token = new())
    {
        return _cache.SetAsync(key, value, options, token);
    }

    public void Refresh(string key)
    {
        _cache.Refresh(key);
    }

    public Task RefreshAsync(string key, CancellationToken token = new())
    {
        return _cache.RefreshAsync(key, token);
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
    }

    public Task RemoveAsync(string key, CancellationToken token = new())
    {
        return _cache.RemoveAsync(key, token);
    }
}
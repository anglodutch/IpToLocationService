using System.Net;
using IpAddressToLocation.Interfaces;
using IpAddressToLocation.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

[assembly: HostingStartup(typeof(IpAddressToLocation.ServiceInjection))]

namespace IpAddressToLocation;


//this class could be expanded further to have multiple providers that use could be used as fall backs depending on service availability and how critical it was to obtain the location. 
public class AddressToLocationService : IIpAddressToLocationService
{
    private readonly ILocationProvider _provider;
    private readonly IDistributedObjectCache _cache;

    private readonly AddressToLocationServiceOptions _options;

    public AddressToLocationService(ILocationProvider provider, IDistributedObjectCache cache, IOptions<AddressToLocationServiceOptions> options)
    {
        _provider = provider;
        _cache = cache;
        _options = options.Value;
    }

    public async Task<Location> GetLocation(IPAddress address)
    {
        var key = address.ToString();
        
        var cached = await _cache.GetAsync<Location>(key);
        
        if (cached != null)
            return cached;

        var location = await _provider.GetLocation(address);

        await _cache.SetAsync(key, location, new DistributedCacheEntryOptions{SlidingExpiration = _options.CacheExpiryTime });
        
        return location;
    }
}

public class AddressToLocationServiceOptions
{
    public static string AddressToLocationService = "AddressToLocationService";

    public TimeSpan CacheExpiryTime { get; set; } = TimeSpan.FromMinutes(10);
}

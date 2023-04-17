using IpAddressToLocation.Interfaces;
using IpAddressToLocation.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace IpAddressToLocation;

public static class ServiceInjection
{
    public static void ConfigureIpAddressToLocation(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<AddressToLocationServiceOptions>(builder.Configuration.GetSection(AddressToLocationServiceOptions.AddressToLocationService));

        builder.Services.Configure<LocationProviderIpLocationOptions>(builder.Configuration.GetSection(LocationProviderIpLocationOptions.LocationProviderIpLocation));

        var ipLocationOptions  = builder.Configuration.GetSection(LocationProviderIpLocationOptions.LocationProviderIpLocation)
            .Get<LocationProviderIpLocationOptions>();
        builder.Services.AddTransient<IIpAddressToLocationService, AddressToLocationService>();
        builder.Services.AddTransient<ILocationProvider, LocationProviderIpLocation>();
        builder.Services.AddTransient<IDistributedObjectCache, DistributedObjectCache>();
        builder.Services.AddHttpClient<Location>("IpLocation", client =>
            {
                client.BaseAddress = ipLocationOptions.BaseAddress;
                foreach (var header in ipLocationOptions.Headers)
                {
                    client.DefaultRequestHeaders.Add(header.Name, header.Value);// Id doing this properly these API keys would be stored in a key Vault 

                }
            }).AddTransientHttpErrorPolicy(b => b.WaitAndRetryAsync(ipLocationOptions.RetryTimeouts))
        ;
        
    }
}

public class LocationProviderIpLocationOptions
{
    public static string LocationProviderIpLocation => "LocationProviderIpLocation";

    public Uri BaseAddress { get; set; }

    public List<HeaderParameter> Headers { get; set; } = new();

    public List<TimeSpan> RetryTimeouts { get; set; } = new();
 }

public class HeaderParameter
{
    public string Name { get; set; }
    public string Value { get; set; }
}




using System.Net;
using IpAddressToLocation.Interfaces;
using IpAddressToLocation.Model;
using Newtonsoft.Json;

namespace IpAddressToLocation;

public class LocationProviderIpLocation : ILocationProvider
{
    protected IHttpClientFactory _httpClientFactory;

    public async Task<Location> GetLocation(IPAddress address)
    {
        var client = _httpClientFactory.CreateClient("IpLocation");

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "ip", address.ToString() },
                }),
            };
            using var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Location>(body);//If I was more aware of what was required from this service then I would probably have a different object to pas back rather then than exactly the object that the service returns.
                                                                 //I could therefore have different services all convert to the same format. 
    }

    public LocationProviderIpLocation(IHttpClientFactory httpClientFactory) 
    {
        _httpClientFactory = httpClientFactory;
    }
}
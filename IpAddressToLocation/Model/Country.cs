using Newtonsoft.Json;

namespace IpAddressToLocation.Model;

public class Country
{
    public string code { get; set; }
    public string name { get; set; }
    public string capital { get; set; }
    public string currency { get; set; }

    [JsonProperty("phone-code")]
    public string phoneCode { get; set; }
}
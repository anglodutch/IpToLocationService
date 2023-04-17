namespace IpAddressToLocation.Model;

public class Location
{
    public string ip { get; set; }
    public Continent continent { get; set; }
    public Country country { get; set; }
    public string region { get; set; }
    public string city { get; set; }
    public double? latitude { get; set; }
    public double? longitude { get; set; }
}
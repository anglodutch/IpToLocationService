using System.Net;
using IpAddressToLocation.Model;

namespace IpAddressToLocation.Interfaces;

public interface IIpAddressToLocationService
{
    Task<Location> GetLocation(IPAddress address);
}
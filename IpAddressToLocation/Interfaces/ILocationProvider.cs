using System.Net;
using IpAddressToLocation.Model;

namespace IpAddressToLocation.Interfaces;

public interface ILocationProvider
{
    Task<Location> GetLocation(IPAddress address);
}
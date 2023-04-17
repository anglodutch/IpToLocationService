using System.Net;
using IpAddressToLocation.Interfaces;
using IpAddressToLocation.Model;
using Microsoft.AspNetCore.Mvc;

namespace IpAddressToLocationService.Controllers;

[ApiController]
[Route("[controller]")]
public class IpAddressToLocationController : ControllerBase
{
    private readonly ILogger<IpAddressToLocationController> _logger;
    private readonly IIpAddressToLocationService _addressToLocationService;

    public IpAddressToLocationController(ILogger<IpAddressToLocationController> logger, IIpAddressToLocationService addressToLocationService)
    {
        _logger = logger;
        _addressToLocationService = addressToLocationService;
    }

    [HttpGet(Name = "GetLocation")]
    public async Task<ActionResult<Location>> Get([IpAddress] string ipAddress)
    {
        return await _addressToLocationService.GetLocation(IPAddress.Parse(ipAddress));
    }
}
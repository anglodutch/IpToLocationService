using System.Net;
using IpAddressToLocation.Interfaces;
using IpAddressToLocation.Model;
using IpAddressToLocationService.Controllers;
using Microsoft.Extensions.Logging;
using Moq;

namespace IpAddressToLocationServiceTests
{
    public class IpAddressToLocationControllerTests
    {
        private Mock<IIpAddressToLocationService> _addressToLocationService;
        private Mock<ILogger<IpAddressToLocationController>> _logger;
        private IpAddressToLocationController _sut;
        private Location _location;

        [SetUp]
        public void Setup()
        {

            _location = new Location
            {
                ip = "8.8.8.8",
                continent = new Continent
                {
                    code = "NA",
                    name = "North America"
                },
                country = new Country
                {
                    code = "US",
                    name = "United States",
                    capital = "Washington",
                    currency = "USD",
                    phoneCode = "1"
                },
                region = "California",
                city = " Mountain View",
                latitude = 37.4223,
                longitude = -122.085
            };

            _addressToLocationService = new Mock<IIpAddressToLocationService>();
            _logger = new Mock<ILogger<IpAddressToLocationController>>();

            _addressToLocationService.Setup(s => s.GetLocation(It.IsAny<IPAddress>())).ReturnsAsync(_location);

            _sut = new IpAddressToLocationController(_logger.Object, _addressToLocationService.Object);
        }

        [Test]
        public async Task Get_Test()
        {
            var response = await _sut.Get("8.8.8.8");

            _addressToLocationService.Verify(s => s.GetLocation(It.Is<IPAddress>(a => a.ToString() == "8.8.8.8")),
                Times.Once);

            Assert.That(response.Value, Is.Not.Null);
            Assert.That(response.Value, Is.EqualTo(_location));
        }
    }
}
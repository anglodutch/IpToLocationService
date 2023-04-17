using System.Net;
using IpAddressToLocation;
using IpAddressToLocation.Interfaces;
using IpAddressToLocation.Model;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace IpAddressToLocationTests
{
    public class AddressToLocationTests
    {
        private Mock<ILocationProvider> _locationProvider;
        private Mock<IDistributedObjectCache> _distributedObjectCache;
        private Mock<IOptions<AddressToLocationServiceOptions>> _options;
        private AddressToLocationService _sut;
        private string _testAddress;
        private Location _response;

        [SetUp]
        public void Setup()
        {
            
            _testAddress = "8.8.8.8";

            _response = new Location
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

            _locationProvider = new Mock<ILocationProvider>();
            _distributedObjectCache = new Mock<IDistributedObjectCache>();
            _options = new Mock<IOptions<AddressToLocationServiceOptions>>();

            _options.Setup(o => o.Value).Returns(new AddressToLocationServiceOptions
                { CacheExpiryTime = TimeSpan.FromMinutes(5) });

            _sut = new AddressToLocationService(_locationProvider.Object, _distributedObjectCache.Object, _options.Object);
        }

        [Test]
        public async Task GetLocation_Uncached_Test()
        {
           
            _locationProvider.Setup(p => p.GetLocation(It.IsAny<IPAddress>())).ReturnsAsync(_response);

            var result = await _sut.GetLocation(IPAddress.Parse(_testAddress));

            _distributedObjectCache.Verify(c => c.GetAsync<Location>(It.Is<string>(s => s == _testAddress), It.IsAny<CancellationToken>()), Times.Once);
            _distributedObjectCache.Verify(c => c.SetAsync(It.Is<string>(s => s == _testAddress), It.Is<Location>(l => l == _response), It.Is<DistributedCacheEntryOptions>(o => o.SlidingExpiration.HasValue && o.SlidingExpiration.Value.TotalMinutes == 5.0), It.Is<CancellationToken>(t => t ==CancellationToken.None)), Times.Once);

            Assert.AreEqual(_response, result);
        }

        [Test]
        public async Task GetLocation_Cached_Test()
        {
            _distributedObjectCache.Setup(c => c.GetAsync<Location>(It.Is<string>(s => s == _testAddress), It.IsAny<CancellationToken>())).ReturnsAsync(_response);


            var result = await _sut.GetLocation(IPAddress.Parse(_testAddress));

            _distributedObjectCache.Verify(c => c.SetAsync(It.Is<string>(s => s == _testAddress), It.Is<Location>(l => l == _response), It.IsAny<DistributedCacheEntryOptions>(), It.Is<CancellationToken>(t => t == CancellationToken.None)), Times.Never);
            _locationProvider.Verify(p => p.GetLocation(It.IsAny<IPAddress>()), Times.Never);

            Assert.AreEqual(_response, result);
        }
    }
}
using IpAddressToLocation;
using IpAddressToLocation.Model;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace IpAddressToLocationTests
{
    public class DistributedObjectCacheTests
    {
        private Mock<IDistributedCache> _distributedCache;
        private DistributedObjectCache _sut;
        private Location _testData;
        private byte[] _serialisedData;

        [SetUp]
        public void Setup()
        {
            _distributedCache = new Mock<IDistributedCache>();

            _sut = new DistributedObjectCache(_distributedCache.Object);

            _testData = new Location
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

            MemoryStream ms = new MemoryStream();
            var br = new BinaryWriter(ms);
            br.Write(JsonConvert.SerializeObject(_testData));
            br.Close();
            _serialisedData = ms.ToArray();
        }

        [Test]
        public async Task SetAsyncTTest()
        {
            var testKey = "TestKey";

            var cacheOptions = new DistributedCacheEntryOptions();

            await _sut.SetAsync(testKey, _testData, cacheOptions, CancellationToken.None);

            _distributedCache.Verify(
                c => c.SetAsync(It.Is<string>(s => s == testKey), It.Is<byte[]>(d => d.SequenceEqual(_serialisedData)),
                    It.Is<DistributedCacheEntryOptions>(o => o == cacheOptions),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetAsyncTTest()
        {
            var testKey = "TestKey";

            _distributedCache.Setup(
                c => c.GetAsync(It.Is<string>(s => s == testKey),It.IsAny<CancellationToken>())).ReturnsAsync(_serialisedData);

            var location = await _sut.GetAsync<Location>(testKey, CancellationToken.None);

            Assert.That( location, Is.Not.Null );

            Assert.That(location!.ip, Is.EqualTo(_testData.ip));
            Assert.That(location.latitude, Is.EqualTo(_testData.latitude));
            Assert.That(location.longitude, Is.EqualTo(_testData.longitude));
        }
    }
}
using IpAddressToLocation;
using System.Net;

namespace IpAddressToLocationTests
{
    public class LocationProviderIpLocationTests
    {
        private Mock<IHttpClientFactory> _httpClientFactory;
        private LocationProviderIpLocation _sut;
        private string _testAddress;

        [SetUp]
        public void Setup()
        {
            _httpClientFactory = new Mock<IHttpClientFactory>();
            _sut = new LocationProviderIpLocation(_httpClientFactory.Object);

            _testAddress = "8.8.8.8";
        }

        [Test]
        public void GetLocationTest()
        {
            _sut.GetLocation(IPAddress.Parse(_testAddress));
        }
    }
}
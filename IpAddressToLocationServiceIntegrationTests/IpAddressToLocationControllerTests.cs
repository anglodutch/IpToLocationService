using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Newtonsoft.Json;
using IpAddressToLocation.Model;

namespace IpAddressToLocationServiceIntegrationTests
{
    public class IpAddressToLocationControllerTests
    {

        private WebApplicationFactory<Program> _factory;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _factory = new WebApplicationFactory<Program>();
        }

        [SetUp]
        public void Setup()
        {
        }

        [TestCase("8.8.8.8", 37.4223, -122.085)]
        [TestCase("8.8.4.4", 45.5017, -73.5673)]
        [TestCase("2001:4860:4860::8888", 37.4225, -122.084)] 
        [TestCase("2001:4860:4860::8844", 37.4225,-122.084)]
        public async Task GetTest(string testAddress, double latitude, double longitude)
        {

            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync($"/IpAddressToLocation?ipAddress={testAddress}");

            Assert.NotNull(response);

            var location = JsonConvert.DeserializeObject<Location>(await response.Content.ReadAsStringAsync());

            Assert.NotNull(location);
            Assert.That(location!.ip, Is.EqualTo(testAddress));
            Assert.That(location.latitude, Is.EqualTo(latitude));
            Assert.That(location.longitude, Is.EqualTo (longitude));
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("256.256.256.256")]
        public async Task GetTest(string testAddress)
        {

            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync($"/IpAddressToLocation?ipAddress={testAddress}");
            
            Assert.NotNull(response);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest) );

        }
    }
}
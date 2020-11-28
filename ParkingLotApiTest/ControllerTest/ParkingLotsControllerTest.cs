using System.Threading.Tasks;
using ParkingLotApi;
using Xunit;

namespace ParkingLotApiTest.ControllerTest
{
    public class ParkingLotsControllerTest : TestBase
    {
        public ParkingLotsControllerTest(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task Should_get_hello_world()
        {
            var client = GetClient();

            var allCompaniesResponse = await client.GetAsync("/Hello");
            var responseBody = await allCompaniesResponse.Content.ReadAsStringAsync();

            Assert.Equal("Hello World", responseBody);
        }
    }
}
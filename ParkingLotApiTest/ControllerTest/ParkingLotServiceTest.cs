using System.Threading.Tasks;
using ParkingLotApi;
using Xunit;
using ParkingLotApi.Entity;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Net.Mime;
using System.Net;
using ParkingLotApi.Repository;
using ParkingLotApi.Service;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ParkingLotApi.Dto;

namespace ParkingLotApiTest.ControllerTest
{
    [Collection("ParkingLotTest")]
    public class ParkingLotServiceTest : TestBase
    {
        public ParkingLotServiceTest(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        public ParkingLotDto GenerateParkingLotDtoInstance()
        {
            return new ParkingLotDto() { Name = "uniqueName", Capacity = 1, Location = "BEIJING" };
        }

        [Fact]
        public async Task Should_Add_ParkingLot_Successfully()
        {
            //given
            var parkingLot = GenerateParkingLotDtoInstance();
            var dbContext = GetContext();
            var parkingLotService = new ParkingLotService(dbContext);

            //when
            await parkingLotService.AddParkingLotAsync(parkingLot);

            //then
            Assert.Equal(1, dbContext.ParkingLots.Count());

            var actualParkingLot = await dbContext.ParkingLots.FirstOrDefaultAsync(lot => lot.Name == parkingLot.Name);
            Assert.Equal(parkingLot, new ParkingLotDto(actualParkingLot));
        }
    }
}
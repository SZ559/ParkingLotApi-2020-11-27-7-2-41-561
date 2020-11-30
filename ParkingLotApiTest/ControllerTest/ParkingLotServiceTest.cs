using System.Threading.Tasks;
using ParkingLotApi;
using Xunit;
using ParkingLotApi.Service;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ParkingLotApi.Dto;
using System.Collections.Generic;

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

        [Fact]
        public async Task Should_Delete_ParkingLot_By_Id_Successfully()
        {
            //given
            var parkingLot = GenerateParkingLotDtoInstance();
            var dbContext = GetContext();
            var parkingLotService = new ParkingLotService(dbContext);
            var name = await parkingLotService.AddParkingLotAsync(parkingLot);

            //when
            await parkingLotService.DeleteParkingLotByIdAsync(name);

            //then
            Assert.Equal(0, dbContext.ParkingLots.Count());
        }

        [Fact]
        public async Task Should_Not_Delete_ParkingLot_By_Name_Given_Name_Not_Existed()
        {
            //given
            var parkingLot = GenerateParkingLotDtoInstance();
            var dbContext = GetContext();
            var parkingLotService = new ParkingLotService(dbContext);
            await parkingLotService.AddParkingLotAsync(parkingLot);

            //when
            await parkingLotService.DeleteParkingLotByIdAsync("notexisted");

            //then
            Assert.Equal(1, dbContext.ParkingLots.Count());
        }

        [Fact]
        public async Task Should_get_parkinglots_with_15_on_each_page_successfully()
        {
            //given
            var parkingLot = GenerateParkingLotDtoInstance();
            var dbContext = GetContext();
            var parkingLotService = new ParkingLotService(dbContext);
            await parkingLotService.AddParkingLotAsync(parkingLot);

            //when
            var acatualParkingLots = await parkingLotService.GetParkingLotByPageIndexAsync(2);

            //then
            Assert.Equal(new List<string>(), acatualParkingLots);
        }

        [Fact]
        public async Task Should_updated_parkingLot_successfully()
        {
            //given
            var parkingLot = GenerateParkingLotDtoInstance();
            var dbContext = GetContext();
            var parkingLotService = new ParkingLotService(dbContext);
            var name = await parkingLotService.AddParkingLotAsync(parkingLot);

            //when
            var updatedCapacity = new CapacityDto() { Capacity = 2 };
            var acatualParkingLots = await parkingLotService.UpdateCapacityAsync(name, updatedCapacity);

            //then
            Assert.Equal(updatedCapacity.Capacity.Value, acatualParkingLots.Capacity.Value);

            var actualParkingLot = await dbContext.ParkingLots.FirstOrDefaultAsync(lot => lot.Name == name);
            Assert.Equal(updatedCapacity.Capacity.Value, actualParkingLot.Capacity);
        }
    }
}
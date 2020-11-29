using System.Threading.Tasks;
using ParkingLotApi;
using Xunit;
using ParkingLotApi.Entity;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Net.Mime;
using System.Net;
using System.Linq;
using ParkingLotApi.Dto;
using System.Collections.Generic;
using System.Linq;

namespace ParkingLotApiTest.ControllerTest
{
    [Collection("ParkingLotTest")]
    public class OrdersControllerTest : TestBase
    {
        public OrdersControllerTest(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        public ParkingLotDto GenerateParkingLotDtoInstance(string name = "uniqueName")
        {
            return new ParkingLotDto() { Name = name, Capacity = 1, Location = "BEIJING" };
        }

        public StringContent SerializeParkingLot<T>(T parkingLot)
        {
            var httpContent = JsonConvert.SerializeObject(parkingLot);
            return new StringContent(httpContent, Encoding.UTF8, MediaTypeNames.Application.Json);
        }

        public async Task<T> DeSerializeResponseAsync<T>(HttpResponseMessage response)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseBody);
        }

        [Fact]
        public async Task Should_Add_Order_Successfully_Given_Space_Avaliable_And_Parking_Lot_Existed()
        {
            //given
            var client = GetClient();
            var parkingLot = GenerateParkingLotDtoInstance();
            var requestBody = SerializeParkingLot(parkingLot);
            var parkingLotPostResponse = await client.PostAsync("/ParkingLots", requestBody);
            var car = new CarDto() { PlateNumber = "N95024" };
            var carRequestBody = SerializeParkingLot(car);
            //when
            var orderPostResponse = await client.PostAsync($"{parkingLotPostResponse.Headers.Location}/Orders", carRequestBody);

            //then
            Assert.Equal(HttpStatusCode.Created, orderPostResponse.StatusCode);

            var responseBody = await DeSerializeResponseAsync<CarDto>(orderPostResponse);
            Assert.Equal(car.PlateNumber, responseBody.PlateNumber);

            var dbContext = GetContext();
            Assert.Equal(1, dbContext.Orders.Count());
        }

        [Fact]
        public async Task Should_Return_Not_Found_Given_Parking_Lot_Not_Existed()
        {
            //given
            var client = GetClient();
            var parkingLot = GenerateParkingLotDtoInstance();
            var requestBody = SerializeParkingLot(parkingLot);
            var parkingLotPostResponse = await client.PostAsync("/ParkingLots", requestBody);
            var car = new CarDto() { PlateNumber = "N95024" };
            var carRequestBody = SerializeParkingLot(car);
            //when
            var orderPostResponse = await client.PostAsync($"{parkingLotPostResponse.Headers.Location}200/Orders", carRequestBody);

            //then
            Assert.Equal(HttpStatusCode.NotFound, orderPostResponse.StatusCode);
        }
    }
}
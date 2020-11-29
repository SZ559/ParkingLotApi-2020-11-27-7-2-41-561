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
            var requestBody = Serialize(parkingLot);
            var parkingLotPostResponse = await client.PostAsync("/ParkingLots", requestBody);
            var car = new CarDto() { PlateNumber = "N95024" };
            var carRequestBody = Serialize(car);
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
            var requestBody = Serialize(parkingLot);
            var parkingLotPostResponse = await client.PostAsync("/ParkingLots", requestBody);
            var car = new CarDto() { PlateNumber = "N95024" };
            var carRequestBody = Serialize(car);
            //when
            var orderPostResponse = await client.PostAsync($"{parkingLotPostResponse.Headers.Location}200/Orders", carRequestBody);

            //then
            Assert.Equal(HttpStatusCode.NotFound, orderPostResponse.StatusCode);
        }

        [Fact]
        public async Task Should_Return_The_Parking_Lot_IS_Full_Given_Parking_Lot_is_Full()
        {
            //given
            var client = GetClient();
            var parkingLot = new ParkingLotDto() { Name = "NAME", Capacity = 0, Location = "BEIJING" };
            var requestBody = Serialize(parkingLot);
            var parkingLotPostResponse = await client.PostAsync("/ParkingLots", requestBody);
            var car = new CarDto() { PlateNumber = "N95024" };
            var carRequestBody = Serialize(car);

            //when
            var orderPostResponse = await client.PostAsync($"{parkingLotPostResponse.Headers.Location}/Orders", carRequestBody);

            //then
            Assert.Equal(HttpStatusCode.BadRequest, orderPostResponse.StatusCode);
            var response = await orderPostResponse.Content.ReadAsStringAsync();
            Assert.Equal("The parking lot is full", response);
        }

        [Fact]
        public async Task Should_Close_Order_Successfully_Given_Correct_Order()
        {
            //given
            var client = GetClient();
            var parkingLot = GenerateParkingLotDtoInstance();
            var requestBody = Serialize(parkingLot);
            var parkingLotPostResponse = await client.PostAsync("/ParkingLots", requestBody);
            var car = new CarDto() { PlateNumber = "N95024" };
            var carRequestBody = Serialize(car);
            var orderPostResponse = await client.PostAsync($"{parkingLotPostResponse.Headers.Location}/Orders", carRequestBody);
            var uri = orderPostResponse.Headers.Location;
            var order = await client.GetAsync(uri);
            var response = await order.Content.ReadAsStringAsync();
            var orderInDto = await DeSerializeResponseAsync<OrderDto>(order);
            var orderToPatch = Serialize(orderInDto);

            //when
            var patchResponse = await client.PatchAsync(uri, orderToPatch);

            //then
            Assert.Equal(HttpStatusCode.NoContent, patchResponse.StatusCode);
        }

        [Fact]
        public async Task Should_Return_Bad_Request_Given_Closed_Order()
        {
            //given
            var client = GetClient();
            var parkingLot = GenerateParkingLotDtoInstance();
            var requestBody = Serialize(parkingLot);
            var parkingLotPostResponse = await client.PostAsync("/ParkingLots", requestBody);
            var car = new CarDto() { PlateNumber = "N95024" };
            var carRequestBody = Serialize(car);
            var orderPostResponse = await client.PostAsync($"{parkingLotPostResponse.Headers.Location}/Orders", carRequestBody);
            var uri = orderPostResponse.Headers.Location;
            var order = await client.GetAsync(uri);
            var response = await order.Content.ReadAsStringAsync();
            var orderInDto = await DeSerializeResponseAsync<OrderDto>(order);
            var orderToPatch = Serialize(orderInDto);
            await client.PatchAsync(uri, orderToPatch);

            //when
            var patchResponse = await client.PatchAsync(uri, orderToPatch);

            //then
            Assert.Equal(HttpStatusCode.BadRequest, patchResponse.StatusCode);
        }

        [Fact]
        public async Task Should_Return_Bad_Request_Given_NotExisted_Order()
        {
            //given
            var client = GetClient();
            var parkingLot = GenerateParkingLotDtoInstance();
            var requestBody = Serialize(parkingLot);
            var parkingLotPostResponse = await client.PostAsync("/ParkingLots", requestBody);
            var car = new CarDto() { PlateNumber = "N95024" };
            var carRequestBody = Serialize(car);
            var orderPostResponse = await client.PostAsync($"{parkingLotPostResponse.Headers.Location}/Orders", carRequestBody);
            var uri = orderPostResponse.Headers.Location;
            var orderInDto = new OrderDto() { OrderNumber = 1 };
            var orderToPatch = Serialize(orderInDto);
            await client.PatchAsync(uri, orderToPatch);

            //when
            var patchResponse = await client.PatchAsync(uri, orderToPatch);

            //then
            Assert.Equal(HttpStatusCode.BadRequest, patchResponse.StatusCode);
        }
    }
}
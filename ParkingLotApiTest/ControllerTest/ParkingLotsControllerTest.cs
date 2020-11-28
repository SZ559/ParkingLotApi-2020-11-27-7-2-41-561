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
    public class ParkingLotsControllerTest : TestBase
    {
        public ParkingLotsControllerTest(CustomWebApplicationFactory<Startup> factory) : base(factory)
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
        public async Task Should_Add_ParkingLot_Successfully_Given_Not_Null_Fields_And_Unique_Name()
        {
            //given
            var client = GetClient();
            var parkingLot = GenerateParkingLotDtoInstance();
            var requestBody = SerializeParkingLot(parkingLot);

            //when
            var postResponse = await client.PostAsync("/ParkingLots", requestBody);

            //then
            Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

            var responseBody = await DeSerializeResponseAsync<ParkingLotDto>(postResponse);
            Assert.Equal(parkingLot, responseBody);

            var dbContext = GetContext();
            Assert.Equal(1, dbContext.ParkingLots.Count());
        }

        [Fact]
        public async Task Should_Not_Add_ParkingLot_And_Return_Bad_Request_With_Error_Message_Given_Null_Feild()
        {
            //given
            var client = GetClient();
            var parkingLot = new ParkingLotDto() { Name = null, Capacity = null, Location = null };
            var requestBody = SerializeParkingLot(parkingLot);

            //when
            var postResponse = await client.PostAsync("/ParkingLots", requestBody);

            //then
            Assert.Equal(HttpStatusCode.BadRequest, postResponse.StatusCode);

            var errorMessage = await postResponse.Content.ReadAsStringAsync();
            Assert.Equal("Name cannot be null. Capacity cannot be null. Location cannot be null.", errorMessage);

            var dbContext = GetContext();
            Assert.Equal(0, dbContext.ParkingLots.Count());
        }

        [Fact]
        public async Task Should_Not_Add_ParkingLot_Given_Existed_Name()
        {
            //given
            var client = GetClient();
            var parkingLot = GenerateParkingLotDtoInstance();
            var requestBody = SerializeParkingLot(parkingLot);
            await client.PostAsync("/ParkingLots", requestBody);

            //when
            var postResponse = await client.PostAsync("/ParkingLots", requestBody);

            //then
            Assert.Equal(HttpStatusCode.Conflict, postResponse.StatusCode);

            var responseBody = await postResponse.Content.ReadAsStringAsync();
            Assert.Equal("Name already exists.", responseBody);

            var dbContext = GetContext();
            Assert.Equal(1, dbContext.ParkingLots.Count());
        }

        [Fact]
        public async Task Should_Delete_ParkingLot_Given_Existed_Id()
        {
            //given
            var client = GetClient();
            var parkingLot = GenerateParkingLotDtoInstance();
            var requestBody = SerializeParkingLot(parkingLot);
            var postResponse = await client.PostAsync("/ParkingLots", requestBody);

            //when
            var deleteResponse = await client.DeleteAsync(postResponse.Headers.Location);

            //then
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            var dbContext = GetContext();
            Assert.Equal(0, dbContext.ParkingLots.Count());
        }

        [Fact]
        public async Task Should_Return_Not_Found_Given_Not_Existed_Id()
        {
            //given
            var client = GetClient();
            var parkingLot = GenerateParkingLotDtoInstance();
            var requestBody = SerializeParkingLot(parkingLot);
            await client.PostAsync("/ParkingLots", requestBody);

            //when
            var deleteResponse = await client.DeleteAsync($"/ParkingLots/100");

            //then
            Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);

            var dbContext = GetContext();
            Assert.Equal(1, dbContext.ParkingLots.Count());
        }

        [Fact]
        public async Task Should_Return_ParingLots_with_15_per_page_Given_Page_Index()
        {
            //given
            var client = GetClient();
            var count = 0;
            var expectedParkingLots = new List<ParkingLotDto>();
            while (count < 15)
            {
                var parkingLot = GenerateParkingLotDtoInstance(count.ToString());
                expectedParkingLots.Add(parkingLot);
                var requestBody = SerializeParkingLot(parkingLot);
                await client.PostAsync("/ParkingLots", requestBody);
                count++;
            }

            while (count < 20)
            {
                var parkingLot = GenerateParkingLotDtoInstance(count.ToString());
                var requestBody = SerializeParkingLot(parkingLot);
                await client.PostAsync("/ParkingLots", requestBody);
                count++;
            }

            //when
            var getResponse = await client.GetAsync($"/ParkingLots?pageIndex=1");

            //then
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var actualParkingLots = await DeSerializeResponseAsync<IList<ParkingLotDto>>(getResponse);
            Assert.Equal(expectedParkingLots, actualParkingLots);
        }

        [Fact]
        public async Task Should_Return_ParingLot_Given_Id()
        {
            //given
            var client = GetClient();
            var parkingLot = GenerateParkingLotDtoInstance();
            var requestBody = SerializeParkingLot(parkingLot);
            var postResponse = await client.PostAsync("/ParkingLots", requestBody);

            //when
            var getResponse = await client.GetAsync(postResponse.Headers.Location);

            //then
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var actualParkingLots = await DeSerializeResponseAsync<ParkingLotDto>(getResponse);
            Assert.Equal(parkingLot, actualParkingLots);
        }

        [Fact]
        public async Task Should_Return_NotFound_Given_Not_Existed_Id()
        {
            //given
            var client = GetClient();
            var parkingLot = GenerateParkingLotDtoInstance();
            var requestBody = SerializeParkingLot(parkingLot);
            var postResponse = await client.PostAsync("/ParkingLots", requestBody);

            //when
            var getResponse = await client.GetAsync("/ParkingLots/100");

            //then
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }
    }
}
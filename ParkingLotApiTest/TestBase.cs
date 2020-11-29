using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using ParkingLotApi;
using ParkingLotApi.Dto;
using ParkingLotApi.Repository;
using Xunit;

namespace ParkingLotApiTest
{
    public class TestBase : IClassFixture<CustomWebApplicationFactory<Startup>>, IDisposable
    {
        public TestBase(CustomWebApplicationFactory<Startup> factory)
        {
            this.Factory = factory;
        }

        protected CustomWebApplicationFactory<Startup> Factory { get; }

        public void Dispose()
        {
            var scope = Factory.Services.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var context = scopedServices.GetRequiredService<ParkingLotContext>();

            context.ParkingLots.RemoveRange(context.ParkingLots);
            context.SaveChanges();
        }

        protected HttpClient GetClient()
        {
            return Factory.CreateClient();
        }

        protected ParkingLotContext GetContext()
        {
            var scope = Factory.Services.CreateScope();
            var scopedServices = scope.ServiceProvider;

            return scopedServices.GetRequiredService<ParkingLotContext>();
        }

        protected ParkingLotDto GenerateParkingLotDtoInstance(string name = "uniqueName")
        {
            return new ParkingLotDto() { Name = name, Capacity = 1, Location = "BEIJING" };
        }

        protected StringContent Serialize<T>(T parkingLot)
        {
            var httpContent = JsonConvert.SerializeObject(parkingLot);
            return new StringContent(httpContent, Encoding.UTF8, MediaTypeNames.Application.Json);
        }

        protected async Task<T> DeSerializeResponseAsync<T>(HttpResponseMessage response)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseBody);
        }
    }
}
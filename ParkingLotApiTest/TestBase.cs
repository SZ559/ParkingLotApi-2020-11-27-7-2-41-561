using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using ParkingLotApi;
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
    }
}
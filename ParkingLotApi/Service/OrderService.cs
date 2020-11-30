using Microsoft.EntityFrameworkCore;
using ParkingLotApi.Dto;
using ParkingLotApi.Entity;
using ParkingLotApi.Repository;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingLotApi.Service
{
    public class OrderService : IOrderService
    {
        private readonly ParkingLotContext parkingLotDbContext;
        private readonly DbSet<OrderEntity> orders;
        public OrderService(ParkingLotContext parkingLotDbContext)
        {
            this.parkingLotDbContext = parkingLotDbContext;
            orders = parkingLotDbContext.Orders;
        }

        public async Task<OrderDto> CreateOrderAsync(string parkingLotName, string plateNumber)
        {
            var order = new OrderEntity()
            {
                PlateNumber = plateNumber,
                ParkingLotName = parkingLotName,
            };
            orders.Add(order);
            await parkingLotDbContext.SaveChangesAsync();
            return new OrderDto(order);
        }

        public async Task<OrderDto> GetOrderAsync(int orderNumber)
        {
            var order = orders.FirstOrDefault(order => order.OrderNumber == orderNumber);
            return order == null ? null : new OrderDto(order);
        }

        public async Task<bool> IsOrderOpenedAsync(int orderNumber)
        {
            var orderInMemory = orders.FirstOrDefault(order => order.OrderNumber == orderNumber);
            return orderInMemory != null && orderInMemory.OrderStatus == OrderStatus.Open;
        }

        public async Task CloseOrderAsync(int orderNumber)
        {
            var orderInMemory = orders.FirstOrDefault(order => order.OrderNumber == orderNumber);
            orderInMemory.OrderStatus = OrderStatus.Close;
            orderInMemory.CloseTime = DateTime.Now;
            parkingLotDbContext.UpdateRange(orderInMemory);
            await parkingLotDbContext.SaveChangesAsync();
        }

        public async Task<int> GetOrderOpenedInParkingLotAsync(string name)
        {
            return orders.Where(order => order.ParkingLotName == name).Count();
        }
    }
}

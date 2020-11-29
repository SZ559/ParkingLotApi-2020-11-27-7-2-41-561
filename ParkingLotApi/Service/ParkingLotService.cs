using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ParkingLotApi.Entity;
using ParkingLotApi.Repository;
using ParkingLotApi.Dto;

namespace ParkingLotApi.Service
{
    public class ParkingLotService : IParkingLotSerive
    {
        private readonly ParkingLotContext parkingLotDbContext;

        public ParkingLotService(ParkingLotContext parkingLotDbContext)
        {
            this.parkingLotDbContext = parkingLotDbContext;
        }

        public async Task<int> AddParkingLotAsync(ParkingLotDto parkingLotDto)
        {
            var parkingLotEntity = new ParkingLotEntity()
            {
                Name = parkingLotDto.Name,
                Capacity = parkingLotDto.Capacity.HasValue ? parkingLotDto.Capacity.Value : 0,
                Location = parkingLotDto.Location,
            };
            await parkingLotDbContext.AddAsync(parkingLotEntity);
            await parkingLotDbContext.SaveChangesAsync();
            return parkingLotEntity.Id;
        }

        public async Task<bool> DeleteParkingLotByIdAsync(int id)
        {
            var parkingLot = await GetParkingLotEntityByIdAsync(id);
            if (parkingLot == null)
            {
                return false;
            }

            parkingLotDbContext.ParkingLots.Remove(parkingLot);
            await parkingLotDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ContainsNameAsync(string name)
        {
            return parkingLotDbContext.ParkingLots.Any(lot => lot.Name == name);
        }

        public async Task<IList<ParkingLotDto>> GetParkingLotByPageIndexAsync(int pageIndex)
        {
            const int parkingLotsperPage = 15;
            return parkingLotDbContext.ParkingLots
                .Skip(parkingLotsperPage * (pageIndex - 1))
                .Take(parkingLotsperPage)
                .Select(paringLot => new ParkingLotDto(paringLot))
                .ToList();
        }

        public async Task<ParkingLotDto> UpdateCapacityAsync(int id, CapacityDto updatedCapacity)
        {
            var parkingLot = await GetParkingLotEntityByIdAsync(id);
            parkingLot.Capacity = updatedCapacity.Capacity.Value;
            parkingLotDbContext.ParkingLots.Update(parkingLot);
            return new ParkingLotDto(parkingLot);
        }

        public async Task<ParkingLotDto> GetParkingLotByIdAsync(int id)
        {
            var parkingLot = parkingLotDbContext.ParkingLots.FirstOrDefault(parkingLot => parkingLot.Id == id);

            return parkingLot == null ? null : new ParkingLotDto(parkingLot);
        }

        public async Task<bool> IsParkingLotFull(int id)
        {
            var parkingLot = await GetParkingLotEntityByIdAsync(id);
            return parkingLot.Capacity <= parkingLot.Orders.Where(lot => lot.OrderStatus == OrderStatus.Open).Count();
        }

        public async Task<bool> IsParkingLotExistedAsync(int id)
        {
            return parkingLotDbContext.ParkingLots.FirstOrDefault(parkingLot => parkingLot.Id == id) != null;
        }

        public async Task<OrderDto> CreateOrderAsync(int id, string plateNumber)
        {
            var parkingLot = await GetParkingLotEntityByIdAsync(id);
            var order = new OrderEntity()
            {
                PlateNumber = plateNumber,
                ParkingLotName = parkingLot.Name,
            };
            parkingLot.Orders.Add(order);
            parkingLotDbContext.UpdateRange(parkingLot);
            await parkingLotDbContext.SaveChangesAsync();
            return new OrderDto(order);
        }

        public async Task<OrderDto> GetOrderAsync(int id, int orderNumber)
        {
            var order = parkingLotDbContext.Orders.FirstOrDefault(order => order.OrderNumber == orderNumber);
            return order == null ? null : new OrderDto(order);
        }

        public async Task<bool> IsOrderOpenedAsync(int orderNumber)
        {
            var orderInMemory = parkingLotDbContext.Orders.FirstOrDefault(order => order.OrderNumber == orderNumber);
            return orderInMemory != null && orderInMemory.OrderStatus == OrderStatus.Open;
        }

        public async Task CloseOrderAsync(int orderNumber)
        {
            var orderInMemory = parkingLotDbContext.Orders.FirstOrDefault(order => order.OrderNumber == orderNumber);
            orderInMemory.OrderStatus = OrderStatus.Close;
            orderInMemory.CloseTime = DateTime.Now;
            parkingLotDbContext.UpdateRange(orderInMemory);
            await parkingLotDbContext.SaveChangesAsync();
        }

        private async Task<ParkingLotEntity> GetParkingLotEntityByIdAsync(int id)
        {
            return parkingLotDbContext.ParkingLots.FirstOrDefault(parkingLot => parkingLot.Id == id);
        }
    }
}

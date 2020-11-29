using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ParkingLotApi.Entity;
using ParkingLotApi.Repository;
using ParkingLotApi.Dto;

namespace ParkingLotApi.Service
{
    public interface IParkingLotSerive
    {
        Task<int> AddParkingLotAsync(ParkingLotDto parkingLotDto);
        Task<ParkingLotDto> GetParkingLotById(int id);
        Task<bool> ContainsName(string name);
        Task<bool> DeleteParkingLotById(int id);
        Task<IList<ParkingLotDto>> GetParkingLotByPageIndex(int pageIndex);
        Task<ParkingLotDto> UpdateCapacity(int id, CapacityDto updatedCapacity);
        Task<OrderDto> CreateOrder(int id, string plateNumber);
        Task<bool> IsParkingLotExisted(int id);
        Task<OrderDto> GetOrder(int id, int orderNumber);
        Task<bool> IsParkingLotFull(int id);
        Task<bool> IsOrderOpened(int orderNumber);
        Task CloseOrder(int orderNumber);
    }

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

        public async Task<bool> DeleteParkingLotById(int id)
        {
            var parkingLot = await GetParkingLotEntityById(id);
            if (parkingLot == null)
            {
                return false;
            }

            parkingLotDbContext.ParkingLots.Remove(parkingLot);
            await parkingLotDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ContainsName(string name)
        {
            return parkingLotDbContext.ParkingLots.Any(lot => lot.Name == name);
        }

        public async Task<IList<ParkingLotDto>> GetParkingLotByPageIndex(int pageIndex)
        {
            const int parkingLotsperPage = 15;
            return parkingLotDbContext.ParkingLots
                .Skip(parkingLotsperPage * (pageIndex - 1))
                .Take(parkingLotsperPage)
                .Select(paringLot => new ParkingLotDto(paringLot))
                .ToList();
        }

        public async Task<ParkingLotDto> UpdateCapacity(int id, CapacityDto updatedCapacity)
        {
            var parkingLot = await GetParkingLotEntityById(id);
            if (parkingLot == null)
            {
                return null;
            }

            parkingLot.Capacity = updatedCapacity.Capacity.Value;

            parkingLotDbContext.ParkingLots.Update(parkingLot);
            return new ParkingLotDto(parkingLot);
        }

        public async Task<ParkingLotDto> GetParkingLotById(int id)
        {
            var parkingLot = parkingLotDbContext.ParkingLots.FirstOrDefault(parkingLot => parkingLot.Id == id);

            return parkingLot == null ? null : new ParkingLotDto(parkingLot);
        }

        public async Task<bool> IsParkingLotFull(int id)
        {
            var parkingLot = await GetParkingLotEntityById(id);
            return parkingLot.Capacity <= parkingLot.Orders.Where(lot => lot.OrderStatus == OrderStatus.Open).Count();
        }

        public async Task<bool> IsParkingLotExisted(int id)
        {
            return parkingLotDbContext.ParkingLots.FirstOrDefault(parkingLot => parkingLot.Id == id) != null;
        }

        public async Task<OrderDto> CreateOrder(int id, string plateNumber)
        {
            var parkingLot = await GetParkingLotEntityById(id);
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

        public async Task<OrderDto> GetOrder(int id, int orderNumber)
        {
            var order = parkingLotDbContext.Orders.FirstOrDefault(order => order.OrderNumber == orderNumber);
            return order == null ? null : new OrderDto(order);
        }

        public async Task<bool> IsOrderOpened(int orderNumber)
        {
            var orderInMemory = parkingLotDbContext.Orders.FirstOrDefault(order => order.OrderNumber == orderNumber);
            return orderInMemory != null && orderInMemory.OrderStatus == OrderStatus.Open;
        }

        public async Task CloseOrder(int orderNumber)
        {
            var orderInMemory = parkingLotDbContext.Orders.FirstOrDefault(order => order.OrderNumber == orderNumber);
            orderInMemory.OrderStatus = OrderStatus.Close;
            orderInMemory.CloseTime = DateTime.Now;
            parkingLotDbContext.UpdateRange(orderInMemory);
            await parkingLotDbContext.SaveChangesAsync();
        }

        private async Task<ParkingLotEntity> GetParkingLotEntityById(int id)
        {
            return parkingLotDbContext.ParkingLots.FirstOrDefault(parkingLot => parkingLot.Id == id);
        }
    }
}

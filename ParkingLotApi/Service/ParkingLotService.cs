using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ParkingLotApi.Entity;
using ParkingLotApi.Repository;
using ParkingLotApi.Dto;
using Microsoft.EntityFrameworkCore;

namespace ParkingLotApi.Service
{
    public class ParkingLotService : IParkingLotSerive
    {
        private readonly ParkingLotContext parkingLotDbContext;

        public ParkingLotService(ParkingLotContext parkingLotDbContext)
        {
            this.parkingLotDbContext = parkingLotDbContext;
        }

        public async Task<ParkingLotDto> GetParkingLotByNameAsync(string name)
        {
            var parkingLot = parkingLotDbContext.ParkingLots.FirstOrDefault(parkingLot => parkingLot.Name == name);

            return parkingLot == null ? null : new ParkingLotDto(parkingLot);
        }

        public async Task<string> AddParkingLotAsync(ParkingLotDto parkingLotDto)
        {
            var parkingLotEntity = new ParkingLotEntity()
            {
                Name = parkingLotDto.Name,
                Capacity = parkingLotDto.Capacity.HasValue ? parkingLotDto.Capacity.Value : 0,
                Location = parkingLotDto.Location,
            };
            await parkingLotDbContext.AddAsync(parkingLotEntity);
            await parkingLotDbContext.SaveChangesAsync();
            return parkingLotEntity.Name;
        }

        public async Task<bool> DeleteParkingLotByIdAsync(string name)
        {
            var parkingLot = await GetParkingLotEntityByIdAsync(name);
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

        public async Task<IList<string>> GetParkingLotByPageIndexAsync(int pageIndex)
        {
            const int parkingLotsperPage = 15;
            return parkingLotDbContext.ParkingLots
                .Skip(parkingLotsperPage * (pageIndex - 1))
                .Take(parkingLotsperPage)
                .Select(paringLot => paringLot.Name)
                .ToList();
        }

        public async Task<ParkingLotDto> UpdateCapacityAsync(string name, CapacityDto updatedCapacity)
        {
            var parkingLot = await GetParkingLotEntityByIdAsync(name);
            parkingLot.Capacity = updatedCapacity.Capacity.Value;
            parkingLotDbContext.ParkingLots.Update(parkingLot);
            return new ParkingLotDto(parkingLot);
        }

        public async Task<uint?> GetParkingLotCapacity(string name)
        {
            var parkingLot = await GetParkingLotEntityByIdAsync(name);
            return parkingLot?.Capacity;
        }

        public async Task<bool> IsParkingLotExistedAsync(string name)
        {
            return parkingLotDbContext.ParkingLots.FirstOrDefault(parkingLot => parkingLot.Name == name) != null;
        }

        private async Task<ParkingLotEntity> GetParkingLotEntityByIdAsync(string name)
        {
            return parkingLotDbContext.ParkingLots.FirstOrDefault(parkingLot => parkingLot.Name == name);
        }
    }
}

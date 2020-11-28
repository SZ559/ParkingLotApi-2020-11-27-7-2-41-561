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
        Task<bool> DeleteParkingLotByName(string name);
        Task<IList<ParkingLotDto>> GetParkingLotByPageIndex(int pageIndex);
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

        public async Task<bool> DeleteParkingLotByName(string name)
        {
            var parkingLot = await GetParkingLotByName(name);
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

        public async Task<ParkingLotEntity> GetParkingLotByName(string name)
        {
            return parkingLotDbContext.ParkingLots.FirstOrDefault(lot => lot.Name == name);
        }

        public async Task<IList<ParkingLotDto>> GetParkingLotByPageIndex(int pageIndex)
        {
            const int parkingLotsperPage = 15;
            var a = parkingLotDbContext.ParkingLots.Skip(1);

            return parkingLotDbContext.ParkingLots
                .Skip(parkingLotsperPage * (pageIndex - 1))
                .Take(parkingLotsperPage)
                .Select(paringLot => new ParkingLotDto(paringLot))
                .ToList();
        }

        public async Task<ParkingLotDto> GetParkingLotById(int id)
        {
            var parkingLot = parkingLotDbContext.ParkingLots.FirstOrDefault(parkingLot => parkingLot.Id == id);

            return parkingLot == null ? null : new ParkingLotDto(parkingLot);
        }
    }
}

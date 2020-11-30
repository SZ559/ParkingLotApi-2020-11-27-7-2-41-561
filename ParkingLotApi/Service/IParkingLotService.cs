using ParkingLotApi.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingLotApi.Service
{
    public interface IParkingLotSerive
    {
        Task<string> AddParkingLotAsync(ParkingLotDto parkingLotDto);
        Task<ParkingLotDto> GetParkingLotByNameAsync(string name);
        Task<bool> ContainsNameAsync(string name);
        Task<bool> DeleteParkingLotByIdAsync(string name);
        Task<IList<string>> GetParkingLotByPageIndexAsync(int pageIndex);
        Task<ParkingLotDto> UpdateCapacityAsync(string name, CapacityDto updatedCapacity);
        Task<bool> IsParkingLotExistedAsync(string name);
        Task<uint?> GetParkingLotCapacity(string name);
    }
}

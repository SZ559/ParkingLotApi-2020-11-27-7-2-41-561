using ParkingLotApi.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingLotApi.Service
{
    public interface IParkingLotSerive
    {
        Task<int> AddParkingLotAsync(ParkingLotDto parkingLotDto);
        Task<ParkingLotDto> GetParkingLotByIdAsync(int id);
        Task<bool> ContainsNameAsync(string name);
        Task<bool> DeleteParkingLotByIdAsync(int id);
        Task<IList<ParkingLotDto>> GetParkingLotByPageIndexAsync(int pageIndex);
        Task<ParkingLotDto> UpdateCapacityAsync(int id, CapacityDto updatedCapacity);
        Task<OrderDto> CreateOrderAsync(int id, string plateNumber);
        Task<bool> IsParkingLotExistedAsync(int id);
        Task<OrderDto> GetOrderAsync(int id, int orderNumber);
        Task<bool> IsParkingLotFull(int id);
        Task<bool> IsOrderOpenedAsync(int orderNumber);
        Task CloseOrderAsync(int orderNumber);
    }
}

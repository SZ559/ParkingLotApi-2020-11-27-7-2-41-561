using ParkingLotApi.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingLotApi.Service
{
    public interface IOrderService
    {
        Task<OrderDto> GetOrderAsync(int orderNumber);
        Task<OrderDto> CreateOrderAsync(string parkingLotName, string plateNumber);
        Task<bool> IsOrderOpenedAsync(int orderNumber);
        Task CloseOrderAsync(int orderNumber);
        Task<int> GetOrderOpenedInParkingLotAsync(string name);
    }
}

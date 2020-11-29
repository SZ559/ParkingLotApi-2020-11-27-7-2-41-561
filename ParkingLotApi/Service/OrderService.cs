using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ParkingLotApi.Entity;
using ParkingLotApi.Repository;
using ParkingLotApi.Dto;

namespace ParkingLotApi.Service
{
    public interface IOrderService
    {
    }

    public class OrderService : IOrderService
    {
        private readonly ParkingLotContext parkingLotDbContext;

        public OrderService(ParkingLotContext parkingLotDbContext)
        {
            this.parkingLotDbContext = parkingLotDbContext;
        }
    }
}

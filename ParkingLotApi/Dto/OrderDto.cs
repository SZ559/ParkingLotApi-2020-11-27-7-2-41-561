using System;
using ParkingLotApi.Entity;
namespace ParkingLotApi.Dto
{
    public class OrderDto
    {
        public OrderDto()
        {
        }

        public OrderDto(OrderEntity order)
        {
            OrderNumber = order.OrderNumber;
            ParkingLotName = order.ParkingLotName;
            PlateNumber = order.PlateNumber;
            CreationTime = order.CreationTime;
        }

        public int? OrderNumber { get; set; }
        public string ParkingLotName { get; set; }
        public string PlateNumber { get; set; }
        public DateTime? CreationTime { get; set; }

        public bool IsValid()
        {
            return OrderNumber != null && !string.IsNullOrEmpty(ParkingLotName) && !string.IsNullOrEmpty(PlateNumber) && CreationTime != null;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }

            var order = (OrderDto)obj;

            return OrderNumber == order.OrderNumber && ParkingLotName == order.ParkingLotName && PlateNumber == order.PlateNumber && CreationTime == order.CreationTime;
        }
    }
}

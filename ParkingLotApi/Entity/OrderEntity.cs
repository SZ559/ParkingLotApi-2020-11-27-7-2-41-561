using System;
using System.ComponentModel.DataAnnotations;

namespace ParkingLotApi.Entity
{
    public enum OrderStatus
    {
        Open,
        Close,
    }

    public class OrderEntity
    {
        public OrderEntity()
        {
        }

        [Key]
        public int OrderNumber { get; set; }
        public string ParkingLotName { get; set; }
        public string PlateNumber { get; set; }
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public DateTime CloseTime { get; set; }
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Open;      
    }
}

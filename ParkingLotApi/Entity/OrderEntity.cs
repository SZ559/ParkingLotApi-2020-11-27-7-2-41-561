using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParkingLotApi.Entity
{
    public enum OrderStatus
    {
        Open,
        Close,
    }

    public class OrderEntity
    {
        [Key]
        public int OrderNumber { get; set; }
        public string ParkingLotName { get; set; }
        public string PlateNumber { get; set; }
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public DateTime CloseTime { get; set; }
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Open;      
    }
}

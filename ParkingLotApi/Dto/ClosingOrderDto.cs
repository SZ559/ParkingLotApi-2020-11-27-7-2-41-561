using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ParkingLotApi.Entity;
namespace ParkingLotApi.Dto
{
    public class ClosingOrderDto
    {
        public int? OrderNumber { get; set; }
        public string ParkingLotName { get; set; }
        public string PlateNumber { get; set; }
        public DateTime CreationTime { get; set; }
    }
}

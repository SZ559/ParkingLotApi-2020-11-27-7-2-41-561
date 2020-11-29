using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParkingLotApi.Dto
{
    public class OrderDto
    {
        public int OrderNumber { get; set; }
        public string ParkingLotName { get; set; }
        public long PlateNumber { get; set; }
        public DateTime CreationTime { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ParkingLotApi.Entity;

namespace ParkingLotApi.Dto
{
    public class CapacityDto
    {
        public uint? Capacity { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingLotApi.Entity
{
    public class ParkingLotDto
    {
        public string Name { get; set; }
        public uint? Capacity { get; set; }
        public string Location { get; set; }
    }
}

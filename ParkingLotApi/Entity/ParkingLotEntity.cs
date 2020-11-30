using System.Collections.Generic;

namespace ParkingLotApi.Entity
{
    public class ParkingLotEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public uint Capacity { get; set; }
        public string Location { get; set; }
    }
}

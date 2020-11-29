using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingLotApi.Entity
{
    public class ParkingLotEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public uint Capacity { get; set; }
        public string Location { get; set; }
        public List<OrderEntity> Orders { get; set; } = new List<OrderEntity>();

        //should parking lot entity includes orders? 
        //yes: cannot create an order without parking lot
        //no: when delete the parking lot, the order should also exist and can be get and view. 
    }
}

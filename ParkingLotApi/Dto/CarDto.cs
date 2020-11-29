using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParkingLotApi.Dto
{
    public class CarDto
    {
        public string PlateNumber { get; set; }
    }
}

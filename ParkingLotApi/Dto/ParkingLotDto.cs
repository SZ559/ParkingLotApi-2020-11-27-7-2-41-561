using ParkingLotApi.Entity;
using System.ComponentModel.DataAnnotations;

namespace ParkingLotApi.Dto
{
    public class ParkingLotDto
    {
        public ParkingLotDto()
        {
        }

        public ParkingLotDto(ParkingLotEntity parkingLotEntity)
        {
            Name = parkingLotEntity.Name;
            Capacity = parkingLotEntity.Capacity;
            Location = parkingLotEntity.Location;
        }

        public string Name { get; set; }
        public uint? Capacity { get; set; }
        public string Location { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }

            var lot = (ParkingLotDto)obj;
            return Name == lot.Name && Capacity == lot.Capacity && Location == lot.Location;
        }

        public bool IsValid(out string errorMessage)
        {
            errorMessage = string.Empty;
            if (this.Name == null)
            {
                errorMessage += "Name cannot be null. ";
            }

            if (this.Capacity == null)
            {
                errorMessage += "Capacity cannot be null. ";
            }

            if (this.Location == null)
            {
                errorMessage += "Location cannot be null. ";
            }

            errorMessage = errorMessage.TrimEnd();
            return errorMessage == string.Empty;
        }
    }
}

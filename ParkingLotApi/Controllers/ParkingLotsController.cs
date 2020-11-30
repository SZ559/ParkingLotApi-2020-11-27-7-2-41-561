using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ParkingLotApi.Service;
using ParkingLotApi.Dto;

namespace ParkingLotApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ParkingLotsController : ControllerBase
    {
        private readonly IParkingLotSerive parkingLotService;
        private readonly IOrderService orderService;
        public ParkingLotsController(IParkingLotSerive parkingLotService, IOrderService orderService)
        {
            this.parkingLotService = parkingLotService;
            this.orderService = orderService;
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<ParkingLotDto>> GetByName(string name)
        {
            var parkingLot = await parkingLotService.GetParkingLotByNameAsync(name);
            if (parkingLot == null)
            {
                return NotFound("The parking lot is not eixsted.");
            }

            return Ok(parkingLot);
        }

        [HttpPost]
        public async Task<ActionResult> AddParkingLotAsync(ParkingLotDto parkingLotDto)
        {
            var errorMessage = string.Empty;
            if (!parkingLotDto.IsValid(out errorMessage))
            {
                return BadRequest(errorMessage);
            }

            if (await parkingLotService.ContainsNameAsync(parkingLotDto.Name))
            {
                return Conflict("Name already exists.");
            }

            var name = await parkingLotService.AddParkingLotAsync(parkingLotDto);

            return CreatedAtAction(nameof(GetByName), new { name = name }, parkingLotDto);
        }

        [HttpDelete("{name}")]
        public async Task<ActionResult<int>> DeleteParkingLotAsync(string name)
        {
            if (!await parkingLotService.IsParkingLotExistedAsync(name))
            {
                return NotFound("The parking lot is not existed.");
            }

            if (await orderService.GetOrderOpenedInParkingLotAsync(name) != 0)
            {
                return BadRequest("The Parking Lot is not empty.");
            }

            await parkingLotService.DeleteParkingLotByIdAsync(name);
            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<IList<ParkingLotDto>>> GetParkingLots([FromQuery] int pageIndex)
        {
            var parkingLots = await parkingLotService.GetParkingLotByPageIndexAsync(pageIndex);
            return Ok(parkingLots);
        }

        [HttpPatch("{name}")]
        public async Task<ActionResult<ParkingLotDto>> UpdateCapacity(string name, CapacityDto updatedCapacity)
        {
            if (updatedCapacity == null || updatedCapacity.Capacity == null)
            {
                return BadRequest("Capacity cannot be null.");
            }

            if (!await parkingLotService.IsParkingLotExistedAsync(name))
            {
                return NotFound("The parking lot is not existed.");
            }

            var updatedParkingLot = await parkingLotService.UpdateCapacityAsync(name, updatedCapacity);
            return Ok(updatedParkingLot);
        }
    }
}

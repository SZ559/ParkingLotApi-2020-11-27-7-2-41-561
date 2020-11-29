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
        public ParkingLotsController(IParkingLotSerive parkingLotService)
        {
            this.parkingLotService = parkingLotService;
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

            var id = await parkingLotService.AddParkingLotAsync(parkingLotDto);

            return CreatedAtAction(nameof(GetById), new { id = id }, parkingLotDto);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<int>> DeleteParkingLotAsync(int id)
        {
            if (!await parkingLotService.IsParkingLotExistedAsync(id))
            {
                return NotFound();
            }

            await parkingLotService.DeleteParkingLotByIdAsync(id);
            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<IList<ParkingLotDto>>> GetParkingLots([FromQuery] int pageIndex)
        {
            var parkingLots = await parkingLotService.GetParkingLotByPageIndexAsync(pageIndex);
            return Ok(parkingLots);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<ParkingLotDto>> UpdateCapacity(int id, CapacityDto updatedCapacity)
        {
            if (updatedCapacity == null || updatedCapacity.Capacity == null)
            {
                return BadRequest("Capacity cannot be null.");
            }

            if (!await parkingLotService.IsParkingLotExistedAsync(id))
            {
                return NotFound();
            }

            var updatedParkingLot = await parkingLotService.UpdateCapacityAsync(id, updatedCapacity);
            return Ok(updatedParkingLot);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ParkingLotDto>> GetById(int id)
        {
            var parkingLot = await parkingLotService.GetParkingLotByIdAsync(id);
            if (parkingLot == null)
            {
                return NotFound();
            }

            return Ok(parkingLot);   
        }
    }
}

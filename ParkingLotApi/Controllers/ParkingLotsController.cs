using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ParkingLotApi.Entity;
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
        public async Task<ActionResult<int>> AddParkingLotAsync(ParkingLotDto parkingLotDto)
        {
            var errorMessage = string.Empty;
            if (!parkingLotDto.IsValid(out errorMessage))
            {
                return BadRequest(errorMessage);
            }

            if (await parkingLotService.ContainsName(parkingLotDto.Name))
            {
                return Conflict("Name already exists.");
            }

            var id = await parkingLotService.AddParkingLotAsync(parkingLotDto);

            return CreatedAtAction(nameof(GetById), new { id = id }, parkingLotDto);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<int>> DeleteParkingLotAsync(int id)
        {
            var isDeleteSuccess = await parkingLotService.DeleteParkingLotById(id);
            if (!isDeleteSuccess)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<IList<ParkingLotDto>>> GetParkingLots([FromQuery] int pageIndex)
        {
            var parkingLots = await parkingLotService.GetParkingLotByPageIndex(pageIndex);
            return Ok(parkingLots);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ParkingLotDto>> GetById(int id)
        {
            var parkingLot = await parkingLotService.GetParkingLotById(id);
            if (parkingLot == null)
            {
                return NotFound();
            }

            return Ok(parkingLot);   
        }
    }
}

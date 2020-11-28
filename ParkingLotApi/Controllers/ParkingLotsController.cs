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

        [HttpDelete("{name}")]
        public async Task<ActionResult<int>> DeleteParkingLotAsync(string name)
        {
            if (!await parkingLotService.ContainsName(name))
            {
                return NotFound();
            }

            await parkingLotService.DeleteParkingLotByName(name);

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ParkingLotDto>> GetById(int id)
        {
            var companyDto = await parkingLotService.GetParkingLotById(id);
            return Ok(companyDto);
        }
    }
}

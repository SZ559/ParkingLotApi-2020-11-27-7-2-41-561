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
    [Route("/ParkingLots/{id}/Orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IParkingLotSerive parkingLotService;
        public OrdersController(IParkingLotSerive parkingLotService)
        {
            this.parkingLotService = parkingLotService;
        }

        [HttpPost]
        public async Task<ActionResult> CreateOrderAsync(int id, CarDto car)
        {
            if (car.PlateNumber == null)
            {
                return BadRequest("Plate Number cannot be null.");
            }

            if (!await parkingLotService.IsParkingLotExisted(id))
            {
                return NotFound("ParkingLot not existed.");
            }

            if (!await parkingLotService.IsParkingLotFull(id))
            {
                return BadRequest();
            }

            var order = await parkingLotService.CreateOrder(id, car.PlateNumber);

            return CreatedAtAction(nameof(GetByOrderNumber), new { id = id, orderNumber = order.OrderNumber }, order);
        }

        [HttpGet("{orderNumber}")]
        public async Task<ActionResult<OrderDto>> GetByOrderNumber(int id, int orderNumber)
        {
            if (!await parkingLotService.IsParkingLotExisted(id))
            {
                return NotFound("ParkingLot not existed.");
            }

            var order = await parkingLotService.GetOrder(id, orderNumber);

            if (order == null)
            {
                return NotFound("Order not existed.");
            }

            return Ok(order);
        }
    }
}

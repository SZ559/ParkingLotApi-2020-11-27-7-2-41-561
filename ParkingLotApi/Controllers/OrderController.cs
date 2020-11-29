using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult<OrderDto>> CreateOrderAsync(int id, CarDto car)
        {
            if (string.IsNullOrEmpty(car.PlateNumber))
            {
                return BadRequest("Plate Number cannot be null.");
            }

            if (!await parkingLotService.IsParkingLotExisted(id))
            {
                return NotFound("ParkingLot not existed.");
            }

            if (await parkingLotService.IsParkingLotFull(id))
            {
                return BadRequest("The parking lot is full");
            }

            var order = await parkingLotService.CreateOrder(id, car.PlateNumber);

            return CreatedAtAction(nameof(GetByOrderNumber), new { id = id, orderNumber = order.OrderNumber }, order);
        }

        [HttpPatch("{orderNumber}")]
        public async Task<ActionResult> UpdateOrderAsync(int id, int orderNumber, OrderDto order)
        {
            if (order == null || !order.IsValid())
            {
                return BadRequest("Order and order information cannot be null.");
            }

            if (!await parkingLotService.IsParkingLotExisted(id))
            {
                return NotFound("ParkingLot not existed.");
            }

            var orderInMemory = await parkingLotService.GetOrder(id, orderNumber);

            if (orderInMemory == null || !orderInMemory.Equals(order))
            {
                return BadRequest("Order not existed.");
            }

            if (!await parkingLotService.IsOrderOpened(orderNumber))
            {
                return BadRequest("Order is used.");
            }

            await parkingLotService.CloseOrder(orderNumber);
            return NoContent();
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

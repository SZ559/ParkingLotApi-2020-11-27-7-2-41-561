using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ParkingLotApi.Service;
using ParkingLotApi.Dto;

namespace ParkingLotApi.Controllers
{
    [ApiController]
    [Route("/Orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IParkingLotSerive parkingLotService;
        private readonly IOrderService orderService;
        public OrdersController(IParkingLotSerive parkingLotService, IOrderService orderService)
        {
            this.parkingLotService = parkingLotService;
            this.orderService = orderService;
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrderAsync(CarOrderDto carOrder)
        {
            if (string.IsNullOrEmpty(carOrder.PlateNumber) || string.IsNullOrEmpty(carOrder.ParkingLotName))
            {
                return BadRequest("Plate Number and Parking Lot Name cannot be null.");
            }

            if (!await parkingLotService.IsParkingLotExistedAsync(carOrder.ParkingLotName))
            {
                return NotFound("ParkingLot not existed.");
            }

            if (await IsParkingLotFullAsync(carOrder.ParkingLotName))
            {
                return BadRequest("The parking lot is full");
            }

            if (await orderService.IsCarExisted(carOrder.PlateNumber))
            {
                return BadRequest("The car already existed.");
            }

            var order = await orderService.CreateOrderAsync(carOrder.ParkingLotName, carOrder.PlateNumber);

            return CreatedAtAction(nameof(GetByOrderNumber), new { orderNumber = order.OrderNumber }, order);
        }

        [HttpPatch("{orderNumber}")]
        public async Task<ActionResult> UpdateOrderAsync(int orderNumber, OrderDto order)
        {
            if (order == null || !order.IsValid())
            {
                return BadRequest("Order and order information cannot be null.");
            }

            if (!await parkingLotService.IsParkingLotExistedAsync(order.ParkingLotName))
            {
                return NotFound("ParkingLot not existed.");
            }

            var orderInMemory = await orderService.GetOrderAsync(orderNumber);

            if (orderInMemory == null || !orderInMemory.Equals(order))
            {
                return BadRequest("Order not existed.");
            }

            if (!await orderService.IsOrderOpenedAsync(orderNumber))
            {
                return BadRequest("Order is used.");
            }

            await orderService.CloseOrderAsync(orderNumber);
            return NoContent();
        }

        [HttpGet("{orderNumber}")]
        public async Task<ActionResult<OrderDto>> GetByOrderNumber(int orderNumber)
        {
            var order = await orderService.GetOrderAsync(orderNumber);

            if (order == null)
            {
                return NotFound("Order not existed.");
            }

            return Ok(order);
        }

        private async Task<bool> IsParkingLotFullAsync(string name)
        {
            var parkingLotCapacity = await parkingLotService.GetParkingLotCapacity(name); 
            var numberOfCarsInParkingLot = await orderService.GetOrderOpenedInParkingLotAsync(name);
            return parkingLotCapacity <= numberOfCarsInParkingLot;
        }
    }
}

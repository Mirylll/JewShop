using JewShop.Server.Services.Interfaces;
using JewShop.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace JewShop.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto dto)
        {
            // Get UserId from authenticated user (optional - guest checkout allowed)
            int? userId = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                Console.WriteLine($"[DEBUG OrdersController] User is authenticated");
                
                // JWT uses ClaimTypes.NameIdentifier for UserId
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                Console.WriteLine($"[DEBUG OrdersController] userIdClaim: {userIdClaim?.Value ?? "NULL"}");
                
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedUserId))
                {
                    userId = parsedUserId;
                    Console.WriteLine($"[DEBUG OrdersController] Parsed userId: {userId}");
                }
            }
            else
            {
                Console.WriteLine($"[DEBUG OrdersController] User is NOT authenticated");
            }
            
            Console.WriteLine($"[DEBUG OrdersController] Calling CreateOrderAsync with userId: {userId}");
            var order = await _orderService.CreateOrderAsync(dto, userId);
            if (order == null) return BadRequest("Cannot create order. Cart might be empty.");
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        [HttpGet]
        public async Task<ActionResult<List<OrderDto>>> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();
            return Ok(order);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var success = await _orderService.UpdateOrderStatusAsync(id, status);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}

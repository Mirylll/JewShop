using JewShop.Server.Services.Interfaces;
using JewShop.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace JewShop.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("{sessionId}")]
        public async Task<ActionResult<List<CartItemDto>>> GetCart(string sessionId)
        {
            var cart = await _cartService.GetCartItemsAsync(sessionId);
            return Ok(cart);
        }

        [HttpPost]
        public async Task<ActionResult<CartItemDto>> AddToCart(AddToCartDto dto)
        {
            var item = await _cartService.AddToCartAsync(dto);
            if (item == null) return BadRequest("Cannot add item to cart.");
            return Ok(item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuantity(int id, [FromBody] UpdateCartItemDto dto)
        {
            var success = await _cartService.UpdateQuantityAsync(id, dto.Quantity);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var success = await _cartService.RemoveFromCartAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpDelete("clear/{sessionId}")]
        public async Task<IActionResult> ClearCart(string sessionId)
        {
            await _cartService.ClearCartAsync(sessionId);
            return NoContent();
        }
    }
}
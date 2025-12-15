using JewShop.Server.Services.Interfaces;
using JewShop.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace JewShop.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponsController : ControllerBase
    {
        private readonly ICouponService _couponService;

        public CouponsController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [HttpGet]
        public async Task<ActionResult<List<CouponDto>>> GetAllCoupons()
        {
            return Ok(await _couponService.GetAllCouponsAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CouponDto>> GetCoupon(int id)
        {
            var coupon = await _couponService.GetCouponByIdAsync(id);
            if (coupon == null) return NotFound();
            return Ok(coupon);
        }

        [HttpPost]
        public async Task<ActionResult<CouponDto>> CreateCoupon(CouponDto dto)
        {
            var existing = await _couponService.GetCouponByCodeAsync(dto.Code);
            if (existing != null)
            {
                return BadRequest("Mã giảm giá đã tồn tại.");
            }

            var createdCoupon = await _couponService.CreateCouponAsync(dto);
            return CreatedAtAction(nameof(GetCoupon), new { id = createdCoupon.Id }, createdCoupon);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCoupon(int id, CouponDto dto)
        {
            var result = await _couponService.UpdateCouponAsync(id, dto);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            var result = await _couponService.DeleteCouponAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpPost("validate")]
        public async Task<ActionResult<CouponDto>> ValidateCoupon([FromBody] string code)
        {
            var coupon = await _couponService.ValidateCouponAsync(code, 0); // Order value check can be added later
            if (coupon == null) return BadRequest("Invalid or expired coupon code.");
            return Ok(coupon);
        }
    }
}

using System;
using JewShop.Server.Services.Interfaces;
using JewShop.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace JewShop.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponsController : ControllerBase
    {
        private readonly ICouponService _service;

        public CouponsController(ICouponService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var coupon = await _service.GetByIdAsync(id);
            if (coupon == null)
            {
                return NotFound();
            }

            return Ok(coupon);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCouponDto dto)
        {
            try
            {
                var created = await _service.CreateAsync(dto);
                return Ok(created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateCouponDto dto)
        {
            try
            {
                var updated = await _service.UpdateAsync(id, dto);
                if (!updated)
                {
                    return NotFound();
                }

                return Ok(true);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return Ok(true);
        }

        [HttpPost("check")]
        public async Task<IActionResult> Check(CheckCouponRequestDto dto)
            => Ok(await _service.CheckAsync(dto));
    }
}

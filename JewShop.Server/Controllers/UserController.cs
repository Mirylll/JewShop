using JewShop.Server.Services.Interfaces;
using JewShop.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JewShop.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // Helper để lấy UserId từ Token
        private int GetUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim != null && int.TryParse(idClaim.Value, out int userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException("Invalid Token");
        }

        [HttpGet("profile")]
        public async Task<ActionResult<UserProfileDto>> GetProfile()
        {
            try
            {
                var userId = GetUserId();
                var profile = await _userService.GetProfileAsync(userId);
                if (profile == null) return NotFound("User not found");
                
                return Ok(profile);
            }
            catch (Exception)
            {
                return Unauthorized();
            }
        }

        [HttpPost("address")]
        public async Task<ActionResult<AddressDto>> AddAddress(AddressDto request)
        {
            var userId = GetUserId();
            var result = await _userService.AddAddressAsync(userId, request);
            return Ok(result);
        }

        [HttpDelete("address/{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var userId = GetUserId();
            var success = await _userService.DeleteAddressAsync(userId, id);
            if (!success) return NotFound("Address not found");
            
            return Ok(new { message = "Deleted successfully" });
        }



        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request)
        {
            var userId = GetUserId();
            var success = await _userService.UpdateProfileAsync(userId, request);
            if (!success) return BadRequest("Không thể cập nhật.");
            return Ok(new { message = "Cập nhật thành công" });
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            var userId = GetUserId();
            var success = await _userService.ChangePasswordAsync(userId, request);
            if (!success) return BadRequest("Mật khẩu cũ không đúng.");
            return Ok(new { message = "Đổi mật khẩu thành công" });
        }
    }
}
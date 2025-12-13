using JewShop.Server.Services.Interfaces;
using JewShop.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace JewShop.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(UserRegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(UserLoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}
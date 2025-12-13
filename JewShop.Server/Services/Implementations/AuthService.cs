using JewShop.Server.Data;
using JewShop.Server.Models;
using JewShop.Server.Services.Interfaces;
using JewShop.Shared.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JewShop.Server.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponse> RegisterAsync(UserRegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return new AuthResponse { Success = false, Message = "Email đã tồn tại." };
            }

            // Mã hóa mật khẩu (BCrypt tự sinh salt và gộp vào hash)
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Email = request.Email,
                Name = request.Name,
                PasswordHash = passwordHash,
                IsVerified = 1, // Tạm thời cho active luôn để test
                Type = "customer",
                Status = "active",
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new AuthResponse { Success = true, Message = "Đăng ký thành công!" };
        }

        public async Task<AuthResponse> LoginAsync(UserLoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            
            // Kiểm tra user và mật khẩu
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return new AuthResponse { Success = false, Message = "Sai email hoặc mật khẩu." };
            }

            // Nếu user bị khóa
            if (user.Status != "active")
            {
                return new AuthResponse { Success = false, Message = "Tài khoản đã bị khóa." };
            }

            string token = CreateToken(user);

            // Lưu Session (Demo đơn giản, sau này có thể mở rộng Refresh Token)
            var session = new Session
            {
                UserId = user.UserId,
                RefreshTokenHash = "demo_refresh_hash", 
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddDays(7)
            };
            _context.Sessions.Add(session);
            await _context.SaveChangesAsync();

            return new AuthResponse { Success = true, Token = token, Message = "Đăng nhập thành công" };
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name ?? ""),
                new Claim(ClaimTypes.Role, user.Type)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("Jwt:Key").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds,
                issuer: _configuration.GetSection("Jwt:Issuer").Value,
                audience: _configuration.GetSection("Jwt:Audience").Value
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
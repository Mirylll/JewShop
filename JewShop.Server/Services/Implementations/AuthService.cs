using JewShop.Server.Data;
using JewShop.Server.Models;
using JewShop.Server.Services.Interfaces;
using JewShop.Shared.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography; // Cần thêm thư viện này để tạo Token ngẫu nhiên
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

        // 1. ĐĂNG KÝ
        public async Task<AuthResponse> RegisterAsync(UserRegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return new AuthResponse { Success = false, Message = "Email đã tồn tại." };
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Email = request.Email,
                Name = request.Name,
                PasswordHash = passwordHash,
                IsVerified = 1, // Tạm thời active luôn
                Type = "customer",
                Status = "active",
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new AuthResponse { Success = true, Message = "Đăng ký thành công!" };
        }

        // 2. ĐĂNG NHẬP
        public async Task<AuthResponse> LoginAsync(UserLoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return new AuthResponse { Success = false, Message = "Sai email hoặc mật khẩu." };
            }

            if (user.Status != "active")
            {
                return new AuthResponse { Success = false, Message = "Tài khoản đã bị khóa." };
            }

            string accessToken = CreateToken(user);
            
            // Tạo Refresh Token ngẫu nhiên
            string refreshToken = GenerateRandomToken();

            // Lưu Session vào DB (Hash refresh token để bảo mật)
            var session = new Session
            {
                UserId = user.UserId,
                RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(refreshToken), 
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddDays(7), // Token sống 7 ngày
                IpAddress = "::1", // Có thể lấy từ HttpContext nếu muốn
                UserAgent = "Web",
                Revoked = 0
            };
            
            _context.Sessions.Add(session);
            await _context.SaveChangesAsync();

            // Trả về Access Token và Refresh Token (dạng raw chưa hash) cho client
            // Lưu ý: Tạm thời mình dùng property 'Message' để chứa RefreshToken trả về
            // Hoặc bạn nên thêm field RefreshToken vào AuthResponse DTO
            return new AuthResponse 
            { 
                Success = true, 
                Token = accessToken, 
                Message = refreshToken // Client sẽ lưu cái này để refresh sau này
            };
        }

        // 3. REFRESH TOKEN (Cấp lại token mới)
        public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            // Tìm các session còn hạn và chưa bị thu hồi
            // Lưu ý: Vì ta chỉ lưu Hash, nên không thể tìm trực tiếp bằng request.RefreshToken
            // Cách tối ưu là Client gửi kèm UserId hoặc AccessToken cũ. 
            // Ở đây mình quét các session còn hạn (Cách đơn giản cho demo)
            
            var activeSessions = await _context.Sessions
                .Where(s => s.ExpiresAt > DateTime.Now && s.Revoked == 0)
                .ToListAsync();

            Session? validSession = null;

            foreach (var session in activeSessions)
            {
                // Verify xem token client gửi lên có khớp với hash trong DB không
                if (BCrypt.Net.BCrypt.Verify(request.RefreshToken, session.RefreshTokenHash))
                {
                    validSession = session;
                    break;
                }
            }

            if (validSession == null)
            {
                return new AuthResponse { Success = false, Message = "Token không hợp lệ hoặc đã hết hạn." };
            }

            var user = await _context.Users.FindAsync(validSession.UserId);
            if (user == null) return new AuthResponse { Success = false, Message = "User không tồn tại." };

            // Cấp Access Token mới
            string newAccessToken = CreateToken(user);

            // (Tuỳ chọn) Bạn có thể tạo luôn RefreshToken mới và hủy cái cũ ở đây để tăng bảo mật (Rotation)

            return new AuthResponse { Success = true, Token = newAccessToken };
        }

        // 4. QUÊN MẬT KHẨU
        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return false; // Không báo lỗi user không tồn tại để tránh hacker dò

            // Tạo token reset
            string token = GenerateRandomToken(); // Hoặc Guid.NewGuid().ToString()

            var passwordReset = new PasswordReset
            {
                UserId = user.UserId,
                Token = token, // Lưu token raw (hoặc hash nếu muốn bảo mật cao hơn)
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddMinutes(30), // Hết hạn sau 30 phút
                Used = 0
            };

            _context.PasswordResets.Add(passwordReset);
            await _context.SaveChangesAsync();

            // GIẢ LẬP GỬI EMAIL (In ra Console Server để bạn copy test)
            Console.WriteLine($"==================================================");
            Console.WriteLine($"[EMAIL MOCK] Reset Password Link cho {email}");
            Console.WriteLine($"Token: {token}");
            Console.WriteLine($"Link: http://localhost:5289/reset-password?token={token}");
            Console.WriteLine($"==================================================");

            return true;
        }

        // 5. ĐẶT LẠI MẬT KHẨU
        public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request)
        {
            // Tìm token trong DB: Phải đúng token, chưa dùng, và còn hạn
            var resetEntry = await _context.PasswordResets
                .FirstOrDefaultAsync(r => r.Token == request.Token && r.Used == 0 && r.ExpiresAt > DateTime.Now);

            if (resetEntry == null) return false;

            var user = await _context.Users.FindAsync(resetEntry.UserId);
            if (user == null) return false;

            // Cập nhật mật khẩu mới
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            
            // Đánh dấu token đã dùng
            resetEntry.Used = 1;

            await _context.SaveChangesAsync();
            return true;
        }

        // --- HELPER METHODS ---

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
                expires: DateTime.Now.AddDays(1), // Access Token ngắn hạn
                signingCredentials: creds,
                issuer: _configuration.GetSection("Jwt:Issuer").Value,
                audience: _configuration.GetSection("Jwt:Audience").Value
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRandomToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}
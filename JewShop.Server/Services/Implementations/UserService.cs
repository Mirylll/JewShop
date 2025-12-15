using JewShop.Server.Data;
using JewShop.Server.Models;
using JewShop.Server.Services.Interfaces;
using JewShop.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace JewShop.Server.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserProfileDto?> GetProfileAsync(int userId)
        {
            
            var user = await _context.Users
                .Include(u => u.Addresses)
                .Include(u => u.Orders) // Include Orders
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null) return null;

            Console.WriteLine($"[DEBUG] User {userId} found. Orders count: {user.Orders?.Count ?? 0}");
            
            return new UserProfileDto
            {
                UserId = user.UserId,
                Email = user.Email,
                Name = user.Name,
                Phone = user.Phone,
                Type = user.Type,
                Addresses = user.Addresses.Select(a => new AddressDto
                {
                    AddressId = a.AddressId,
                    Label = a.Label,
                    FullAddress = a.FullAddress,
                    City = a.City,
                    Phone = a.Phone,
                    IsDefault = a.IsDefault == 1 
                }).ToList(),
                Orders = user.Orders.Select(o => new OrderDto
                {
                    Id = o.Id,
                    OrderCode = o.OrderCode,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt
                }).OrderByDescending(x => x.CreatedAt).ToList()
            };
        }

        public async Task<AddressDto> AddAddressAsync(int userId, AddressDto dto)
        {
            var newAddress = new Address
            {
                UserId = userId,
                Label = dto.Label,
                FullAddress = dto.FullAddress,
                City = dto.City,
                Phone = dto.Phone,
                IsDefault = (byte)(dto.IsDefault ? 1 : 0)
            };

            _context.Addresses.Add(newAddress);
            await _context.SaveChangesAsync();

            dto.AddressId = newAddress.AddressId; 
            return dto;
        }

        public async Task<bool> DeleteAddressAsync(int userId, int addressId)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.AddressId == addressId && a.UserId == userId);

            if (address == null) return false;

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
            return true;
        }



        

        public async Task<bool> UpdateProfileAsync(int userId, UpdateProfileRequest request)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.Name = request.Name;
            user.Phone = request.Phone;
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            
            if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash))
            {
                return false;
            }

            
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _context.SaveChangesAsync();
            
            return true;
        }

        // Admin methods
        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            return await _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    Email = u.Email,
                    Name = u.Name,
                    Phone = u.Phone,
                    Type = u.Type,
                    Status = u.Status,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<bool> UpdateUserRoleAsync(int userId, string newRole)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            if (newRole != "customer" && newRole != "admin")
            {
                return false;
            }

            user.Type = newRole;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateUserStatusAsync(int userId, string status)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            if (status != "active" && status != "locked")
            {
                return false;
            }

            user.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}   
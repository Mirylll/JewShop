using JewShop.Shared.Dtos;

namespace JewShop.Server.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserProfileDto?> GetProfileAsync(int userId);
        Task<AddressDto> AddAddressAsync(int userId, AddressDto addressDto);
        Task<bool> DeleteAddressAsync(int userId, int addressId);
        Task<bool> UpdateProfileAsync(int userId, UpdateProfileRequest request);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request);
        
        // Admin methods
        Task<List<UserDto>> GetAllUsersAsync();
        Task<bool> UpdateUserRoleAsync(int userId, string newRole);
        Task<bool> UpdateUserStatusAsync(int userId, string status);
    }
}
using JewShop.Shared.Dtos;
using System.Net.Http.Json;

namespace JewShop.Client.Services
{
    public interface IUserService
    {
        Task<UserProfileDto?> GetProfile();
        Task<AddressDto?> AddAddress(AddressDto request);
        Task<bool> DeleteAddress(int addressId);
        
        // --- CÁC HÀM MỚI ---
        Task<bool> UpdateProfile(UpdateProfileRequest request);
        Task<bool> ChangePassword(ChangePasswordRequest request);
    }

    public class UserService : IUserService
    {
        private readonly HttpClient _http;

        public UserService(HttpClient http)
        {
            _http = http;
        }

        public async Task<UserProfileDto?> GetProfile()
        {
            try 
            {
                 return await _http.GetFromJsonAsync<UserProfileDto>("api/user/profile");
            }
            catch { return null; }
        }

        public async Task<AddressDto?> AddAddress(AddressDto request)
        {
            var result = await _http.PostAsJsonAsync("api/user/address", request);
            return result.IsSuccessStatusCode ? await result.Content.ReadFromJsonAsync<AddressDto>() : null;
        }

        public async Task<bool> DeleteAddress(int addressId)
        {
            var result = await _http.DeleteAsync($"api/user/address/{addressId}");
            return result.IsSuccessStatusCode;
        }

        // --- CÁC HÀM MỚI ---
        public async Task<bool> UpdateProfile(UpdateProfileRequest request)
        {
            var result = await _http.PutAsJsonAsync("api/user/profile", request);
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> ChangePassword(ChangePasswordRequest request)
        {
            var result = await _http.PostAsJsonAsync("api/user/change-password", request);
            return result.IsSuccessStatusCode;
        }
    }
}
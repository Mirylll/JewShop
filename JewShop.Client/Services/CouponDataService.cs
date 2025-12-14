using System.Net.Http.Json;
using JewShop.Shared.Dtos;

namespace JewShop.Client.Services
{
    public class CouponDataService
    {
        private readonly HttpClient _httpClient;
        private const string Endpoint = "api/coupons";

        public CouponDataService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<CouponDto>> GetAllAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<List<CouponDto>>(Endpoint);
            return result ?? [];
        }

        public async Task<CouponDto?> CreateAsync(CreateCouponDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync(Endpoint, dto);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<CouponDto>();
        }

        public async Task<bool> UpdateAsync(int id, UpdateCouponDto dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"{Endpoint}/{id}", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{Endpoint}/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<CheckCouponResponseDto?> CheckAsync(CheckCouponRequestDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync($"{Endpoint}/check", dto);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<CheckCouponResponseDto>();
        }
    }
}

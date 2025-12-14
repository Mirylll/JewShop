using JewShop.Shared.Dtos;
using System.Net.Http.Json;

namespace JewShop.Client.Services
{
    public class CouponService
    {
        private readonly HttpClient _http;

        public CouponService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<CouponDto>> GetAllCoupons()
        {
            return await _http.GetFromJsonAsync<List<CouponDto>>("api/coupons") ?? new List<CouponDto>();
        }

        public async Task<CouponDto?> GetCouponById(int id)
        {
            return await _http.GetFromJsonAsync<CouponDto>($"api/coupons/{id}");
        }

        public async Task<CouponDto?> CreateCoupon(CouponDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/coupons", dto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CouponDto>();
            }
            return null;
        }

        public async Task<bool> UpdateCoupon(int id, CouponDto dto)
        {
            var response = await _http.PutAsJsonAsync($"api/coupons/{id}", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteCoupon(int id)
        {
            var response = await _http.DeleteAsync($"api/coupons/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<CouponDto?> ValidateCoupon(string code)
        {
            var response = await _http.PostAsJsonAsync("api/coupons/validate", code);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CouponDto>();
            }
            return null;
        }
    }
}

using System.Net.Http.Json;
using JewShop.Shared.Dtos;

namespace JewShop.Client.Services
{
    public class SupplierDataService
    {
        private readonly HttpClient _httpClient;
        private const string Endpoint = "api/suppliers";

        public SupplierDataService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<SupplierDto>> GetAllAsync(string? search = null)
        {
            var uri = string.IsNullOrWhiteSpace(search)
                ? Endpoint
                : $"{Endpoint}?search={Uri.EscapeDataString(search)}";

            var result = await _httpClient.GetFromJsonAsync<List<SupplierDto>>(uri);
            return result ?? [];
        }

        public async Task<SupplierDto?> GetByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<SupplierDto>($"{Endpoint}/{id}");
        }

        public async Task<SupplierDto?> CreateAsync(CreateSupplierDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync(Endpoint, dto);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<SupplierDto>();
        }

        public async Task<SupplierDto?> UpdateAsync(int id, UpdateSupplierDto dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"{Endpoint}/{id}", dto);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<SupplierDto>();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{Endpoint}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}

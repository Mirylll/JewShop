using System.Net.Http.Json;
using JewShop.Shared.Dtos;

namespace JewShop.Client.Services
{
    public class ProductDataService
    {
        private readonly HttpClient _httpClient;
        private const string Endpoint = "api/products";

        public ProductDataService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ProductDto>> GetAllAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<List<ProductDto>>(Endpoint);
            return result ?? [];
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<ProductDto>($"{Endpoint}/{id}");
        }

        public async Task<ProductDto?> CreateAsync(CreateProductDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync(Endpoint, dto);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<ProductDto>();
        }

        public async Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"{Endpoint}/{id}", dto);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<ProductDto>();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{Endpoint}/{id}");
            return response.IsSuccessStatusCode;
        }

        // Image Methods
        public async Task<ProductImageDto?> AddImageAsync(int productId, CreateProductImageDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync($"{Endpoint}/{productId}/images", dto);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<ProductImageDto>();
        }

        public async Task<bool> DeleteImageAsync(int imageId)
        {
            var response = await _httpClient.DeleteAsync($"{Endpoint}/images/{imageId}");
            return response.IsSuccessStatusCode;
        }

        // Variant Methods
        public async Task<ProductVariantDto?> AddVariantAsync(int productId, CreateProductVariantDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync($"{Endpoint}/{productId}/variants", dto);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<ProductVariantDto>();
        }

        public async Task<ProductVariantDto?> UpdateVariantAsync(int variantId, UpdateProductVariantDto dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"{Endpoint}/variants/{variantId}", dto);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<ProductVariantDto>();
        }

        public async Task<bool> DeleteVariantAsync(int variantId)
        {
            var response = await _httpClient.DeleteAsync($"{Endpoint}/variants/{variantId}");
            return response.IsSuccessStatusCode;
        }
    }
}

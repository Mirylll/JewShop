using JewShop.Shared.Dtos;
using System.Net.Http.Json;

namespace JewShop.Client.Services
{
    public class ProductDataService
    {
        private readonly HttpClient _http;

        public ProductDataService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<ProductDto>> GetAll()
        {
            return await _http.GetFromJsonAsync<List<ProductDto>>("api/product") ?? [];
        }
    }
}

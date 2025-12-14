using System.Net.Http.Json;
using JewShop.Shared.Dtos;

namespace JewShop.Client.Services
{
    public class OrderService
    {
        private readonly HttpClient _http;

        public OrderService(HttpClient http)
        {
            _http = http;
        }

        public async Task<OrderDto?> CreateOrder(CreateOrderDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/orders", dto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<OrderDto>();
            }
            return null;
        }

        public async Task<List<OrderDto>> GetAllOrders()
        {
            return await _http.GetFromJsonAsync<List<OrderDto>>("api/orders") ?? new List<OrderDto>();
        }

        public async Task<OrderDto?> GetOrderById(int id)
        {
            try
            {
                return await _http.GetFromJsonAsync<OrderDto>($"api/orders/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UpdateStatus(int id, string status)
        {
            var response = await _http.PutAsJsonAsync($"api/orders/{id}/status", status);
            return response.IsSuccessStatusCode;
        }
    }
}

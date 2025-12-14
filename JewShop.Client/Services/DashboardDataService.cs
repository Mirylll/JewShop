using System.Net.Http.Json;
using JewShop.Shared.Dtos;

namespace JewShop.Client.Services
{
    public class DashboardDataService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:5289"; // API dev port
        private const string Endpoint = BaseUrl + "/api/dashboard/summary";

        public DashboardDataService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DashboardSummaryDto?> GetSummaryAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<DashboardSummaryDto>(Endpoint, cancellationToken);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Lỗi gọi API: {ex.Message}");
                return null;
            }
        }
    }
}

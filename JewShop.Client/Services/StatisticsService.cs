using System.Net.Http.Json;
using JewShop.Shared.Dtos;

namespace JewShop.Client.Services
{
    public class StatisticsService(HttpClient http)
    {
        private readonly HttpClient _http = http;

        public async Task<DashboardStatsDto?> GetOverviewAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<DashboardStatsDto>("api/statistics/overview");
            }
            catch
            {
                return null;
            }
        }
    }
}

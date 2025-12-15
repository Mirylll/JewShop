using JewShop.Shared.Dtos;

namespace JewShop.Server.Services.Interfaces;

public interface IStatisticsService
{
    Task<DashboardStatsDto> GetOverviewAsync();
}

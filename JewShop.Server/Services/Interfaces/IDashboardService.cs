using JewShop.Shared.Dtos;

namespace JewShop.Server.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
    }
}

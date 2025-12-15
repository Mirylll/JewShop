using JewShop.Server.Services.Interfaces;
using JewShop.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JewShop.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatisticsController(IStatisticsService statisticsService) : ControllerBase
{
    private readonly IStatisticsService _statisticsService = statisticsService;

    [HttpGet("overview")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<DashboardStatsDto>> GetOverview()
    {
        var overview = await _statisticsService.GetOverviewAsync();
        return Ok(overview);
    }
}

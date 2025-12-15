using JewShop.Server.Data;
using JewShop.Server.Services.Interfaces;
using JewShop.Shared.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace JewShop.Server.Services.Implementations;

public class StatisticsService(ApplicationDbContext context) : IStatisticsService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<DashboardStatsDto> GetOverviewAsync()
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        var orders = _context.Orders.AsNoTracking();
        var activeOrders = orders.Where(o => o.Status != "cancelled");

        var totalOrders = await orders.CountAsync();
        var pendingOrders = await orders.CountAsync(o => o.Status == "pending");
        var deliveredOrders = await orders.CountAsync(o => o.Status == "delivered");
        var totalCustomers = await _context.Users.AsNoTracking().CountAsync(u => u.Type == "customer");
        var totalProducts = await _context.Products.AsNoTracking().CountAsync();
        var lowStock = await _context.ProductVariants.AsNoTracking().CountAsync(v => v.Stock <= v.ReorderLevel);
        var totalRevenue = await activeOrders.SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;
        var todayRevenue = await activeOrders
            .Where(o => o.CreatedAt >= today && o.CreatedAt < tomorrow)
            .SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;

        var topProducts = await _context.OrderItems.AsNoTracking()
            .Where(oi => oi.VariantId != null)
            .Join(
                _context.ProductVariants.AsNoTracking(),
                oi => oi.VariantId,
                pv => pv.Id,
                (oi, pv) => new { oi, pv.ProductId }
            )
            .Join(
                _context.Products.AsNoTracking(),
                temp => temp.ProductId,
                p => p.Id,
                (temp, product) => new { product.Name, temp.oi.Quantity, temp.oi.UnitPrice }
            )
            .GroupBy(x => x.Name)
            .Select(g => new TopProductStatDto
            {
                ProductName = g.Key,
                UnitsSold = g.Sum(x => x.Quantity),
                Revenue = g.Sum(x => x.Quantity * x.UnitPrice)
            })
            .OrderByDescending(x => x.UnitsSold)
            .ThenByDescending(x => x.Revenue)
            .Take(5)
            .ToListAsync();

        var categoryRevenueRaw = await _context.OrderItems.AsNoTracking()
            .Join(
                activeOrders,
                oi => oi.OrderId,
                o => o.Id,
                (oi, _) => oi
            )
            .Where(oi => oi.VariantId != null)
            .Join(
                _context.ProductVariants.AsNoTracking(),
                oi => oi.VariantId,
                pv => pv.Id,
                (oi, pv) => new { oi, pv.ProductId }
            )
            .Join(
                _context.Products.AsNoTracking(),
                temp => temp.ProductId,
                p => p.Id,
                (temp, product) => new
                {
                    Category = string.IsNullOrWhiteSpace(product.Category) ? "Khác" : product.Category,
                    Revenue = temp.oi.Quantity * temp.oi.UnitPrice
                }
            )
            .GroupBy(x => x.Category)
            .Select(g => new
            {
                Category = g.Key,
                Revenue = g.Sum(x => x.Revenue)
            })
            .OrderByDescending(x => x.Revenue)
            .ToListAsync();

        var totalCategoryRevenue = categoryRevenueRaw.Sum(x => x.Revenue);
        var categoryDistribution = categoryRevenueRaw
            .Select(slice => new CategoryRevenueSliceDto
            {
                CategoryName = slice.Category,
                Revenue = slice.Revenue,
                Percentage = totalCategoryRevenue > 0m
                    ? Math.Round((slice.Revenue / totalCategoryRevenue) * 100m, 2, MidpointRounding.AwayFromZero)
                    : 0m
            })
            .ToList();

        return new DashboardStatsDto
        {
            TotalOrders = totalOrders,
            PendingOrders = pendingOrders,
            DeliveredOrders = deliveredOrders,
            TotalCustomers = totalCustomers,
            TotalProducts = totalProducts,
            LowStockVariants = lowStock,
            TotalRevenue = totalRevenue,
            TodayRevenue = todayRevenue,
            TopProducts = topProducts,
            CategoryRevenueDistribution = categoryDistribution
        };
    }
}

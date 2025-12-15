using JewShop.Server.Data;
using JewShop.Server.Services.Interfaces;
using JewShop.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace JewShop.Server.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;
        private const int LowStockThreshold = 5;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;

            var productQuery = _context.Products.AsNoTracking();
            var supplierQuery = _context.Suppliers.AsNoTracking();
            var couponQuery = _context.Coupons.AsNoTracking();

            // Đếm tổng
            var productCount = await productQuery.CountAsync(cancellationToken);
            var supplierCount = await supplierQuery.CountAsync(cancellationToken);
            var couponCount = await couponQuery.CountAsync(cancellationToken);
            var activeCouponCount = await couponQuery.CountAsync(c => c.Active && (c.StartsAt ?? DateTime.MinValue) <= now && (c.ExpiresAt ?? DateTime.MinValue) >= now, cancellationToken);
            var upcomingCouponCount = await couponQuery.CountAsync(c => (c.StartsAt ?? DateTime.MinValue) > now, cancellationToken);
            var expiredCouponCount = await couponQuery.CountAsync(c => (c.ExpiresAt ?? DateTime.MinValue) < now, cancellationToken);
            var outOfStockCount = await productQuery.CountAsync(p => p.StockQuantity == 0, cancellationToken);
            var lowStockCount = await productQuery.CountAsync(p => p.StockQuantity > 0 && p.StockQuantity <= LowStockThreshold, cancellationToken);

            // Tổng giá trị kho
            var totalInventoryValue = await productQuery
                .Select(p => (decimal?)(p.Price * p.StockQuantity))
                .SumAsync(cancellationToken) ?? 0m;

            var averageProductPrice = await productQuery
                .Select(p => (decimal?)p.Price)
                .AverageAsync(cancellationToken) ?? 0m;

            // Sản phẩm gần hết hàng
            var lowStockProducts = await productQuery
                .Where(p => p.StockQuantity <= LowStockThreshold)
                .OrderBy(p => p.StockQuantity)
                .ThenBy(p => p.Name)
                .Take(6)
                .Select(p => new DashboardLowStockDto
                {
                    ProductId = p.Id,
                    Name = p.Name,
                    StockQuantity = p.StockQuantity,
                    Price = p.Price
                })
                .ToListAsync(cancellationToken);

            // Coupon snapshot
            var couponSnapshots = await couponQuery
                .OrderByDescending(c => c.Active)
                .ThenBy(c => c.ExpiresAt)
                .Take(6)
                .Select(c => new DashboardCouponSnapshotDto
                {
                    CouponId = c.Id,
                    Code = c.Code,
                    IsActive = c.Active
                        && (c.StartsAt ?? DateTime.MinValue) <= now
                        && (c.ExpiresAt ?? DateTime.MaxValue) >= now,
                    StartsAt = c.StartsAt ?? DateTime.UtcNow,
                    ExpiresAt = c.ExpiresAt ?? DateTime.UtcNow,
                    UsedCount = c.UsedCount ?? 0,
                    UsageLimit = c.UsageLimit ?? 0
                })
                .ToListAsync(cancellationToken);

            return new DashboardSummaryDto
            {
                GeneratedAtUtc = now,
                ProductCount = productCount,
                SupplierCount = supplierCount,
                CouponCount = couponCount,
                ActiveCouponCount = activeCouponCount,
                UpcomingCouponCount = upcomingCouponCount,
                ExpiredCouponCount = expiredCouponCount,
                OutOfStockCount = outOfStockCount,
                LowStockCount = lowStockCount,
                LowStockThreshold = LowStockThreshold,
                TotalInventoryValue = totalInventoryValue,
                AverageProductPrice = averageProductPrice,
                LowStockProducts = lowStockProducts,
                CouponSnapshots = couponSnapshots
            };
        }
    }
}

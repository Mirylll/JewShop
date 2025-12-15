using System;
using System.Collections.Generic;

namespace JewShop.Shared.Dtos
{
    public class DashboardSummaryDto
    {
        public DateTime GeneratedAtUtc { get; set; }
        public int ProductCount { get; set; }
        public int SupplierCount { get; set; }
        public int CouponCount { get; set; }
        public int ActiveCouponCount { get; set; }
        public int UpcomingCouponCount { get; set; }
        public int ExpiredCouponCount { get; set; }
        public int OutOfStockCount { get; set; }
        public int LowStockCount { get; set; }
        public int LowStockThreshold { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public decimal AverageProductPrice { get; set; }
        public List<DashboardLowStockDto> LowStockProducts { get; set; } = [];
        public List<DashboardCouponSnapshotDto> CouponSnapshots { get; set; } = [];
    }

    public class DashboardLowStockDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public decimal Price { get; set; }
    }

    public class DashboardCouponSnapshotDto
    {
        public int CouponId { get; set; }
        public string Code { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime StartsAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public int UsedCount { get; set; }
        public int UsageLimit { get; set; }
    }
}

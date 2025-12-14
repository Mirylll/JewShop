using System;

namespace JewShop.Shared.Dtos
{
    public class CouponDto
    {
        public int Id { get; set; }

        public string Code { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Type { get; set; } = "percent";

        public decimal Value { get; set; }

        public decimal MinOrderValue { get; set; }

        public int UsageLimit { get; set; }

        public int UsedCount { get; set; }

        public DateTime StartsAt { get; set; }

        public DateTime ExpiresAt { get; set; }

        public bool Active { get; set; }
    }
}

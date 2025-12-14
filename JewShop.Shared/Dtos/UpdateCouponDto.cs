using System;
using System.ComponentModel.DataAnnotations;

namespace JewShop.Shared.Dtos
{
    public class UpdateCouponDto
    {
        [Required]
        [StringLength(120)]
        public string Code { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [RegularExpression("percent|fixed", ErrorMessage = "Type must be percent or fixed")]
        public string Type { get; set; } = "percent";

        [Range(0.01, double.MaxValue)]
        public decimal Value { get; set; } = 10;

        [Range(0, double.MaxValue)]
        public decimal MinOrderValue { get; set; }

        [Range(1, int.MaxValue)]
        public int UsageLimit { get; set; } = 100;

        [Required]
        public DateTime StartsAt { get; set; } = DateTime.UtcNow.Date;

        [Required]
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.Date.AddDays(7);

        public bool Active { get; set; } = true;
    }
}

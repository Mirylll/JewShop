using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JewShop.Server.Models
{
    [Table("Coupons")]
    public class Coupon
    {
        [Key]
        [Column("coupon_id")]
        public int Id { get; set; }

        [Required]
        [Column("code")]
        [StringLength(120)]
        public string Code { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Required]
        [Column("type")]
        [StringLength(20)]
        public string Type { get; set; } = "percent";

        [Column("value", TypeName = "decimal(10, 2)")]
        public decimal Value { get; set; }

        [Column("min_order_value", TypeName = "decimal(10, 2)")]
        public decimal MinOrderValue { get; set; }

        [Column("usage_limit")]
        public int UsageLimit { get; set; }

        [Column("used_count")]
        public int UsedCount { get; set; }

        [Column("starts_at")]
        public DateTime StartsAt { get; set; } = DateTime.UtcNow;

        [Column("expires_at")]
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(7);

        [Column("active")]
        public bool Active { get; set; } = true;
    }
}

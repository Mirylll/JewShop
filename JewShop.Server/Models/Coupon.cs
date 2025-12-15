
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

        [Column("code")]
        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("type")]
        public string Type { get; set; } = "percent"; // percent, fixed

        [Column("value")]
        public decimal Value { get; set; }

        [Column("min_order_value")]
        public decimal MinOrderValue { get; set; }

        [Column("usage_limit")]
        public int UsageLimit { get; set; }

        [Column("used_count")]
        public int UsedCount { get; set; }

        [Column("starts_at")]
        public DateTime? StartsAt { get; set; }

        [Column("expires_at")]
        public DateTime? ExpiryDate { get; set; }

        [Column("active")]
        public bool Active { get; set; } = true;
    }
}

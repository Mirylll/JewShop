using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JewShop.Server.Models
{
    public class Order
    {
        [Key]
        [Column("order_id")]
        public int Id { get; set; }

        [Column("order_code")]
        [Required]
        [StringLength(50)]
        public string OrderCode { get; set; } = string.Empty;

        [Column("user_id")]
        public int? UserId { get; set; }

        [Column("coupon_id")]
        public int? CouponId { get; set; }

        [Column("customer_name")]
        [StringLength(255)]
        public string? CustomerName { get; set; }

        [Column("customer_phone")]
        [StringLength(50)]
        public string? CustomerPhone { get; set; }

        [Column("shipping_address")]
        public string? ShippingAddress { get; set; }

        [Column("subtotal")]
        public decimal Subtotal { get; set; }

        [Column("discount_amount")]
        public decimal DiscountAmount { get; set; }

        [Column("shipping_fee")]
        public decimal ShippingFee { get; set; }

        [Column("total_amount")]
        public decimal TotalAmount { get; set; }

        [Column("status")]
        public string Status { get; set; } = "pending"; // pending, confirmed, shipping, delivered, cancelled, returned

        [Column("payment_status")]
        public string PaymentStatus { get; set; } = "unpaid"; // unpaid, partially_paid, paid, refunded

        [Column("note")]
        public string? Note { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("CouponId")]
        public Coupon? Coupon { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new();
    }
}
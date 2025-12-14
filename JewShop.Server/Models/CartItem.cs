using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JewShop.Server.Models
{
    public class CartItem
    {
        [Key]
        [Column("cart_item_id")]
        public int Id { get; set; }

        [Column("user_id")]
        public int? UserId { get; set; }

        [Column("session_id")]
        [StringLength(255)]
        public string? SessionId { get; set; }

        [Column("variant_id")]
        public int VariantId { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; } = 1;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("VariantId")]
        public ProductVariant? Variant { get; set; }
    }
}

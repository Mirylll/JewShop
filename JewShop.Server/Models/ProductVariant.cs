using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JewShop.Server.Models
{
    [Table("ProductVariants")]
    public class ProductVariant
    {
        [Key]
        [Column("variant_id")]
        public int Id { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("sku")]
        [StringLength(100)]
        public string? Sku { get; set; }

        [Column("size")]
        [StringLength(50)]
        public string? Size { get; set; }

        [Column("color")]
        [StringLength(50)]
        public string? Color { get; set; }

        [Column("length")]
        [StringLength(50)]
        public string? Length { get; set; }

        [Column("price")]
        public decimal? Price { get; set; }

        [Column("stock")]
        public int Stock { get; set; } = 0;

        [Column("reorder_level")]
        public int ReorderLevel { get; set; } = 5;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("ProductId")]
        public Product? Product { get; set; }
    }
}

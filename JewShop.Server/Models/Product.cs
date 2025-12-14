using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JewShop.Server.Models
{
    [Table("Products")]
    public class Product
    {
        [Key]
        [Column("product_id")]
        public int Id { get; set; }

        [Required]
        [Column("name")]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("category")]
        [StringLength(100)]
        public string? Category { get; set; }

        [Column("material")]
        [StringLength(100)]
        public string? Material { get; set; }

        [Column("gemstone")]
        [StringLength(100)]
        public string? Gemstone { get; set; }

        [Column("base_price")]
        public decimal BasePrice { get; set; } = 0;

        [Column("weight")]
        public decimal? Weight { get; set; }

        [Column("thumbnail_url")]
        public string? ThumbnailUrl { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    }
}

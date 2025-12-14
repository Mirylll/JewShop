using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JewShop.Server.Models
{
    [Table("ProductImages")]
    public class ProductImage
    {
        [Key]
        [Column("image_id")]
        public int Id { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("image_url")]
        public string? ImageUrl { get; set; }

        [Column("sort_order")]
        public int SortOrder { get; set; } = 0;

        [Column("is_primary")]
        public bool IsPrimary { get; set; } = false;

        [ForeignKey("ProductId")]
        public Product? Product { get; set; }
    }
}

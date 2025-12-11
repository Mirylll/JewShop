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
        public string Name { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

       
        [Column("category")] 
        public string? Category { get; set; } 
      

        [Column("base_price", TypeName = "decimal(15, 2)")] 
        public decimal Price { get; set; }

       
        [Column("stock_quantity")] 
        public int StockQuantity { get; set; } 

        [Column("thumbnail_url")] 
        public string? ImageUrl { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}
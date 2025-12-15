using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JewShop.Server.Models
{
    [Table("Imports")]
    public class Import
    {
        [Key]
        [Column("import_id")]
        public int Id { get; set; }

        [Column("import_code")]
        [Required]
        [StringLength(50)]
        public string ImportCode { get; set; } = string.Empty;

        [Column("supplier_id")]
        public int SupplierId { get; set; }

        [Column("import_date")]
        public DateTime ImportDate { get; set; } = DateTime.UtcNow;

        [Column("total_amount")]
        public decimal TotalAmount { get; set; }

        [Column("status")]
        [StringLength(20)]
        public string Status { get; set; } = "pending"; // pending, completed, cancelled

        [Column("note")]
        public string? Note { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("SupplierId")]
        public virtual Supplier? Supplier { get; set; }

        public virtual ICollection<ImportItem> ImportItems { get; set; } = new List<ImportItem>();
    }

    [Table("ImportItems")]
    public class ImportItem
    {
        [Key]
        [Column("import_item_id")]
        public int Id { get; set; }

        [Column("import_id")]
        public int ImportId { get; set; }

        [Column("variant_id")]
        public int VariantId { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("unit_price")]
        public decimal UnitPrice { get; set; }

        [Column("total_price")]
        public decimal TotalPrice { get; set; }

        [ForeignKey("ImportId")]
        public virtual Import? Import { get; set; }

        [ForeignKey("VariantId")]
        public virtual ProductVariant? Variant { get; set; }
    }
}

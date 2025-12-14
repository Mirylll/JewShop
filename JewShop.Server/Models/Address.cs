using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JewShop.Server.Models
{
    [Table("Addresses")]
    public class Address
    {
        [Key]
        [Column("address_id")]
        public int AddressId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("label")]
        public string Label { get; set; } = "Nhà riêng";

        [Column("full_address")]
        public string FullAddress { get; set; } = string.Empty;

        [Column("city")]
        public string? City { get; set; }

        [Column("phone")]
        public string? Phone { get; set; }

        [Column("is_default")]
        public byte IsDefault { get; set; } = 0; // Dùng byte giống User.IsVerified

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}
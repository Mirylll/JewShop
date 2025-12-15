using System.ComponentModel.DataAnnotations;

namespace JewShop.Shared.Dtos
{
    public class UpdateSupplierDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Phone { get; set; }

        [EmailAddress]
        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(300)]
        public string? Address { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace JewShop.Shared.Dtos
{
    public class CreateProductImageDto
    {
        [Required]
        public string ImageUrl { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public bool IsPrimary { get; set; }
    }
}

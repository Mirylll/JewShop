using System.ComponentModel.DataAnnotations;

namespace JewShop.Shared.Dtos
{
    public class UpdateProfileRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Phone]
        public string? Phone { get; set; }
    }
}
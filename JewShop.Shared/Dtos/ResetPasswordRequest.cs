using System.ComponentModel.DataAnnotations;

namespace JewShop.Shared.Dtos
{
    public class ResetPasswordRequest
    {
        [Required]
        public string Token { get; set; } = string.Empty; 

        [Required, MinLength(6)]
        public string NewPassword { get; set; } = string.Empty;

        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
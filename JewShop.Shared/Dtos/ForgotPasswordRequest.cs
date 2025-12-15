using System.ComponentModel.DataAnnotations;

namespace JewShop.Shared.Dtos
{
    public class ForgotPasswordRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
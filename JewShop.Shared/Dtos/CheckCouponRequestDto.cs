using System.ComponentModel.DataAnnotations;

namespace JewShop.Shared.Dtos
{
    public class CheckCouponRequestDto
    {
        [Required]
        [StringLength(120)]
        public string Code { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal OrderAmount { get; set; }
    }
}

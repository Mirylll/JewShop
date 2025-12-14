using System.ComponentModel.DataAnnotations;

namespace JewShop.Shared.Dtos
{
    public class CouponDto
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Mã giảm giá không được để trống")]
        public string Code { get; set; } = string.Empty;
        
        [Required]
        public string Type { get; set; } = "percent"; // percent or fixed
        
        [Range(0, double.MaxValue, ErrorMessage = "Giá trị phải lớn hơn 0")]
        public decimal Value { get; set; }
        
        public bool Active { get; set; } = true;
        
        public DateTime? ExpiryDate { get; set; }

        public int UsageLimit { get; set; } = 0; // 0 means unlimited

        public int UsedCount { get; set; } = 0;
    }
}

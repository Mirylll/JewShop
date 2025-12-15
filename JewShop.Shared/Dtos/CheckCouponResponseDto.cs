namespace JewShop.Shared.Dtos
{
    public class CheckCouponResponseDto
    {
        public bool IsValid { get; set; }

        public string Message { get; set; } = string.Empty;

        public CouponDto? Coupon { get; set; }

        public decimal? DiscountAmount { get; set; }
    }
}

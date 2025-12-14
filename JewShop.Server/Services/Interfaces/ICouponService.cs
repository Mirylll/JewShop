using JewShop.Shared.Dtos;

namespace JewShop.Server.Services.Interfaces
{
    public interface ICouponService
    {
        Task<List<CouponDto>> GetAllCouponsAsync();
        Task<CouponDto?> GetCouponByIdAsync(int id);
        Task<CouponDto?> GetCouponByCodeAsync(string code);
        Task<CouponDto> CreateCouponAsync(CouponDto dto);
        Task<bool> UpdateCouponAsync(int id, CouponDto dto);
        Task<bool> DeleteCouponAsync(int id);
        Task<CouponDto?> ValidateCouponAsync(string code, decimal orderValue);
    }
}

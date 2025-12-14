using JewShop.Shared.Dtos;

namespace JewShop.Server.Services.Interfaces
{
    public interface ICouponService
    {
        Task<List<CouponDto>> GetAllAsync();
        Task<CouponDto?> GetByIdAsync(int id);
        Task<CouponDto> CreateAsync(CreateCouponDto dto);
        Task<bool> UpdateAsync(int id, UpdateCouponDto dto);
        Task<bool> DeleteAsync(int id);
        Task<CheckCouponResponseDto> CheckAsync(CheckCouponRequestDto dto);
    }
}

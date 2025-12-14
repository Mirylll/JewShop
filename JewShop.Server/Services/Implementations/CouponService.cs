using JewShop.Server.Data;
using JewShop.Server.Models;
using JewShop.Server.Services.Interfaces;
using JewShop.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace JewShop.Server.Services.Implementations
{
    public class CouponService : ICouponService
    {
        private readonly ApplicationDbContext _context;

        public CouponService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CouponDto>> GetAllCouponsAsync()
        {
            return await _context.Coupons
                .Select(c => new CouponDto
                {
                    Id = c.Id,
                    Code = c.Code ?? "",
                    Type = c.Type ?? "percent",
                    Value = c.Value,
                    Active = c.Active,
                    ExpiryDate = c.ExpiryDate,
                    UsageLimit = c.UsageLimit,
                    UsedCount = c.UsedCount
                })
                .ToListAsync();
        }

        public async Task<CouponDto?> GetCouponByIdAsync(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null) return null;

            return new CouponDto
            {
                Id = coupon.Id,
                Code = coupon.Code ?? "",
                Type = coupon.Type ?? "percent",
                Value = coupon.Value,
                Active = coupon.Active,
                ExpiryDate = coupon.ExpiryDate,
                UsageLimit = coupon.UsageLimit,
                UsedCount = coupon.UsedCount
            };
        }

        public async Task<CouponDto?> GetCouponByCodeAsync(string code)
        {
            var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code == code);
            if (coupon == null) return null;

            return new CouponDto
            {
                Id = coupon.Id,
                Code = coupon.Code ?? "",
                Type = coupon.Type ?? "percent",
                Value = coupon.Value,
                Active = coupon.Active,
                ExpiryDate = coupon.ExpiryDate,
                UsageLimit = coupon.UsageLimit,
                UsedCount = coupon.UsedCount
            };
        }

        public async Task<CouponDto> CreateCouponAsync(CouponDto dto)
        {
            var coupon = new Coupon
            {
                Code = dto.Code,
                Type = dto.Type,
                Value = dto.Value,
                Active = dto.Active,
                ExpiryDate = dto.ExpiryDate,
                UsageLimit = dto.UsageLimit,
                UsedCount = 0
            };

            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync();

            dto.Id = coupon.Id;
            return dto;
        }

        public async Task<bool> UpdateCouponAsync(int id, CouponDto dto)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null) return false;

            coupon.Code = dto.Code;
            coupon.Type = dto.Type;
            coupon.Value = dto.Value;
            coupon.Active = dto.Active;
            coupon.ExpiryDate = dto.ExpiryDate;
            coupon.UsageLimit = dto.UsageLimit;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCouponAsync(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null) return false;

            _context.Coupons.Remove(coupon);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<CouponDto?> ValidateCouponAsync(string code, decimal orderValue)
        {
            var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code == code);
            if (coupon == null || !coupon.Active) return null;
            if (coupon.ExpiryDate.HasValue && coupon.ExpiryDate.Value < DateTime.UtcNow) return null;

            return new CouponDto
            {
                Id = coupon.Id,
                Code = coupon.Code ?? "",
                Type = coupon.Type ?? "percent",
                Value = coupon.Value,
                Active = coupon.Active,
                ExpiryDate = coupon.ExpiryDate,
                UsageLimit = coupon.UsageLimit,
                UsedCount = coupon.UsedCount
            };
        }
    }
}

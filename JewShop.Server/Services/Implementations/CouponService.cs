using System;
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

        public async Task<List<CouponDto>> GetAllAsync()
        {
            return await _context.Coupons
                .AsNoTracking()
                .OrderByDescending(c => c.StartsAt)
                .Select(c => new CouponDto
                {
                    Id = c.Id,
                    Code = c.Code,
                    Description = c.Description,
                    Type = c.Type,
                    Value = c.Value,
                    MinOrderValue = c.MinOrderValue,
                    UsageLimit = c.UsageLimit,
                    UsedCount = c.UsedCount,
                    StartsAt = c.StartsAt,
                    ExpiresAt = c.ExpiresAt,
                    Active = c.Active
                })
                .ToListAsync();
        }

        public async Task<CouponDto?> GetByIdAsync(int id)
        {
            var coupon = await _context.Coupons.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            return coupon == null ? null : MapToDto(coupon);
        }

        public async Task<CouponDto> CreateAsync(CreateCouponDto dto)
        {
            await EnsureCodeIsUniqueAsync(dto.Code);
            ValidateDates(dto.StartsAt, dto.ExpiresAt);

            var coupon = new Coupon
            {
                Code = NormalizeCode(dto.Code),
                Description = dto.Description?.Trim(),
                Type = NormalizeType(dto.Type),
                Value = dto.Value,
                MinOrderValue = dto.MinOrderValue,
                UsageLimit = dto.UsageLimit,
                UsedCount = 0,
                StartsAt = dto.StartsAt,
                ExpiresAt = dto.ExpiresAt,
                Active = dto.Active
            };

            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync();

            return MapToDto(coupon);
        }

        public async Task<bool> UpdateAsync(int id, UpdateCouponDto dto)
        {
            var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Id == id);
            if (coupon == null)
            {
                return false;
            }

            await EnsureCodeIsUniqueAsync(dto.Code, id);
            ValidateDates(dto.StartsAt, dto.ExpiresAt);

            coupon.Code = NormalizeCode(dto.Code);
            coupon.Description = dto.Description?.Trim();
            coupon.Type = NormalizeType(dto.Type);
            coupon.Value = dto.Value;
            coupon.MinOrderValue = dto.MinOrderValue;
            coupon.UsageLimit = dto.UsageLimit;
            coupon.StartsAt = dto.StartsAt;
            coupon.ExpiresAt = dto.ExpiresAt;
            coupon.Active = dto.Active;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Id == id);
            if (coupon == null)
            {
                return false;
            }

            _context.Coupons.Remove(coupon);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<CheckCouponResponseDto> CheckAsync(CheckCouponRequestDto dto)
        {
            var normalizedCode = NormalizeCode(dto.Code);
            var coupon = await _context.Coupons.AsNoTracking().FirstOrDefaultAsync(c => c.Code == normalizedCode);

            if (coupon == null)
            {
                return InvalidResult("Mã giảm giá không tồn tại.");
            }

            var now = DateTime.UtcNow;
            if (!coupon.Active)
            {
                return InvalidResult("Mã giảm giá đang bị khóa.");
            }

            if (coupon.StartsAt > now)
            {
                return InvalidResult("Chương trình chưa bắt đầu.");
            }

            if (coupon.ExpiresAt < now)
            {
                return InvalidResult("Mã giảm giá đã hết hạn.");
            }

            if (coupon.UsageLimit > 0 && coupon.UsedCount >= coupon.UsageLimit)
            {
                return InvalidResult("Mã giảm giá đã đạt giới hạn sử dụng.");
            }

            if (dto.OrderAmount < coupon.MinOrderValue)
            {
                return InvalidResult($"Đơn hàng phải tối thiểu {coupon.MinOrderValue:C}.");
            }

            var discount = CalculateDiscount(dto.OrderAmount, coupon);
            return new CheckCouponResponseDto
            {
                IsValid = true,
                Message = "Mã giảm giá hợp lệ.",
                Coupon = MapToDto(coupon),
                DiscountAmount = discount
            };
        }

        private static decimal CalculateDiscount(decimal orderAmount, Coupon coupon)
        {
            if (coupon.Type == "percent")
            {
                var percentValue = orderAmount * (coupon.Value / 100m);
                return Math.Min(orderAmount, decimal.Round(percentValue, 2));
            }

            return Math.Min(orderAmount, decimal.Round(coupon.Value, 2));
        }

        private static void ValidateDates(DateTime startsAt, DateTime expiresAt)
        {
            if (expiresAt <= startsAt)
            {
                throw new ArgumentException("Ngày kết thúc phải lớn hơn ngày bắt đầu.");
            }
        }

        private async Task EnsureCodeIsUniqueAsync(string code, int? ignoreId = null)
        {
            var normalized = NormalizeCode(code);
            var query = _context.Coupons.AsQueryable().Where(c => c.Code == normalized);
            if (ignoreId.HasValue)
            {
                query = query.Where(c => c.Id != ignoreId.Value);
            }

            if (await query.AnyAsync())
            {
                throw new ArgumentException("Mã giảm giá đã tồn tại.");
            }
        }

        private static string NormalizeCode(string code)
        {
            return code?.Trim().ToUpperInvariant() ?? string.Empty;
        }

        private static string NormalizeType(string type)
        {
            var normalized = type?.Trim().ToLowerInvariant();
            return normalized is "fixed" or "percent" ? normalized : "percent";
        }

        private static CouponDto MapToDto(Coupon coupon)
        {
            return new CouponDto
            {
                Id = coupon.Id,
                Code = coupon.Code,
                Description = coupon.Description,
                Type = coupon.Type,
                Value = coupon.Value,
                MinOrderValue = coupon.MinOrderValue,
                UsageLimit = coupon.UsageLimit,
                UsedCount = coupon.UsedCount,
                StartsAt = coupon.StartsAt,
                ExpiresAt = coupon.ExpiresAt,
                Active = coupon.Active
            };
        }

        private static CheckCouponResponseDto InvalidResult(string message)
        {
            return new CheckCouponResponseDto
            {
                IsValid = false,
                Message = message
            };
        }
    }
}

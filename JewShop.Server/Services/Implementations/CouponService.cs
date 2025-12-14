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
            var coupons = await _context.Coupons
                .AsNoTracking()
                .OrderByDescending(c => c.StartsAt)
                .ThenByDescending(c => c.Id)
                .ToListAsync();

            return coupons.Select(MapToDto).ToList();
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
            if (coupon == null) return false;

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
            if (coupon == null) return false;

            _context.Coupons.Remove(coupon);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<CheckCouponResponseDto> CheckAsync(CheckCouponRequestDto dto)
        {
            var normalizedCode = NormalizeCode(dto.Code);
            var coupon = await _context.Coupons.AsNoTracking().FirstOrDefaultAsync(c => c.Code == normalizedCode);

            if (coupon == null) return InvalidResult("Mã giảm giá không tồn tại.");

            var now = DateTime.UtcNow;
            var startsAt = coupon.StartsAt ?? DateTime.MinValue;
            var expiresAt = coupon.ExpiresAt ?? DateTime.MaxValue;
            var usageLimit = coupon.UsageLimit ?? 0;
            var usedCount = coupon.UsedCount ?? 0;
            var minOrderValue = coupon.MinOrderValue ?? 0m;

            if (!coupon.Active) return InvalidResult("Mã giảm giá đang bị khóa.");
            if (startsAt > now) return InvalidResult("Chương trình chưa bắt đầu.");
            if (expiresAt < now) return InvalidResult("Mã giảm giá đã hết hạn.");
            if (usageLimit > 0 && usedCount >= usageLimit) return InvalidResult("Mã giảm giá đã đạt giới hạn sử dụng.");
            if (dto.OrderAmount < minOrderValue) return InvalidResult($"Đơn hàng phải tối thiểu {minOrderValue:C}.");

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
            var couponValue = coupon.Value ?? 0m;

            if (coupon.Type == "percent")
            {
                var percentValue = orderAmount * (couponValue / 100m);
                return Math.Min(orderAmount, decimal.Round(percentValue, 2));
            }

            return Math.Min(orderAmount, decimal.Round(couponValue, 2));
        }

        private static void ValidateDates(DateTime? startsAt, DateTime? expiresAt)
        {
            var start = startsAt.GetValueOrDefault();
            var end = expiresAt.GetValueOrDefault();
            if (end <= start) throw new ArgumentException("Ngày kết thúc phải lớn hơn ngày bắt đầu.");
        }

        private async Task EnsureCodeIsUniqueAsync(string code, int? ignoreId = null)
        {
            var normalized = NormalizeCode(code);
            var query = _context.Coupons.AsQueryable().Where(c => c.Code == normalized);
            if (ignoreId.HasValue) query = query.Where(c => c.Id != ignoreId.Value);

            if (await query.AnyAsync()) throw new ArgumentException("Mã giảm giá đã tồn tại.");
        }

        private static string NormalizeCode(string code) => code?.Trim().ToUpperInvariant() ?? string.Empty;
        private static string NormalizeType(string type)
        {
            var normalized = type?.Trim().ToLowerInvariant();
            return normalized is "fixed" or "percent" ? normalized : "percent";
        }

        private static CouponDto MapToDto(Coupon coupon) => new()
        {
            Id = coupon.Id,
            Code = coupon.Code,
            Description = coupon.Description,
            Type = coupon.Type,
            Value = coupon.Value ?? 0m,
            MinOrderValue = coupon.MinOrderValue ?? 0m,
            UsageLimit = coupon.UsageLimit ?? 0,
            UsedCount = coupon.UsedCount ?? 0,
            StartsAt = coupon.StartsAt ?? DateTime.UtcNow,
            ExpiresAt = coupon.ExpiresAt ?? DateTime.UtcNow,
            Active = coupon.Active
        };

        private static CheckCouponResponseDto InvalidResult(string message) => new()
        {
            IsValid = false,
            Message = message
        };
    }
}

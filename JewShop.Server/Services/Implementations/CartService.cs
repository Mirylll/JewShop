using JewShop.Server.Data;
using JewShop.Server.Models;
using JewShop.Server.Services.Interfaces;
using JewShop.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace JewShop.Server.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;

        public CartService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CartItemDto>> GetCartItemsAsync(string sessionId)
        {
            return await _context.CartItems
                .Include(c => c.Variant)
                .ThenInclude(v => v.Product)
                .Where(c => c.SessionId == sessionId)
                .Select(c => new CartItemDto
                {
                    Id = c.Id,
                    VariantId = c.VariantId,
                    ProductId = c.Variant.ProductId,
                    ProductName = c.Variant.Product.Name,
                    ProductImage = c.Variant.Product.ThumbnailUrl,
                    Size = c.Variant.Size,
                    Color = c.Variant.Color,
                    Price = c.Variant.Price ?? c.Variant.Product.BasePrice,
                    Quantity = c.Quantity,
                    Stock = c.Variant.Stock
                })
                .ToListAsync();
        }

        public async Task<CartItemDto?> AddToCartAsync(AddToCartDto dto)
        {
            var variant = await _context.ProductVariants
                .Include(v => v.Product)
                .FirstOrDefaultAsync(v => v.Id == dto.VariantId);

            if (variant == null) return null;

            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.SessionId == dto.SessionId && c.VariantId == dto.VariantId);

            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
                await _context.SaveChangesAsync();
                
                return new CartItemDto
                {
                    Id = existingItem.Id,
                    VariantId = existingItem.VariantId,
                    ProductId = variant.ProductId,
                    ProductName = variant.Product.Name,
                    ProductImage = variant.Product.ThumbnailUrl,
                    Size = variant.Size,
                    Color = variant.Color,
                    Price = variant.Price ?? variant.Product.BasePrice,
                    Quantity = existingItem.Quantity,
                    Stock = variant.Stock
                };
            }

            var newItem = new CartItem
            {
                SessionId = dto.SessionId,
                VariantId = dto.VariantId,
                Quantity = dto.Quantity
            };

            _context.CartItems.Add(newItem);
            await _context.SaveChangesAsync();

            return new CartItemDto
            {
                Id = newItem.Id,
                VariantId = newItem.VariantId,
                ProductId = variant.ProductId,
                ProductName = variant.Product.Name,
                ProductImage = variant.Product.ThumbnailUrl,
                Size = variant.Size,
                Color = variant.Color,
                Price = variant.Price ?? variant.Product.BasePrice,
                Quantity = newItem.Quantity,
                Stock = variant.Stock
            };
        }

        public async Task<bool> UpdateQuantityAsync(int cartItemId, int quantity)
        {
            var item = await _context.CartItems.FindAsync(cartItemId);
            if (item == null) return false;

            if (quantity <= 0)
            {
                _context.CartItems.Remove(item);
            }
            else
            {
                item.Quantity = quantity;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveFromCartAsync(int cartItemId)
        {
            var item = await _context.CartItems.FindAsync(cartItemId);
            if (item == null) return false;

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ClearCartAsync(string sessionId)
        {
            var items = await _context.CartItems
                .Where(c => c.SessionId == sessionId)
                .ToListAsync();

            if (!items.Any()) return false;

            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
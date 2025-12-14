using JewShop.Server.Data;
using JewShop.Server.Models;
using JewShop.Server.Services.Interfaces;
using JewShop.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace JewShop.Server.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _context.Products
                .Include(p => p.Images)
                .Include(p => p.Variants)
                .Where(p => p.IsActive)
                .ToListAsync();

            return products.Select(MapToDto);
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Images)
                .Include(p => p.Variants)
                .FirstOrDefaultAsync(p => p.Id == id);

            return product == null ? null : MapToDto(product);
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Category = dto.Category,
                Material = dto.Material,
                Gemstone = dto.Gemstone,
                BasePrice = dto.BasePrice,
                Weight = dto.Weight,
                ThumbnailUrl = dto.ThumbnailUrl,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return MapToDto(product);
        }

        public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto dto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return null;

            if (dto.Name != null) product.Name = dto.Name;
            if (dto.Description != null) product.Description = dto.Description;
            if (dto.Category != null) product.Category = dto.Category;
            if (dto.Material != null) product.Material = dto.Material;
            if (dto.Gemstone != null) product.Gemstone = dto.Gemstone;
            if (dto.BasePrice.HasValue) product.BasePrice = dto.BasePrice.Value;
            if (dto.Weight.HasValue) product.Weight = dto.Weight;
            if (dto.ThumbnailUrl != null) product.ThumbnailUrl = dto.ThumbnailUrl;
            if (dto.IsActive.HasValue) product.IsActive = dto.IsActive.Value;

            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetProductByIdAsync(id);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        // Image Management
        public async Task<ProductImageDto> AddProductImageAsync(int productId, CreateProductImageDto dto)
        {
            var image = new ProductImage
            {
                ProductId = productId,
                ImageUrl = dto.ImageUrl,
                SortOrder = dto.SortOrder,
                IsPrimary = dto.IsPrimary
            };

            _context.ProductImages.Add(image);
            await _context.SaveChangesAsync();

            return new ProductImageDto
            {
                Id = image.Id,
                ImageUrl = image.ImageUrl,
                SortOrder = image.SortOrder,
                IsPrimary = image.IsPrimary
            };
        }

        public async Task<bool> DeleteProductImageAsync(int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId);
            if (image == null) return false;

            _context.ProductImages.Remove(image);
            await _context.SaveChangesAsync();
            return true;
        }

        // Variant Management
        public async Task<ProductVariantDto> AddProductVariantAsync(int productId, CreateProductVariantDto dto)
        {
            var variant = new ProductVariant
            {
                ProductId = productId,
                Sku = dto.Sku,
                Size = dto.Size,
                Color = dto.Color,
                Length = dto.Length,
                Price = dto.Price,
                Stock = dto.Stock,
                ReorderLevel = dto.ReorderLevel
            };

            _context.ProductVariants.Add(variant);
            await _context.SaveChangesAsync();

            return new ProductVariantDto
            {
                Id = variant.Id,
                Sku = variant.Sku,
                Size = variant.Size,
                Color = variant.Color,
                Length = variant.Length,
                Price = variant.Price,
                Stock = variant.Stock,
                ReorderLevel = variant.ReorderLevel
            };
        }

        public async Task<ProductVariantDto?> UpdateProductVariantAsync(int variantId, UpdateProductVariantDto dto)
        {
            var variant = await _context.ProductVariants.FindAsync(variantId);
            if (variant == null) return null;

            if (dto.Sku != null) variant.Sku = dto.Sku;
            if (dto.Size != null) variant.Size = dto.Size;
            if (dto.Color != null) variant.Color = dto.Color;
            if (dto.Length != null) variant.Length = dto.Length;
            if (dto.Price.HasValue) variant.Price = dto.Price;
            variant.Stock = dto.Stock;
            variant.ReorderLevel = dto.ReorderLevel;

            await _context.SaveChangesAsync();

            return new ProductVariantDto
            {
                Id = variant.Id,
                Sku = variant.Sku,
                Size = variant.Size,
                Color = variant.Color,
                Length = variant.Length,
                Price = variant.Price,
                Stock = variant.Stock,
                ReorderLevel = variant.ReorderLevel
            };
        }

        public async Task<bool> DeleteProductVariantAsync(int variantId)
        {
            var variant = await _context.ProductVariants.FindAsync(variantId);
            if (variant == null) return false;

            _context.ProductVariants.Remove(variant);
            await _context.SaveChangesAsync();
            return true;
        }

        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Category = product.Category,
                Material = product.Material,
                Gemstone = product.Gemstone,
                BasePrice = product.BasePrice,
                Weight = product.Weight,
                ThumbnailUrl = product.ThumbnailUrl,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt,
                Images = product.Images?.Select(i => new ProductImageDto
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl,
                    SortOrder = i.SortOrder,
                    IsPrimary = i.IsPrimary
                }).ToList() ?? new List<ProductImageDto>(),
                Variants = product.Variants?.Select(v => new ProductVariantDto
                {
                    Id = v.Id,
                    Sku = v.Sku,
                    Size = v.Size,
                    Color = v.Color,
                    Length = v.Length,
                    Price = v.Price,
                    Stock = v.Stock,
                    ReorderLevel = v.ReorderLevel
                }).ToList() ?? new List<ProductVariantDto>()
            };
        }
    }
}

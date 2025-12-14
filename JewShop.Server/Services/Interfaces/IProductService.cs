using JewShop.Shared.Dtos;

namespace JewShop.Server.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<ProductDto> CreateProductAsync(CreateProductDto dto);
        Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto dto);
        Task<bool> DeleteProductAsync(int id);

        // Image Management
        Task<ProductImageDto> AddProductImageAsync(int productId, CreateProductImageDto dto);
        Task<bool> DeleteProductImageAsync(int imageId);

        // Variant Management
        Task<ProductVariantDto> AddProductVariantAsync(int productId, CreateProductVariantDto dto);
        Task<ProductVariantDto?> UpdateProductVariantAsync(int variantId, UpdateProductVariantDto dto);
        Task<bool> DeleteProductVariantAsync(int variantId);
    }
}

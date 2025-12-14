namespace JewShop.Shared.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? Material { get; set; }
        public string? Gemstone { get; set; }
        public decimal BasePrice { get; set; }
        public decimal? Weight { get; set; }
        public string? ThumbnailUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ProductImageDto> Images { get; set; } = new();
        public List<ProductVariantDto> Variants { get; set; } = new();
    }

    public class ProductImageDto
    {
        public int Id { get; set; }
        public string? ImageUrl { get; set; }
        public int SortOrder { get; set; }
        public bool IsPrimary { get; set; }
    }

    public class ProductVariantDto
    {
        public int Id { get; set; }
        public string? Sku { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public string? Length { get; set; }
        public decimal? Price { get; set; }
        public int Stock { get; set; }
        public int ReorderLevel { get; set; }
    }
}

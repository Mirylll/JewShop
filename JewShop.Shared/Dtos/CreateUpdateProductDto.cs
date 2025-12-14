namespace JewShop.Shared.Dtos
{
    public class CreateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? Material { get; set; }
        public string? Gemstone { get; set; }
        public decimal BasePrice { get; set; }
        public decimal? Weight { get; set; }
        public string? ThumbnailUrl { get; set; }
    }

    public class UpdateProductDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? Material { get; set; }
        public string? Gemstone { get; set; }
        public decimal? BasePrice { get; set; }
        public decimal? Weight { get; set; }
        public string? ThumbnailUrl { get; set; }
        public bool? IsActive { get; set; }
    }
}

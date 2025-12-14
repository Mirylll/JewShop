namespace JewShop.Shared.Dtos
{
    public class UpdateProductVariantDto
    {
        public string? Sku { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public string? Length { get; set; }
        public decimal? Price { get; set; }
        public int Stock { get; set; }
        public int ReorderLevel { get; set; }
    }
}

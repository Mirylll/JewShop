using System.ComponentModel.DataAnnotations;

namespace JewShop.Shared.Dtos
{
    public class CartItemDto
    {
        public int Id { get; set; }
        public int VariantId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductImage { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Price * Quantity;
        public int Stock { get; set; }
    }

    public class AddToCartDto
    {
        [Required]
        public string SessionId { get; set; } = string.Empty;

        [Required]
        public int VariantId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; } = 1;
    }

    public class UpdateCartItemDto
    {
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
using JewShop.Shared.Dtos;

namespace JewShop.Server.Services.Interfaces
{
    public interface ICartService
    {
        Task<List<CartItemDto>> GetCartItemsAsync(string sessionId);
        Task<CartItemDto?> AddToCartAsync(AddToCartDto dto);
        Task<bool> UpdateQuantityAsync(int cartItemId, int quantity);
        Task<bool> RemoveFromCartAsync(int cartItemId);
        Task<bool> ClearCartAsync(string sessionId);
    }
}
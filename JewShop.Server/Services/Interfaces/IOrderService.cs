using JewShop.Shared.Dtos;

namespace JewShop.Server.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto?> CreateOrderAsync(CreateOrderDto dto);
        Task<List<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto?> GetOrderByIdAsync(int id);
        Task<bool> UpdateOrderStatusAsync(int id, string status);
    }
}
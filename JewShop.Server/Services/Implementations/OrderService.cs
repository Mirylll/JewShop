using JewShop.Server.Data;
using JewShop.Server.Models;
using JewShop.Server.Services.Interfaces;
using JewShop.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace JewShop.Server.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICartService _cartService;

        public OrderService(ApplicationDbContext context, ICartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        public async Task<OrderDto?> CreateOrderAsync(CreateOrderDto dto, int? userId = null)
        {
            var cartItems = await _cartService.GetCartItemsAsync(dto.SessionId);
            if (!cartItems.Any()) return null;

            var subtotal = cartItems.Sum(i => i.TotalPrice);
            var shippingFee = 30000m; // Flat rate for now
            var discount = 0m;
            int? couponId = null;

            if (!string.IsNullOrEmpty(dto.CouponCode))
            {
                var coupon = await _context.Coupons
                    .FirstOrDefaultAsync(c => c.Code == dto.CouponCode && c.Active);
                
                if (coupon != null && (!coupon.ExpiryDate.HasValue || coupon.ExpiryDate > DateTime.UtcNow))
                {
                    // Check usage limit
                    if (coupon.UsageLimit > 0 && coupon.UsedCount >= coupon.UsageLimit)
                    {
                        throw new Exception("Mã giảm giá đã hết lượt sử dụng.");
                    }

                    couponId = coupon.Id;
                    if (coupon.Type == "percent")
                    {
                        discount = subtotal * (coupon.Value / 100);
                    }
                    else
                    {
                        discount = coupon.Value;
                    }

                    // Increment used count
                    coupon.UsedCount++;
                }
            }

            // Validate stock BEFORE creating order
            foreach (var item in cartItems)
            {
                var variant = await _context.ProductVariants.FindAsync(item.VariantId);
                if (variant == null || variant.Stock < item.Quantity)
                {
                    var productName = variant?.Product?.Name ?? "Unknown";
                    throw new Exception($"Sản phẩm '{productName}' không đủ hàng trong kho. Còn lại: {variant?.Stock ?? 0}, yêu cầu: {item.Quantity}");
                }
            }

            var order = new Order
            {
                UserId = userId, // SET UserId here!
                OrderCode = $"ORD-{DateTime.UtcNow.Ticks}",
                CustomerName = dto.CustomerName,
                CustomerPhone = dto.CustomerPhone,
                ShippingAddress = dto.ShippingAddress,
                Subtotal = subtotal,
                ShippingFee = shippingFee,
                DiscountAmount = discount,
                TotalAmount = subtotal + shippingFee - discount,
                Note = dto.Note,
                Status = "pending",
                PaymentStatus = "unpaid",
                CreatedAt = DateTime.UtcNow,
                CouponId = couponId
            };
            
            Console.WriteLine($"[DEBUG OrderService] Creating order with UserId: {userId}, OrderCode: {order.OrderCode}");

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Deduct stock and create order items
            foreach (var item in cartItems)
            {
                var variant = await _context.ProductVariants.FindAsync(item.VariantId);
                if (variant != null)
                {
                    variant.Stock -= item.Quantity;
                }

                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    VariantId = item.VariantId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price,
                    TotalPrice = item.TotalPrice
                };
                _context.OrderItems.Add(orderItem);
            }

            await _context.SaveChangesAsync();
            await _cartService.ClearCartAsync(dto.SessionId);

            return await GetOrderByIdAsync(order.Id);
        }

        public async Task<List<OrderDto>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Variant)
                .ThenInclude(v => v.Product)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => MapToDto(o))
                .ToListAsync();
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Variant)
                .ThenInclude(v => v.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            return order == null ? null : MapToDto(order);
        }

        public async Task<bool> UpdateOrderStatusAsync(int id, string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return false;

            // Validate transition
            if (!IsValidTransition(order.Status, status))
            {
                return false;
            }

            order.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        private bool IsValidTransition(string currentStatus, string newStatus)
        {
            if (currentStatus == newStatus) return true;

            return currentStatus switch
            {
                "pending" => newStatus == "confirmed" || newStatus == "cancelled",
                "confirmed" => newStatus == "shipping" || newStatus == "cancelled",
                "shipping" => newStatus == "delivered" || newStatus == "cancelled",
                "delivered" => false, // Terminal state
                "cancelled" => false, // Terminal state
                _ => false
            };
        }

        private static OrderDto MapToDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                OrderCode = order.OrderCode,
                CustomerName = order.CustomerName,
                CustomerPhone = order.CustomerPhone,
                ShippingAddress = order.ShippingAddress,
                Subtotal = order.Subtotal,
                DiscountAmount = order.DiscountAmount,
                ShippingFee = order.ShippingFee,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                PaymentStatus = order.PaymentStatus,
                CreatedAt = order.CreatedAt,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    VariantId = oi.VariantId ?? 0,
                    ProductName = oi.Variant?.Product?.Name ?? "Unknown Product",
                    ProductImage = oi.Variant?.Product?.ThumbnailUrl,
                    Size = oi.Variant?.Size,
                    Color = oi.Variant?.Color,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    TotalPrice = oi.TotalPrice
                }).ToList()
            };
        }
    }
}

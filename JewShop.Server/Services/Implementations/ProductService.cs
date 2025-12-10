using JewShop.Server.Data;
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

        public async Task<List<ProductDto>> GetAllAsync()
        {
            return await _context.Products
                .Select(x => new ProductDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    ImageUrl = x.ImageUrl
                }).ToListAsync();
        }
    }
}

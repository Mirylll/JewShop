using JewShop.Server.Data;
using JewShop.Server.Models;
using JewShop.Server.Services.Interfaces;
using JewShop.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace JewShop.Server.Services.Implementations
{
    public class ImportService : IImportService
    {
        private readonly ApplicationDbContext _context;

        public ImportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ImportDto>> GetAllImportsAsync()
        {
            return await _context.Imports
                .Include(i => i.Supplier)
                .Include(i => i.ImportItems)
                    .ThenInclude(ii => ii.Variant)
                    .ThenInclude(v => v.Product)
                .OrderByDescending(i => i.CreatedAt)
                .Select(i => MapToDto(i))
                .ToListAsync();
        }

        public async Task<ImportDto?> GetImportByIdAsync(int id)
        {
            var import = await _context.Imports
                .Include(i => i.Supplier)
                .Include(i => i.ImportItems)
                    .ThenInclude(ii => ii.Variant)
                    .ThenInclude(v => v.Product)
                .FirstOrDefaultAsync(i => i.Id == id);

            return import == null ? null : MapToDto(import);
        }

        public async Task<ImportDto> CreateImportAsync(CreateImportDto dto)
        {
            // Validate supplier exists
            var supplier = await _context.Suppliers.FindAsync(dto.SupplierId);
            if (supplier == null)
            {
                throw new Exception("Nhà cung cấp không tồn tại");
            }

            // Calculate total
            decimal totalAmount = 0;
            foreach (var item in dto.Items)
            {
                totalAmount += item.Quantity * item.UnitPrice;
            }

            var import = new Import
            {
                ImportCode = $"IMP-{DateTime.UtcNow.Ticks}",
                SupplierId = dto.SupplierId,
                ImportDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                Status = "pending",
                Note = dto.Note,
                CreatedAt = DateTime.UtcNow
            };

            _context.Imports.Add(import);
            await _context.SaveChangesAsync();

            // Add import items
            foreach (var itemDto in dto.Items)
            {
                var variant = await _context.ProductVariants.FindAsync(itemDto.VariantId);
                if (variant == null) continue;

                var importItem = new ImportItem
                {
                    ImportId = import.Id,
                    VariantId = itemDto.VariantId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = itemDto.UnitPrice,
                    TotalPrice = itemDto.Quantity * itemDto.UnitPrice
                };

                _context.ImportItems.Add(importItem);
            }

            await _context.SaveChangesAsync();

            return (await GetImportByIdAsync(import.Id))!;
        }

        public async Task<bool> CompleteImportAsync(int id)
        {
            var import = await _context.Imports
                .Include(i => i.ImportItems)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (import == null || import.Status != "pending")
            {
                return false;
            }

            // Update stock for all items
            foreach (var item in import.ImportItems)
            {
                var variant = await _context.ProductVariants.FindAsync(item.VariantId);
                if (variant != null)
                {
                    variant.Stock += item.Quantity;
                }
            }

            import.Status = "completed";
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CancelImportAsync(int id)
        {
            var import = await _context.Imports.FindAsync(id);
            if (import == null || import.Status != "pending")
            {
                return false;
            }

            import.Status = "cancelled";
            await _context.SaveChangesAsync();

            return true;
        }

        private static ImportDto MapToDto(Import import)
        {
            return new ImportDto
            {
                Id = import.Id,
                ImportCode = import.ImportCode,
                SupplierId = import.SupplierId,
                SupplierName = import.Supplier?.Name,
                ImportDate = import.ImportDate,
                TotalAmount = import.TotalAmount,
                Status = import.Status,
                Note = import.Note,
                CreatedAt = import.CreatedAt,
                ImportItems = import.ImportItems.Select(ii => new ImportItemDto
                {
                    Id = ii.Id,
                    VariantId = ii.VariantId,
                    ProductName = ii.Variant?.Product?.Name,
                    Size = ii.Variant?.Size,
                    Color = ii.Variant?.Color,
                    Quantity = ii.Quantity,
                    UnitPrice = ii.UnitPrice,
                    TotalPrice = ii.TotalPrice
                }).ToList()
            };
        }
    }
}

using JewShop.Shared.Dtos;

namespace JewShop.Server.Services.Interfaces
{
    public interface IImportService
    {
        Task<List<ImportDto>> GetAllImportsAsync();
        Task<ImportDto?> GetImportByIdAsync(int id);
        Task<ImportDto> CreateImportAsync(CreateImportDto dto);
        Task<bool> CompleteImportAsync(int id);
        Task<bool> CancelImportAsync(int id);
    }
}

using JewShop.Server.Services.Interfaces;
using JewShop.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JewShop.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class ImportsController : ControllerBase
    {
        private readonly IImportService _importService;

        public ImportsController(IImportService importService)
        {
            _importService = importService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ImportDto>>> GetAll()
        {
            var imports = await _importService.GetAllImportsAsync();
            return Ok(imports);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ImportDto>> GetById(int id)
        {
            var import = await _importService.GetImportByIdAsync(id);
            if (import == null) return NotFound();
            return Ok(import);
        }

        [HttpPost]
        public async Task<ActionResult<ImportDto>> Create(CreateImportDto dto)
        {
            try
            {
                var import = await _importService.CreateImportAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = import.Id }, import);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/complete")]
        public async Task<IActionResult> Complete(int id)
        {
            var success = await _importService.CompleteImportAsync(id);
            if (!success) return BadRequest(new { message = "Không thể hoàn thành phiếu nhập" });
            return Ok(new { message = "Đã hoàn thành phiếu nhập và cập nhật kho" });
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var success = await _importService.CancelImportAsync(id);
            if (!success) return BadRequest(new { message = "Không thể hủy phiếu nhập" });
            return Ok(new { message = "Đã hủy phiếu nhập" });
        }
    }
}

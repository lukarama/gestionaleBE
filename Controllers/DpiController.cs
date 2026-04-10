using Gestionale.Api.DTOs;
using Gestionale.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gestionale.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DpiController : ControllerBase
    {
        private readonly IDpiService _dpiService;

        public DpiController(IDpiService dpiService)
        {
            _dpiService = dpiService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DpiListDto>>> GetAll()
        {
            var dpis = await _dpiService.GetAllAsync();
            return Ok(dpis);
        }

        [HttpGet("attivi")]
        public async Task<ActionResult<IEnumerable<SelectOptionDto>>> GetAttivi()
        {
            var dpis = await _dpiService.GetAttiviAsync();
            return Ok(dpis);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DpiDetailDto>> GetById(int id)
        {
            var dpi = await _dpiService.GetByIdAsync(id);

            if (dpi == null)
                return NotFound(new { message = "DPI non trovato" });

            return Ok(dpi);
        }

        [HttpPost]
        public async Task<ActionResult<DpiDetailDto>> Create(CreateDpiDto dto)
        {
            var result = await _dpiService.CreateAsync(dto);

            if (!result.Success)
                return StatusCode(result.StatusCode ?? 400, new { message = result.Message });

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateDpiDto dto)
        {
            var result = await _dpiService.UpdateAsync(id, dto);

            if (!result.Success)
                return StatusCode(result.StatusCode ?? 400, new { message = result.Message });

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _dpiService.DeleteAsync(id);

            if (!result.Success)
                return StatusCode(result.StatusCode ?? 400, new { message = result.Message });

            return NoContent();
        }
    }
}
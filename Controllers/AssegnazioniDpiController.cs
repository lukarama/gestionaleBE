using Gestionale.Api.DTOs;
using Gestionale.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gestionale.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssegnazioniDpiController : ControllerBase
    {
        private readonly IAssegnazioniDpiService _assegnazioniDpiService;

        public AssegnazioniDpiController(IAssegnazioniDpiService assegnazioniDpiService)
        {
            _assegnazioniDpiService = assegnazioniDpiService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AssegnazioneDpiListDto>>> GetAll()
        {
            var assegnazioni = await _assegnazioniDpiService.GetAllAsync();
            return Ok(assegnazioni);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AssegnazioneDpiDetailDto>> GetById(int id)
        {
            var assegnazione = await _assegnazioniDpiService.GetByIdAsync(id);

            if (assegnazione == null)
                return NotFound(new { message = "Assegnazione DPI non trovata" });

            return Ok(assegnazione);
        }

        [HttpGet("dipendente/{dipendenteId}")]
        public async Task<ActionResult<IEnumerable<AssegnazioneDpiListDto>>> GetByDipendenteId(int dipendenteId)
        {
            var assegnazioni = await _assegnazioniDpiService.GetByDipendenteIdAsync(dipendenteId);
            return Ok(assegnazioni);
        }

        [HttpPost]
        public async Task<ActionResult<AssegnazioneDpiDetailDto>> Create(CreateAssegnazioneDpiDto dto)
        {
            var result = await _assegnazioniDpiService.CreateAsync(dto);

            if (!result.Success)
                return StatusCode(result.StatusCode ?? 400, new { message = result.Message });

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateAssegnazioneDpiDto dto)
        {
            var result = await _assegnazioniDpiService.UpdateAsync(id, dto);

            if (!result.Success)
                return StatusCode(result.StatusCode ?? 400, new { message = result.Message });

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _assegnazioniDpiService.DeleteAsync(id);

            if (!result.Success)
                return StatusCode(result.StatusCode ?? 400, new { message = result.Message });

            return NoContent();
        }
    }
}

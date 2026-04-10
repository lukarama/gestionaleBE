using Gestionale.Api.DTOs;
using Gestionale.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gestionale.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssegnazioniMezziController : ControllerBase
    {
        private readonly IAssegnazioniMezziService _assegnazioniMezziService;

        public AssegnazioniMezziController(IAssegnazioniMezziService assegnazioniMezziService)
        {
            _assegnazioniMezziService = assegnazioniMezziService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AssegnazioneMezzoListDto>>> GetAll()
        {
            var assegnazioni = await _assegnazioniMezziService.GetAllAsync();
            return Ok(assegnazioni);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AssegnazioneMezzoDetailDto>> GetById(int id)
        {
            var assegnazione = await _assegnazioniMezziService.GetByIdAsync(id);

            if (assegnazione == null)
                return NotFound(new { message = "Assegnazione mezzo non trovata" });

            return Ok(assegnazione);
        }

        [HttpPost]
        public async Task<ActionResult<AssegnazioneMezzoDetailDto>> Create(CreateAssegnazioneMezzoDto dto)
        {
            var result = await _assegnazioniMezziService.CreateAsync(dto);

            if (!result.Success)
                return StatusCode(result.StatusCode ?? 400, new { message = result.Message });

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateAssegnazioneMezzoDto dto)
        {
            var result = await _assegnazioniMezziService.UpdateAsync(id, dto);

            if (!result.Success)
                return StatusCode(result.StatusCode ?? 400, new { message = result.Message });

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _assegnazioniMezziService.DeleteAsync(id);

            if (!result.Success)
                return StatusCode(result.StatusCode ?? 400, new { message = result.Message });

            return NoContent();
        }
    }
}
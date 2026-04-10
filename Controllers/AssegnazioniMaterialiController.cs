using Gestionale.Api.DTOs;
using Gestionale.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gestionale.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssegnazioniMaterialiController : ControllerBase
    {
        private readonly IAssegnazioniMaterialiService _assegnazioniMaterialiService;

        public AssegnazioniMaterialiController(IAssegnazioniMaterialiService assegnazioniMaterialiService)
        {
            _assegnazioniMaterialiService = assegnazioniMaterialiService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AssegnazioneMaterialeListDto>>> GetAll()
        {
            var assegnazioni = await _assegnazioniMaterialiService.GetAllAsync();
            return Ok(assegnazioni);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AssegnazioneMaterialeDetailDto>> GetById(int id)
        {
            var assegnazione = await _assegnazioniMaterialiService.GetByIdAsync(id);

            if (assegnazione == null)
                return NotFound(new { message = "Assegnazione materiale non trovata" });

            return Ok(assegnazione);
        }

        [HttpPost]
        public async Task<ActionResult<AssegnazioneMaterialeDetailDto>> Create(CreateAssegnazioneMaterialeDto dto)
        {
            var result = await _assegnazioniMaterialiService.CreateAsync(dto);

            if (!result.Success)
                return StatusCode(result.StatusCode ?? 400, new { message = result.Message });

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateAssegnazioneMaterialeDto dto)
        {
            var result = await _assegnazioniMaterialiService.UpdateAsync(id, dto);

            if (!result.Success)
                return StatusCode(result.StatusCode ?? 400, new { message = result.Message });

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _assegnazioniMaterialiService.DeleteAsync(id);

            if (!result.Success)
                return StatusCode(result.StatusCode ?? 400, new { message = result.Message });

            return NoContent();
        }
    }
}
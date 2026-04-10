using Gestionale.Api.DTOs;
using Gestionale.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gestionale.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaterialiController : ControllerBase
    {
        private readonly IMaterialiService _materialiService;

        public MaterialiController(IMaterialiService materialiService)
        {
            _materialiService = materialiService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaterialeListDto>>> GetAll()
        {
            var materiali = await _materialiService.GetAllAsync();
            return Ok(materiali);
        }

        [HttpGet("attivi")]
        public async Task<ActionResult<IEnumerable<SelectOptionDto>>> GetAttivi()
        {
            var materiali = await _materialiService.GetAttiviAsync();
            return Ok(materiali);
        }

        [HttpGet("sotto-scorta")]
        public async Task<ActionResult<IEnumerable<MaterialeListDto>>> GetSottoScorta()
        {
            var materiali = await _materialiService.GetSottoScortaAsync();
            return Ok(materiali);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MaterialeDetailDto>> GetById(int id)
        {
            var materiale = await _materialiService.GetByIdAsync(id);

            if (materiale == null)
                return NotFound(new { message = "Materiale non trovato" });

            return Ok(materiale);
        }

        [HttpPost]
        public async Task<ActionResult<MaterialeDetailDto>> Create(CreateMaterialeDto dto)
        {
            var result = await _materialiService.CreateAsync(dto);

            if (!result.Success)
                return StatusCode(result.StatusCode ?? 400, new { message = result.Message });

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateMaterialeDto dto)
        {
            var result = await _materialiService.UpdateAsync(id, dto);

            if (!result.Success)
                return StatusCode(result.StatusCode ?? 400, new { message = result.Message });

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _materialiService.DeleteAsync(id);

            if (!result.Success)
                return StatusCode(result.StatusCode ?? 400, new { message = result.Message });

            return NoContent();
        }
    }
}
using Gestionale.Api.DTOs;
using Gestionale.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gestionale.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DipendentiController : ControllerBase
    {
        private readonly IDipendentiService _dipendentiService;

        public DipendentiController(IDipendentiService dipendentiService)
        {
            _dipendentiService = dipendentiService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DipendenteListDto>>> GetAll()
        {
            var dipendenti = await _dipendentiService.GetAllAsync();
            return Ok(dipendenti);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DipendenteDetailDto>> GetById(int id)
        {
            var dipendente = await _dipendentiService.GetByIdAsync(id);

            if (dipendente == null)
            {
                return NotFound(new { message = "Dipendente non trovato" });
            }

            return Ok(dipendente);
        }

        [HttpPost]
        public async Task<ActionResult<DipendenteDetailDto>> Create(CreateDipendenteDto dto)
        {
            var result = await _dipendentiService.CreateAsync(dto);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateDipendenteDto dto)
        {
            var result = await _dipendentiService.UpdateAsync(id, dto);

            if (!result.Success)
            {
                if (result.Message == "Dipendente non trovato.")
                {
                    return NotFound(new { message = result.Message });
                }

                return BadRequest(new { message = result.Message });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _dipendentiService.DeleteAsync(id);

            if (!result.Success)
            {
                if (result.Message == "Dipendente non trovato.")
                {
                    return NotFound(new { message = result.Message });
                }

                return BadRequest(new { message = result.Message });
            }

            return NoContent();
        }
    }
}
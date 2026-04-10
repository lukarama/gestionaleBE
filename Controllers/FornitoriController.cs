using Gestionale.Api.DTOs;
using Gestionale.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gestionale.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FornitoriController : ControllerBase
    {
        private readonly IFornitoriService _fornitoriService;

        public FornitoriController(IFornitoriService fornitoriService)
        {
            _fornitoriService = fornitoriService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FornitoreListDto>>> GetAll()
        {
            var fornitori = await _fornitoriService.GetAllAsync();
            return Ok(fornitori);
        }

        [HttpGet("attivi")]
        public async Task<ActionResult<IEnumerable<SelectOptionDto>>> GetAttivi()
        {
            var fornitori = await _fornitoriService.GetAttiviAsync();
            return Ok(fornitori);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FornitoreDetailDto>> GetById(int id)
        {
            var fornitore = await _fornitoriService.GetByIdAsync(id);

            if (fornitore == null)
            {
                return NotFound(new { message = "Fornitore non trovato" });
            }

            return Ok(fornitore);
        }

        [HttpPost]
        public async Task<ActionResult<FornitoreDetailDto>> Create(CreateFornitoreDto dto)
        {
            var result = await _fornitoriService.CreateAsync(dto);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode ?? 400, new { message = result.Message });
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateFornitoreDto dto)
        {
            var result = await _fornitoriService.UpdateAsync(id, dto);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode ?? 400, new { message = result.Message });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _fornitoriService.DeleteAsync(id);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode ?? 400, new { message = result.Message });
            }

            return NoContent();
        }
    }
}
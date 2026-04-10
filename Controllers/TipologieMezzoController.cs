using Gestionale.Api.DTOs;
using Gestionale.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gestionale.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TipologieMezzoController : ControllerBase
    {
        private readonly ITipologieMezzoService _tipologieMezzoService;

        public TipologieMezzoController(ITipologieMezzoService tipologieMezzoService)
        {
            _tipologieMezzoService = tipologieMezzoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TipologiaMezzoListDto>>> GetAll()
        {
            var tipologie = await _tipologieMezzoService.GetAllAsync();
            return Ok(tipologie);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TipologiaMezzoDetailDto>> GetById(int id)
        {
            var tipologia = await _tipologieMezzoService.GetByIdAsync(id);

            if (tipologia == null)
            {
                return NotFound(new { message = "Tipologia mezzo non trovata" });
            }

            return Ok(tipologia);
        }

        [HttpGet("attive")]
        public async Task<ActionResult<IEnumerable<SelectOptionDto>>> GetAttive()
        {
            var tipologie = await _tipologieMezzoService.GetAttiveAsync();
            return Ok(tipologie);
        }

        [HttpPost]
        public async Task<ActionResult<TipologiaMezzoDetailDto>> Create(CreateTipologiaMezzoDto dto)
        {
            var result = await _tipologieMezzoService.CreateAsync(dto);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode ?? 400, new { message = result.Message });
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateTipologiaMezzoDto dto)
        {
            var result = await _tipologieMezzoService.UpdateAsync(id, dto);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode ?? 400, new { message = result.Message });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _tipologieMezzoService.DeleteAsync(id);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode ?? 400, new { message = result.Message });
            }

            return NoContent();
        }
    }
}
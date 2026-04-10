using Gestionale.Api.DTOs;
using Gestionale.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gestionale.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MansioniController : ControllerBase
    {
        private readonly IMansioniService _mansioniService;

        public MansioniController(IMansioniService mansioniService)
        {
            _mansioniService = mansioniService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MansioneListDto>>> GetAll()
        {
            var mansioni = await _mansioniService.GetAllAsync();
            return Ok(mansioni);
        }

        [HttpGet("attive")]
        public async Task<ActionResult<IEnumerable<SelectOptionDto>>> GetAttive()
        {
            var mansioni = await _mansioniService.GetAttiveAsync();
            return Ok(mansioni);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MansioneDetailDto>> GetById(int id)
        {
            var mansione = await _mansioniService.GetByIdAsync(id);

            if (mansione == null)
                return NotFound(new { message = "Mansione non trovata" });

            return Ok(mansione);
        }

        [HttpPost]
        public async Task<ActionResult<MansioneDetailDto>> Create(CreateMansioneDto dto)
        {
            var result = await _mansioniService.CreateAsync(dto);

            if (!result.Success)
                return StatusCode(result.StatusCode ?? 400, new { message = result.Message });

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateMansioneDto dto)
        {
            var result = await _mansioniService.UpdateAsync(id, dto);

            if (!result.Success)
                return StatusCode(result.StatusCode ?? 400, new { message = result.Message });

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mansioniService.DeleteAsync(id);

            if (!result.Success)
                return StatusCode(result.StatusCode ?? 400, new { message = result.Message });

            return NoContent();
        }
    }
}
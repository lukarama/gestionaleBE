using Gestionale.Api.DTOs;
using Gestionale.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gestionale.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CantieriController : ControllerBase
    {
        private readonly ICantieriService _cantieriService;

        public CantieriController(ICantieriService cantieriService)
        {
            _cantieriService = cantieriService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CantiereListDto>>> GetAll()
        {
            var cantieri = await _cantieriService.GetAllAsync();
            return Ok(cantieri);
        }

        [HttpGet("attivi")]
        public async Task<ActionResult<IEnumerable<SelectOptionDto>>> GetAttivi()
        {
            var cantieri = await _cantieriService.GetAttiviAsync();
            return Ok(cantieri);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CantiereDetailDto>> GetById(int id)
        {
            var cantiere = await _cantieriService.GetByIdAsync(id);

            if (cantiere == null)
            {
                return NotFound(new { message = "Cantiere non trovato" });
            }

            return Ok(cantiere);
        }

        [HttpPost]
        public async Task<ActionResult<CantiereDetailDto>> Create(CreateCantiereDto dto)
        {
            var result = await _cantieriService.CreateAsync(dto);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode ?? 400, new { message = result.Message });
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateCantiereDto dto)
        {
            var result = await _cantieriService.UpdateAsync(id, dto);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode ?? 400, new { message = result.Message });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _cantieriService.DeleteAsync(id);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode ?? 400, new { message = result.Message });
            }

            return NoContent();
        }
    }
}
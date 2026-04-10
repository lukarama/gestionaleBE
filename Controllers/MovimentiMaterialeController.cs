using Gestionale.Api.DTOs;
using Gestionale.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gestionale.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovimentiMaterialeController : ControllerBase
    {
        private readonly IMovimentiMaterialeService _movimentiMaterialeService;

        public MovimentiMaterialeController(IMovimentiMaterialeService movimentiMaterialeService)
        {
            _movimentiMaterialeService = movimentiMaterialeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovimentoMaterialeListDto>>> GetAll()
        {
            var movimenti = await _movimentiMaterialeService.GetAllAsync();
            return Ok(movimenti);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MovimentoMaterialeDetailDto>> GetById(int id)
        {
            var movimento = await _movimentiMaterialeService.GetByIdAsync(id);

            if (movimento == null)
                return NotFound(new { message = "Movimento materiale non trovato" });

            return Ok(movimento);
        }

        [HttpPost]
        public async Task<ActionResult<MovimentoMaterialeDetailDto>> Create(CreateMovimentoMaterialeDto dto)
        {
            var result = await _movimentiMaterialeService.CreateAsync(dto);

            if (!result.Success)
                return StatusCode(result.StatusCode ?? 400, new { message = result.Message });

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateMovimentoMaterialeDto dto)
        {
            var result = await _movimentiMaterialeService.UpdateAsync(id, dto);

            if (!result.Success)
                return StatusCode(result.StatusCode ?? 400, new { message = result.Message });

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _movimentiMaterialeService.DeleteAsync(id);

            if (!result.Success)
                return StatusCode(result.StatusCode ?? 400, new { message = result.Message });

            return NoContent();
        }
    }
}
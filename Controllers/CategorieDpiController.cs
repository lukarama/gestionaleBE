using Gestionale.Api.DTOs;
using Gestionale.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gestionale.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategorieDpiController : ControllerBase
    {
        private readonly ICategorieDpiService _categorieDpiService;

        public CategorieDpiController(ICategorieDpiService categorieDpiService)
        {
            _categorieDpiService = categorieDpiService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaDpiListDto>>> GetAll()
        {
            var categorie = await _categorieDpiService.GetAllAsync();
            return Ok(categorie);
        }

        [HttpGet("attive")]
        public async Task<ActionResult<IEnumerable<SelectOptionDto>>> GetAttive()
        {
            var categorie = await _categorieDpiService.GetAttiveAsync();
            return Ok(categorie);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoriaDpiDetailDto>> GetById(int id)
        {
            var categoria = await _categorieDpiService.GetByIdAsync(id);

            if (categoria == null)
            {
                return NotFound(new { message = "Categoria DPI non trovata" });
            }

            return Ok(categoria);
        }

        [HttpPost]
        public async Task<ActionResult<CategoriaDpiDetailDto>> Create(CreateCategoriaDpiDto dto)
        {
            var result = await _categorieDpiService.CreateAsync(dto);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode ?? 400, new { message = result.Message });
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateCategoriaDpiDto dto)
        {
            var result = await _categorieDpiService.UpdateAsync(id, dto);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode ?? 400, new { message = result.Message });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categorieDpiService.DeleteAsync(id);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode ?? 400, new { message = result.Message });
            }

            return NoContent();
        }
    }
}
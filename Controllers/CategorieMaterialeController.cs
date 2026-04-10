using Gestionale.Api.DTOs;
using Gestionale.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gestionale.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategorieMaterialeController : ControllerBase
    {
        private readonly ICategorieMaterialeService _categorieMaterialeService;

        public CategorieMaterialeController(ICategorieMaterialeService categorieMaterialeService)
        {
            _categorieMaterialeService = categorieMaterialeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaMaterialeListDto>>> GetAll()
        {
            var categorie = await _categorieMaterialeService.GetAllAsync();
            return Ok(categorie);
        }

        [HttpGet("attive")]
        public async Task<ActionResult<IEnumerable<SelectOptionDto>>> GetAttive()
        {
            var categorie = await _categorieMaterialeService.GetAttiveAsync();
            return Ok(categorie);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoriaMaterialeDetailDto>> GetById(int id)
        {
            var categoria = await _categorieMaterialeService.GetByIdAsync(id);

            if (categoria == null)
            {
                return NotFound(new { message = "Categoria materiale non trovata" });
            }

            return Ok(categoria);
        }

        [HttpPost]
        public async Task<ActionResult<CategoriaMaterialeDetailDto>> Create(CreateCategoriaMaterialeDto dto)
        {
            var result = await _categorieMaterialeService.CreateAsync(dto);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode ?? 400, new { message = result.Message });
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateCategoriaMaterialeDto dto)
        {
            var result = await _categorieMaterialeService.UpdateAsync(id, dto);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode ?? 400, new { message = result.Message });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categorieMaterialeService.DeleteAsync(id);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode ?? 400, new { message = result.Message });
            }

            return NoContent();
        }
    }
}
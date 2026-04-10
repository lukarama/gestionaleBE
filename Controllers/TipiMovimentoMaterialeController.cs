using Gestionale.Api.DTOs;
using Gestionale.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gestionale.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TipiMovimentoMaterialeController : ControllerBase
{
    private readonly ITipiMovimentoMaterialeService _service;

    public TipiMovimentoMaterialeController(ITipiMovimentoMaterialeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<TipiMovimentoMaterialeDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TipiMovimentoMaterialeDto>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<TipiMovimentoMaterialeDto>> Create([FromBody] CreateTipoMovimentoMaterialeDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTipoMovimentoMaterialeDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
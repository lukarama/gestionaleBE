using Gestionale.Api.DTOs;
using Gestionale.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gestionale.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TipiDocumentoController : ControllerBase
{
    private readonly ITipiDocumentoService _service;

    public TipiDocumentoController(ITipiDocumentoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<TipoDocumentoDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TipoDocumentoDto>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);

        if (result == null)
            return NotFound(new { message = "Tipo documento non trovato." });

        return Ok(result);
    }

    [HttpGet("select")]
    public async Task<ActionResult<List<TipoDocumentoSelectDto>>> GetSelect()
    {
        var result = await _service.GetSelectAsync();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTipoDocumentoDto dto)
    {
        var result = await _service.CreateAsync(dto);

        if (!result.Success)
            return StatusCode(result.StatusCode ?? 400, result);

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTipoDocumentoDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);

        if (!result.Success)
            return StatusCode(result.StatusCode ?? 400, result);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);

        if (!result.Success)
            return StatusCode(result.StatusCode ?? 400, result);

        return Ok(result);
    }
}
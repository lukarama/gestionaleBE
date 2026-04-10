using Gestionale.Api.DTOs;
using Gestionale.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gestionale.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VisiteMedicheController : ControllerBase
{
    private readonly IVisiteMedicheService _service;

    public VisiteMedicheController(IVisiteMedicheService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<VisitaMedicaListDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("dashboard/scadenze")]
    public async Task<ActionResult<List<VisitaMedicaScadenzaDashboardDto>>> GetDashboardScadenze([FromQuery] int giorni = 30)
    {
        var result = await _service.GetDashboardScadenzeAsync(giorni);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<VisitaMedicaDetailDto>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);

        if (result == null)
            return NotFound(new { message = "Visita medica non trovata." });

        return Ok(result);
    }

    [HttpGet("dipendente/{dipendenteId}")]
    public async Task<ActionResult<List<VisitaMedicaListDto>>> GetByDipendenteId(int dipendenteId)
    {
        var result = await _service.GetByDipendenteIdAsync(dipendenteId);
        return Ok(result);
    }

    [HttpGet("in-scadenza")]
    public async Task<ActionResult<List<VisitaMedicaListDto>>> GetInScadenza([FromQuery] int giorni = 30)
    {
        var result = await _service.GetInScadenzaAsync(giorni);
        return Ok(result);
    }

    [HttpGet("select/tipi-visita")]
    public async Task<ActionResult<List<TipoVisitaMedicaSelectDto>>> GetTipiVisitaSelect()
    {
        var result = await _service.GetTipiVisitaSelectAsync();
        return Ok(result);
    }

    [HttpGet("select/esiti")]
    public async Task<ActionResult<List<EsitoVisitaMedicaSelectDto>>> GetEsitiSelect()
    {
        var result = await _service.GetEsitiSelectAsync();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateVisitaMedicaDto dto)
    {
        var result = await _service.CreateAsync(dto);

        if (!result.Success)
            return StatusCode(result.StatusCode ?? 400, result);

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateVisitaMedicaDto dto)
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
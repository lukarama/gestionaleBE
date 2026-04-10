using Gestionale.Api.DTOs;
using Gestionale.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gestionale.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentiMezziController : ControllerBase
{
    private readonly IDocumentiMezziService _service;

    public DocumentiMezziController(IDocumentiMezziService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<DocumentoMezzoListDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DocumentoMezzoDetailDto>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);

        if (result == null)
            return NotFound(new { message = "Documento mezzo non trovato." });

        return Ok(result);
    }

    [HttpGet("mezzo/{mezzoId}")]
    public async Task<ActionResult<List<DocumentoMezzoListDto>>> GetByMezzoId(int mezzoId)
    {
        var result = await _service.GetByMezzoIdAsync(mezzoId);
        return Ok(result);
    }

    [HttpGet("in-scadenza")]
    public async Task<ActionResult<List<DocumentoMezzoScadenzaDto>>> GetInScadenza([FromQuery] int giorni = 30)
    {
        var result = await _service.GetInScadenzaAsync(giorni);
        return Ok(result);
    }

    [HttpGet("{id}/download")]
    public async Task<IActionResult> Download(int id)
    {
        var result = await _service.GetDownloadAsync(id);

        if (!result.Success)
            return StatusCode(result.StatusCode ?? 400, result);

        return File(
            result.Data!.FileBytes,
            result.Data.ContentType,
            result.Data.NomeFile);
    }

    [HttpGet("{id}/open")]
    public async Task<IActionResult> Open(int id)
    {
        var result = await _service.GetDownloadAsync(id);

        if (!result.Success)
            return StatusCode(result.StatusCode ?? 400, result);

        Response.Headers["Content-Disposition"] = $"inline; filename=\"{result.Data!.NomeFile}\"";

        return File(
            result.Data.FileBytes,
            result.Data.ContentType);
    }

    [HttpGet("dashboard/scadenze")]
    public async Task<ActionResult<List<DocumentoMezzoDashboardScadenzaDto>>> GetDashboardScadenze([FromQuery] int giorni = 30)
    {
        var result = await _service.GetDashboardScadenzeAsync(giorni);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDocumentoMezzoDto dto)
    {
        var result = await _service.CreateAsync(dto);

        if (!result.Success)
            return StatusCode(result.StatusCode ?? 400, result);

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] UploadDocumentoMezzoDto dto)
    {
        var result = await _service.UploadAsync(dto);

        if (!result.Success)
            return StatusCode(result.StatusCode ?? 400, result);

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDocumentoMezzoDto dto)
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
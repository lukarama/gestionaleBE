using Gestionale.Api.DTOs;
using Gestionale.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gestionale.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentiCantieriController : ControllerBase
{
    private readonly IDocumentiCantieriService _service;

    public DocumentiCantieriController(IDocumentiCantieriService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<DocumentoCantiereListDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DocumentoCantiereDetailDto>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);

        if (result == null)
            return NotFound(new { message = "Documento cantiere non trovato." });

        return Ok(result);
    }

    [HttpGet("cantiere/{cantiereId}")]
    public async Task<ActionResult<List<DocumentoCantiereListDto>>> GetByCantiereId(int cantiereId)
    {
        var result = await _service.GetByCantiereIdAsync(cantiereId);
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

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDocumentoCantiereDto dto)
    {
        var result = await _service.CreateAsync(dto);

        if (!result.Success)
            return StatusCode(result.StatusCode ?? 400, result);

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] UploadDocumentoCantiereDto dto)
    {
        var result = await _service.UploadAsync(dto);

        if (!result.Success)
            return StatusCode(result.StatusCode ?? 400, result);

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDocumentoCantiereDto dto)
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

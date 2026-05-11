using System.Security.Claims;
using Gestionale.Api.Common.Auth;
using Gestionale.Api.DTOs;
using Gestionale.Api.Security;
using Gestionale.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gestionale.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN,SEGRETERIA,DIPENDENTE")]
public class DocumentiDipendentiController : ControllerBase
{
    private readonly IDocumentiDipendentiService _service;

    public DocumentiDipendentiController(IDocumentiDipendentiService service)
    {
        _service = service;
    }

    [HttpGet("dipendenti")]
    [Authorize(Roles = "ADMIN,SEGRETERIA")]
    public async Task<IActionResult> GetDipendentiDocumenti()
    {
        var result = await _service.GetDipendentiDocumentiSelectAsync(BuildUserContext());
        if (!result.Success)
            return ToErrorResult(result.StatusCode ?? 400, result.Message);

        return Ok(result.Data);
    }

    [HttpGet("dipendenti/{dipendenteId:int}")]
    public async Task<IActionResult> GetTreeByDipendenteId(int dipendenteId)
    {
        var result = await _service.GetTreeByDipendenteIdAsync(dipendenteId, BuildUserContext());
        if (!result.Success)
            return ToErrorResult(result.StatusCode ?? 400, result.Message);

        return Ok(result.Data);
    }

    [HttpGet("miei")]
    [HasPermission(PermissionCodes.DocumentiDipendentiReadSelf)]
    public async Task<IActionResult> GetMieiDocumenti()
    {
        var result = await _service.GetMyTreeAsync(BuildUserContext());
        if (!result.Success)
            return ToErrorResult(result.StatusCode ?? 400, result.Message);

        return Ok(result.Data);
    }

    [HttpPost("dipendenti/{dipendenteId:int}/cartelle")]
    [Authorize(Roles = "ADMIN,SEGRETERIA")]
    public async Task<IActionResult> CreateCartella(int dipendenteId, [FromBody] CreateCartellaDocumentoDipendenteDto dto)
    {
        var result = await _service.CreateCartellaAsync(dipendenteId, dto, BuildUserContext());
        if (!result.Success)
            return ToErrorResult(result.StatusCode ?? 400, result.Message);

        return CreatedAtAction(nameof(GetTreeByDipendenteId), new { dipendenteId = result.Data!.DipendenteId }, result.Data);
    }

    [HttpPatch("cartelle/{cartellaId:int}")]
    [Authorize(Roles = "ADMIN,SEGRETERIA")]
    public async Task<IActionResult> RenameCartella(int cartellaId, [FromBody] UpdateCartellaDocumentoDipendenteDto dto)
    {
        var result = await _service.RenameCartellaAsync(cartellaId, dto, BuildUserContext());
        if (!result.Success)
            return ToErrorResult(result.StatusCode ?? 400, result.Message);

        return Ok(result.Data);
    }

    [HttpDelete("cartelle/{cartellaId:int}")]
    [Authorize(Roles = "ADMIN,SEGRETERIA")]
    public async Task<IActionResult> DeleteCartella(int cartellaId)
    {
        var result = await _service.DeleteCartellaAsync(cartellaId, BuildUserContext());
        if (!result.Success)
            return ToErrorResult(result.StatusCode ?? 400, result.Message);

        return NoContent();
    }

    [HttpPost("upload")]
    [Authorize(Roles = "ADMIN,SEGRETERIA")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload([FromForm] UploadDocumentoDipendenteDto dto)
    {
        var result = await _service.UploadAsync(dto, BuildUserContext());
        if (!result.Success)
            return ToErrorResult(result.StatusCode ?? 400, result.Message);

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    [HttpGet("{id:int}/download")]
    public async Task<IActionResult> Download(int id)
    {
        var result = await _service.GetDownloadAsync(id, BuildUserContext());
        if (!result.Success)
            return ToErrorResult(result.StatusCode ?? 400, result.Message);

        return File(result.Data!.FileBytes, result.Data.ContentType, result.Data.NomeFile);
    }

    [HttpGet("{id:int}/open")]
    public async Task<IActionResult> Open(int id)
    {
        var result = await _service.GetDownloadAsync(id, BuildUserContext());
        if (!result.Success)
            return ToErrorResult(result.StatusCode ?? 400, result.Message);

        Response.Headers["Content-Disposition"] = $"inline; filename=\"{result.Data!.NomeFile}\"";
        return File(result.Data.FileBytes, result.Data.ContentType);
    }

    [HttpPatch("{id:int}/nome")]
    [Authorize(Roles = "ADMIN,SEGRETERIA")]
    public async Task<IActionResult> RenameDocumento(int id, [FromBody] RenameDocumentoDipendenteDto dto)
    {
        var result = await _service.RenameDocumentoAsync(id, dto, BuildUserContext());
        if (!result.Success)
            return ToErrorResult(result.StatusCode ?? 400, result.Message);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "ADMIN,SEGRETERIA")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id, BuildUserContext());
        if (!result.Success)
            return ToErrorResult(result.StatusCode ?? 400, result.Message);

        return NoContent();
    }

    [HttpGet]
    [Authorize(Roles = "ADMIN,SEGRETERIA")]
    public async Task<ActionResult<List<DocumentoDipendenteListDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DocumentoDipendenteDetailDto>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);

        if (result == null)
            return NotFound(new { message = "Documento dipendente non trovato." });

        var access = await _service.GetTreeByDipendenteIdAsync(result.DipendenteId, BuildUserContext());
        if (!access.Success)
            return ToErrorResult(access.StatusCode ?? 400, access.Message);

        return Ok(result);
    }

    [HttpGet("dipendente/{dipendenteId:int}")]
    public async Task<ActionResult<List<DocumentoDipendenteListDto>>> GetByDipendenteId(int dipendenteId)
    {
        var access = await _service.GetTreeByDipendenteIdAsync(dipendenteId, BuildUserContext());
        if (!access.Success)
            return ToErrorResult(access.StatusCode ?? 400, access.Message);

        return Ok(access.Data!.Documenti);
    }

    [HttpGet("dipendente/{dipendenteId:int}/files")]
    [Authorize(Roles = "ADMIN,SEGRETERIA")]
    public async Task<IActionResult> GetFilesByDipendenteId(int dipendenteId)
    {
        var result = await _service.GetFilesByDipendenteIdAsync(dipendenteId);
        if (!result.Success)
            return ToErrorResult(result.StatusCode ?? 400, result.Message);

        return Ok(result.Data);
    }

    [HttpGet("dipendente/{dipendenteId:int}/files/{nomeFile}/download")]
    [Authorize(Roles = "ADMIN,SEGRETERIA")]
    public async Task<IActionResult> DownloadFileByDipendenteId(int dipendenteId, string nomeFile)
    {
        var result = await _service.GetFileDownloadByDipendenteIdAsync(dipendenteId, nomeFile);
        if (!result.Success)
            return ToErrorResult(result.StatusCode ?? 400, result.Message);

        return File(result.Data!.FileBytes, result.Data.ContentType, result.Data.NomeFile);
    }

    [HttpGet("dipendente/{dipendenteId:int}/files/{nomeFile}/open")]
    [Authorize(Roles = "ADMIN,SEGRETERIA")]
    public async Task<IActionResult> OpenFileByDipendenteId(int dipendenteId, string nomeFile)
    {
        var result = await _service.GetFileDownloadByDipendenteIdAsync(dipendenteId, nomeFile);
        if (!result.Success)
            return ToErrorResult(result.StatusCode ?? 400, result.Message);

        Response.Headers["Content-Disposition"] = $"inline; filename=\"{result.Data!.NomeFile}\"";
        return File(result.Data.FileBytes, result.Data.ContentType);
    }

    [HttpGet("in-scadenza")]
    [Authorize(Roles = "ADMIN,SEGRETERIA")]
    public async Task<ActionResult<List<DocumentoDipendenteScadenzaDto>>> GetInScadenza([FromQuery] int giorni = 30)
    {
        var result = await _service.GetInScadenzaAsync(giorni);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN,SEGRETERIA")]
    public async Task<IActionResult> Create([FromBody] CreateDocumentoDipendenteDto dto)
    {
        var result = await _service.CreateAsync(dto);
        if (!result.Success)
            return ToErrorResult(result.StatusCode ?? 400, result.Message);

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "ADMIN,SEGRETERIA")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDocumentoDipendenteDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        if (!result.Success)
            return ToErrorResult(result.StatusCode ?? 400, result.Message);

        return NoContent();
    }

    private UserContext BuildUserContext()
    {
        return new UserContext
        {
            UserId = TryParseInt(User.FindFirst(ClaimTypes.NameIdentifier)?.Value),
            DipendenteId = TryParseInt(User.FindFirst(CustomClaimTypes.DipendenteId)?.Value),
            Roles = User.FindAll(ClaimTypes.Role).Select(claim => claim.Value).ToArray()
        };
    }

    private static int? TryParseInt(string? value)
    {
        return int.TryParse(value, out var parsed) ? parsed : null;
    }

    private ObjectResult ToErrorResult(int statusCode, string? message)
    {
        return StatusCode(statusCode, new { message });
    }
}

using Gestionale.Api.DTOs;
using Gestionale.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gestionale.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MagazzinoController : ControllerBase
{
    private readonly IMagazzinoService _service;
    private readonly IImportazioniMagazzinoService _importazioniService;

    public MagazzinoController(IMagazzinoService service, IImportazioniMagazzinoService importazioniService)
    {
        _service = service;
        _importazioniService = importazioniService;
    }

    [HttpPost("import-excel/preview")]
    public async Task<IActionResult> PreviewImportExcel([FromForm] UploadImportMovimentiMagazzinoDto dto)
    {
        var result = await _importazioniService.CreaAnteprimaAsync(dto);

        if (!result.Success)
            return StatusCode(result.StatusCode ?? 400, result);

        return Ok(result);
    }

    [HttpPost("import-excel/confirm")]
    public async Task<IActionResult> ConfermaImportExcel([FromBody] ConfermaImportMovimentiMagazzinoDto dto)
    {
        var result = await _importazioniService.ConfermaImportazioneAsync(dto);

        if (!result.Success)
            return StatusCode(result.StatusCode ?? 400, result);

        return Ok(result);
    }

    [HttpPost("prelievo")]
    public async Task<IActionResult> Prelievo([FromBody] PrelievoMaterialeDto dto)
    {
        var result = await _service.PrelevaAsync(dto);

        if (!result.Success)
            return StatusCode(result.StatusCode ?? 400, result);

        return Ok(result);
    }

    [HttpPost("rifornimento")]
    public async Task<IActionResult> Rifornimento([FromBody] PrelievoMaterialeDto dto)
    {
        var result = await _service.RifornisciAsync(dto);

        if (!result.Success)
            return StatusCode(result.StatusCode ?? 400, result);

        return Ok(result);
    }

    [HttpGet("by-barcode/{barcode}")]
    public async Task<IActionResult> GetByBarcode(string barcode)
    {
        var result = await _service.GetByEanAsync(barcode);

        if (!result.Success)
            return StatusCode(result.StatusCode ?? 400, result);

        return Ok(result);
    }

    [HttpGet("sotto-scorta")]
    public async Task<ActionResult<List<MaterialeSottoScortaDto>>> GetSottoScorta()
    {
        var result = await _service.GetMaterialiSottoScortaAsync();
        return Ok(result);
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult<DashboardMagazzinoDto>> GetDashboard()
    {
        var result = await _service.GetDashboardAsync();
        return Ok(result);
    }

    [HttpGet("materiale/{materialeId}/movimenti")]
    public async Task<ActionResult<List<StoricoMovimentoMaterialeDto>>> GetStoricoMovimentiMateriale(int materialeId)
    {
        var result = await _service.GetStoricoMovimentiMaterialeAsync(materialeId);
        return Ok(result);
    }

    [HttpGet("materiale/{id}")]
    public async Task<IActionResult> GetMaterialeById(int id)
    {
        var result = await _service.GetMaterialeByIdAsync(id);

        if (!result.Success)
            return StatusCode(result.StatusCode ?? 400, result);

        return Ok(result);
    }

    [HttpGet("materiale/{id}/disponibilita")]
    public async Task<IActionResult> GetDisponibilitaMateriale(int id)
    {
        var result = await _service.GetDisponibilitaMaterialeAsync(id);

        if (!result.Success)
            return StatusCode(result.StatusCode ?? 400, result);

        return Ok(result);
    }

    [HttpGet("movimenti/ultimi")]
    public async Task<ActionResult<List<UltimoMovimentoMagazzinoDto>>> GetUltimiMovimenti([FromQuery] int top = 10)
    {
        var result = await _service.GetUltimiMovimentiAsync(top);
        return Ok(result);
    }

    [HttpGet("select/materiali")]
    public async Task<ActionResult<List<MaterialeSelectDto>>> GetMaterialiSelect()
    {
        var result = await _service.GetMaterialiSelectAsync();
        return Ok(result);
    }

    [HttpGet("select/dipendenti")]
    public async Task<ActionResult<List<DipendenteSelectDto>>> GetDipendentiSelect()
    {
        var result = await _service.GetDipendentiSelectAsync();
        return Ok(result);
    }

    [HttpGet("select/cantieri")]
    public async Task<ActionResult<List<CantiereSelectDto>>> GetCantieriSelect()
    {
        var result = await _service.GetCantieriSelectAsync();
        return Ok(result);
    }

    [HttpGet("materiali")]
    public async Task<ActionResult<List<RicercaMaterialeDto>>> RicercaMateriali([FromQuery] string? testo)
    {
        var result = await _service.RicercaMaterialiAsync(testo);
        return Ok(result);
    }
}

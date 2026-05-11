using Gestionale.Api.DTOs;
using Gestionale.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gestionale.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatiAssegnazioneController : ControllerBase
{
    private readonly IStatiAssegnazioneService _service;

    public StatiAssegnazioneController(IStatiAssegnazioneService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<StatoAssegnazioneListDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("attivi")]
    public async Task<ActionResult<List<SelectOptionDto>>> GetAttivi()
    {
        var result = await _service.GetAttiviAsync();
        return Ok(result);
    }
}

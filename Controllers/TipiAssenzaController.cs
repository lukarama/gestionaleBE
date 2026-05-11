using Gestionale.Api.DTOs;
using Gestionale.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gestionale.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN,DIPENDENTE")]
public class TipiAssenzaController : ControllerBase
{
    private readonly IAssenzeService _assenzeService;

    public TipiAssenzaController(IAssenzeService assenzeService)
    {
        _assenzeService = assenzeService;
    }

    [HttpGet]
    public async Task<ActionResult<List<TipoAssenzaDto>>> GetTipiAssenza()
    {
        var result = await _assenzeService.GetTipiAssenzaAsync();
        return Ok(result);
    }
}

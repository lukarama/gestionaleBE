using System.Security.Claims;
using Gestionale.Api.Common.Auth;
using Gestionale.Api.DTOs;
using Gestionale.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gestionale.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN,DIPENDENTE")]
public class AssenzeController : ControllerBase
{
    private readonly IAssenzeService _assenzeService;

    public AssenzeController(IAssenzeService assenzeService)
    {
        _assenzeService = assenzeService;
    }

    [HttpGet("mie")]
    public async Task<IActionResult> GetMieRichieste()
    {
        var result = await _assenzeService.GetMieRichiesteAsync(BuildUserContext());

        if (!result.Success)
            return ToErrorResult(result.StatusCode ?? 400, result.Message);

        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAssenzaDto dto)
    {
        var result = await _assenzeService.CreateRichiestaAsync(dto, BuildUserContext());

        if (!result.Success)
            return ToErrorResult(result.StatusCode ?? 400, result.Message);

        return CreatedAtAction(nameof(GetMieRichieste), new { id = result.Data!.Id }, result.Data);
    }

    [HttpPatch("{id:int}/stato")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> UpdateStato(int id, [FromBody] UpdateStatoAssenzaDto dto)
    {
        var result = await _assenzeService.UpdateStatoRichiestaAsync(id, dto, BuildUserContext());

        if (!result.Success)
            return ToErrorResult(result.StatusCode ?? 400, result.Message);

        return Ok(result.Data);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _assenzeService.DeleteRichiestaAsync(id, BuildUserContext());

        if (!result.Success)
            return ToErrorResult(result.StatusCode ?? 400, result.Message);

        return NoContent();
    }

    private UserContext BuildUserContext()
    {
        var dipendenteIdValue = User.FindFirst(CustomClaimTypes.DipendenteId)?.Value;

        return new UserContext
        {
            UserId = TryParseInt(User.FindFirst(ClaimTypes.NameIdentifier)?.Value),
            DipendenteId = TryParseInt(dipendenteIdValue),
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

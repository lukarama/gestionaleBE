using Gestionale.Api.Common.Auth;
using Gestionale.Api.DTOs;
using Gestionale.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gestionale.Api.Controllers;

[ApiController]
[Route("api/admin/utenti")]
[Authorize(Roles = RoleCodes.Admin)]
public class UtentiAdminController : ControllerBase
{
    private readonly IUtentiAdminService _utentiAdminService;

    public UtentiAdminController(IUtentiAdminService utentiAdminService)
    {
        _utentiAdminService = utentiAdminService;
    }

    [HttpGet("dipendenti")]
    public async Task<ActionResult<List<UtenteDipendenteAdminDto>>> GetDipendenti()
    {
        var result = await _utentiAdminService.GetDipendentiAccountsAsync();
        return Ok(result);
    }

    [HttpPost("dipendenti")]
    public async Task<IActionResult> CreateDipendenteAccount([FromBody] CreateUtenteDipendenteDto dto)
    {
        var result = await _utentiAdminService.CreateDipendenteAccountAsync(dto);
        if (!result.Success)
        {
            return StatusCode(result.StatusCode ?? 400, new { message = result.Message });
        }

        return StatusCode(result.StatusCode ?? 201, result.Data);
    }

    [HttpPut("{userId:int}/visibilita")]
    public async Task<IActionResult> UpdateVisibility(int userId, [FromBody] UpdateUtenteVisibilityDto dto)
    {
        var result = await _utentiAdminService.UpdateVisibilityAsync(userId, dto);
        if (!result.Success)
        {
            return StatusCode(result.StatusCode ?? 400, new { message = result.Message });
        }

        return Ok(result.Data);
    }

    [HttpPost("{userId:int}/reset-password")]
    public async Task<IActionResult> ResetPassword(int userId, [FromBody] ResetUtentePasswordDto? dto)
    {
        var result = await _utentiAdminService.ResetPasswordAsync(userId, dto);
        if (!result.Success)
        {
            return StatusCode(result.StatusCode ?? 400, new { message = result.Message });
        }

        return Ok(result.Data);
    }

    [HttpDelete("{userId:int}")]
    public async Task<IActionResult> DeleteAccount(int userId)
    {
        var result = await _utentiAdminService.DeleteAccountAsync(userId);
        if (!result.Success)
        {
            return StatusCode(result.StatusCode ?? 400, new { message = result.Message });
        }

        return NoContent();
    }
}

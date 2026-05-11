using System.Security.Claims;
using Gestionale.Api.Common.Auth;
using Gestionale.Api.DTOs;
using Gestionale.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gestionale.Api.Controllers;

[ApiController]
[Route("api/material-requests")]
[Authorize(Roles = "ADMIN,RESPONSABILE,DIPENDENTE")]
public class MaterialRequestsController : ControllerBase
{
    private readonly IMaterialRequestsService _service;

    public MaterialRequestsController(IMaterialRequestsService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMaterialRequestDto dto)
    {
        var result = await _service.CreateAsync(dto, BuildUserContext());
        if (!result.Success)
            return ToErrorResult(result.StatusCode ?? 400, result.Message);

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync(BuildUserContext());
        if (!result.Success)
            return ToErrorResult(result.StatusCode ?? 400, result.Message);

        return Ok(result.Data);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id, BuildUserContext());
        if (!result.Success)
            return ToErrorResult(result.StatusCode ?? 400, result.Message);

        return Ok(result.Data);
    }

    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = "ADMIN,RESPONSABILE")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateRequestStatusDto dto)
    {
        var result = await _service.UpdateStatusAsync(id, dto, BuildUserContext());
        if (!result.Success)
            return ToErrorResult(result.StatusCode ?? 400, result.Message);

        return Ok(result.Data);
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

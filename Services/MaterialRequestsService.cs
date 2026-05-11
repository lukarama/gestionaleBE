using System.Linq.Expressions;
using Gestionale.Api.Common;
using Gestionale.Api.Common.Auth;
using Gestionale.Api.DTOs;
using Gestionale.Api.Models;
using Gestionale.Api.Repositories.Interfaces;
using Gestionale.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gestionale.Api.Services;

public class MaterialRequestsService : IMaterialRequestsService
{
    private readonly IMaterialRequestRepository _repository;

    public MaterialRequestsService(IMaterialRequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<ServiceResult<List<MaterialRequestDto>>> GetAllAsync(UserContext user)
    {
        var query = ApplyVisibility(_repository.Query().AsNoTracking(), user);
        if (query == null)
            return ServiceResult<List<MaterialRequestDto>>.Fail("Permessi insufficienti o dipendente non associato all'utente.", 403);

        var result = await query
            .OrderByDescending(request => request.CreatedAt)
            .ThenByDescending(request => request.Id)
            .Select(ToDtoExpression())
            .ToListAsync();

        return ServiceResult<List<MaterialRequestDto>>.Ok(result);
    }

    public async Task<ServiceResult<MaterialRequestDto>> GetByIdAsync(int id, UserContext user)
    {
        var query = ApplyVisibility(_repository.Query().AsNoTracking(), user);
        if (query == null)
            return ServiceResult<MaterialRequestDto>.Fail("Permessi insufficienti o dipendente non associato all'utente.", 403);

        var result = await query
            .Where(request => request.Id == id)
            .Select(ToDtoExpression())
            .FirstOrDefaultAsync();

        if (result == null)
            return ServiceResult<MaterialRequestDto>.Fail("Richiesta materiale non trovata.", 404);

        return ServiceResult<MaterialRequestDto>.Ok(result);
    }

    public async Task<ServiceResult<MaterialRequestDto>> CreateAsync(CreateMaterialRequestDto dto, UserContext user)
    {
        var dipendenteId = ResolveDipendenteId(dto.DipendenteId, user);
        if (!dipendenteId.HasValue)
            return ServiceResult<MaterialRequestDto>.Fail("Dipendente non associato all'utente autenticato.", 403);

        var validation = ValidateCreate(dto);
        if (!validation.Success)
            return ServiceResult<MaterialRequestDto>.Fail(validation.Message!, validation.StatusCode ?? 400);

        var now = DateTime.UtcNow;
        var entity = new MaterialRequest
        {
            DipendenteId = dipendenteId.Value,
            MaterialeRichiesto = Pulisci(dto.MaterialeRichiesto)!,
            Quantita = dto.Quantita,
            Motivazione = Pulisci(dto.Motivazione)!,
            Priorita = Pulisci(dto.Priorita)!,
            DataDesiderata = dto.DataDesiderata,
            Note = Pulisci(dto.Note),
            Stato = RequestStatusCodes.InAttesa,
            CreatedAt = now
        };

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        return await GetByIdAsync(entity.Id, user);
    }

    public async Task<ServiceResult<MaterialRequestDto>> UpdateStatusAsync(int id, UpdateRequestStatusDto dto, UserContext user)
    {
        if (!user.IsAdminOrResponsabile())
            return ServiceResult<MaterialRequestDto>.Fail("Permessi insufficienti per gestire la richiesta.", 403);

        var stato = Pulisci(dto.Stato)?.ToUpperInvariant();
        var nota = Pulisci(dto.Nota);

        if (stato == null || !RequestStatusCodes.StatiGestione.Contains(stato))
            return ServiceResult<MaterialRequestDto>.Fail("Stato richiesta non valido. Valori ammessi: APPROVATA, RIFIUTATA, IN_REVISIONE.");

        if (RequestStatusCodes.RichiedeNota(stato) && string.IsNullOrWhiteSpace(nota))
            return ServiceResult<MaterialRequestDto>.Fail("La nota è obbligatoria per rifiutare o mettere in revisione la richiesta.");

        var entity = await _repository.GetForUpdateAsync(id);
        if (entity == null)
            return ServiceResult<MaterialRequestDto>.Fail("Richiesta materiale non trovata.", 404);

        entity.Stato = stato;
        entity.NotaGestione = nota;
        entity.GestitoDaUtenteId = user.UserId;
        entity.GestitoAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();

        return await GetByIdAsync(entity.Id, user);
    }

    private static IQueryable<MaterialRequest>? ApplyVisibility(IQueryable<MaterialRequest> query, UserContext user)
    {
        if (user.IsAdminOrResponsabile())
            return query;

        return user.DipendenteId.HasValue
            ? query.Where(request => request.DipendenteId == user.DipendenteId.Value)
            : null;
    }

    private static ServiceResult<bool> ValidateCreate(CreateMaterialRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.MaterialeRichiesto))
            return ServiceResult<bool>.Fail("Il materiale richiesto è obbligatorio.");
        if (dto.Quantita <= 0)
            return ServiceResult<bool>.Fail("La quantità deve essere maggiore di zero.");
        if (string.IsNullOrWhiteSpace(dto.Motivazione))
            return ServiceResult<bool>.Fail("La motivazione è obbligatoria.");
        if (string.IsNullOrWhiteSpace(dto.Priorita))
            return ServiceResult<bool>.Fail("La priorità è obbligatoria.");
        if (dto.DataDesiderata == default)
            return ServiceResult<bool>.Fail("La data desiderata è obbligatoria.");

        return ServiceResult<bool>.Ok(true);
    }

    private static int? ResolveDipendenteId(int? requestedDipendenteId, UserContext user)
    {
        return user.IsAdminOrResponsabile()
            ? requestedDipendenteId
            : user.DipendenteId;
    }

    private static Expression<Func<MaterialRequest, MaterialRequestDto>> ToDtoExpression()
    {
        return request => new MaterialRequestDto
        {
            Id = request.Id,
            DipendenteId = request.DipendenteId,
            Dipendente = request.Dipendente.Cognome + " " + request.Dipendente.Nome,
            MaterialeRichiesto = request.MaterialeRichiesto,
            Quantita = request.Quantita,
            Motivazione = request.Motivazione,
            Priorita = request.Priorita,
            DataDesiderata = request.DataDesiderata,
            Note = request.Note,
            Stato = request.Stato,
            NotaGestione = request.NotaGestione,
            GestitoDaUtenteId = request.GestitoDaUtenteId,
            GestitoDaUtente = request.GestitoDaUtente != null
                ? request.GestitoDaUtente.Cognome + " " + request.GestitoDaUtente.Nome
                : null,
            GestitoAt = request.GestitoAt,
            CreatedAt = request.CreatedAt,
            UpdatedAt = request.UpdatedAt
        };
    }

    private static string? Pulisci(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}

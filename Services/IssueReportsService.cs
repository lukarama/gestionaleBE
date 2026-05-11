using System.Linq.Expressions;
using Gestionale.Api.Common;
using Gestionale.Api.Common.Auth;
using Gestionale.Api.DTOs;
using Gestionale.Api.Models;
using Gestionale.Api.Repositories.Interfaces;
using Gestionale.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gestionale.Api.Services;

public class IssueReportsService : IIssueReportsService
{
    private readonly IIssueReportRepository _repository;

    public IssueReportsService(IIssueReportRepository repository)
    {
        _repository = repository;
    }

    public async Task<ServiceResult<List<IssueReportDto>>> GetAllAsync(UserContext user)
    {
        var query = ApplyVisibility(_repository.Query().AsNoTracking(), user);
        if (query == null)
            return ServiceResult<List<IssueReportDto>>.Fail("Permessi insufficienti o dipendente non associato all'utente.", 403);

        var result = await query
            .OrderByDescending(report => report.CreatedAt)
            .ThenByDescending(report => report.Id)
            .Select(ToDtoExpression())
            .ToListAsync();

        return ServiceResult<List<IssueReportDto>>.Ok(result);
    }

    public async Task<ServiceResult<IssueReportDto>> GetByIdAsync(int id, UserContext user)
    {
        var query = ApplyVisibility(_repository.Query().AsNoTracking(), user);
        if (query == null)
            return ServiceResult<IssueReportDto>.Fail("Permessi insufficienti o dipendente non associato all'utente.", 403);

        var result = await query
            .Where(report => report.Id == id)
            .Select(ToDtoExpression())
            .FirstOrDefaultAsync();

        if (result == null)
            return ServiceResult<IssueReportDto>.Fail("Segnalazione non trovata.", 404);

        return ServiceResult<IssueReportDto>.Ok(result);
    }

    public async Task<ServiceResult<IssueReportDto>> CreateAsync(CreateIssueReportDto dto, UserContext user)
    {
        var dipendenteId = ResolveDipendenteId(dto.DipendenteId, user);
        if (!dipendenteId.HasValue)
            return ServiceResult<IssueReportDto>.Fail("Dipendente non associato all'utente autenticato.", 403);

        var validation = ValidateCreate(dto);
        if (!validation.Success)
            return ServiceResult<IssueReportDto>.Fail(validation.Message!, validation.StatusCode ?? 400);

        var now = DateTime.UtcNow;
        var entity = new IssueReport
        {
            DipendenteId = dipendenteId.Value,
            Categoria = Pulisci(dto.Categoria)!,
            Oggetto = Pulisci(dto.Oggetto)!,
            Luogo = Pulisci(dto.Luogo),
            Descrizione = Pulisci(dto.Descrizione)!,
            Priorita = Pulisci(dto.Priorita)!,
            Note = Pulisci(dto.Note),
            Stato = RequestStatusCodes.InAttesa,
            CreatedAt = now
        };

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        return await GetByIdAsync(entity.Id, user);
    }

    public async Task<ServiceResult<IssueReportDto>> UpdateStatusAsync(int id, UpdateRequestStatusDto dto, UserContext user)
    {
        if (!user.IsAdminOrResponsabile())
            return ServiceResult<IssueReportDto>.Fail("Permessi insufficienti per gestire la segnalazione.", 403);

        var stato = Pulisci(dto.Stato)?.ToUpperInvariant();
        var nota = Pulisci(dto.Nota);

        if (stato == null || !RequestStatusCodes.StatiGestione.Contains(stato))
            return ServiceResult<IssueReportDto>.Fail("Stato richiesta non valido. Valori ammessi: APPROVATA, RIFIUTATA, IN_REVISIONE.");

        if (RequestStatusCodes.RichiedeNota(stato) && string.IsNullOrWhiteSpace(nota))
            return ServiceResult<IssueReportDto>.Fail("La nota è obbligatoria per rifiutare o mettere in revisione la segnalazione.");

        var entity = await _repository.GetForUpdateAsync(id);
        if (entity == null)
            return ServiceResult<IssueReportDto>.Fail("Segnalazione non trovata.", 404);

        entity.Stato = stato;
        entity.NotaGestione = nota;
        entity.GestitoDaUtenteId = user.UserId;
        entity.GestitoAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();

        return await GetByIdAsync(entity.Id, user);
    }

    private static IQueryable<IssueReport>? ApplyVisibility(IQueryable<IssueReport> query, UserContext user)
    {
        if (user.IsAdminOrResponsabile())
            return query;

        return user.DipendenteId.HasValue
            ? query.Where(report => report.DipendenteId == user.DipendenteId.Value)
            : null;
    }

    private static ServiceResult<bool> ValidateCreate(CreateIssueReportDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Categoria))
            return ServiceResult<bool>.Fail("La categoria è obbligatoria.");
        if (string.IsNullOrWhiteSpace(dto.Oggetto))
            return ServiceResult<bool>.Fail("L'oggetto segnalato è obbligatorio.");
        if (string.IsNullOrWhiteSpace(dto.Descrizione))
            return ServiceResult<bool>.Fail("La descrizione è obbligatoria.");
        if (string.IsNullOrWhiteSpace(dto.Priorita))
            return ServiceResult<bool>.Fail("La priorità è obbligatoria.");

        return ServiceResult<bool>.Ok(true);
    }

    private static int? ResolveDipendenteId(int? requestedDipendenteId, UserContext user)
    {
        return user.IsAdminOrResponsabile()
            ? requestedDipendenteId
            : user.DipendenteId;
    }

    private static Expression<Func<IssueReport, IssueReportDto>> ToDtoExpression()
    {
        return report => new IssueReportDto
        {
            Id = report.Id,
            DipendenteId = report.DipendenteId,
            Dipendente = report.Dipendente.Cognome + " " + report.Dipendente.Nome,
            Categoria = report.Categoria,
            Oggetto = report.Oggetto,
            Luogo = report.Luogo,
            Descrizione = report.Descrizione,
            Priorita = report.Priorita,
            Note = report.Note,
            Stato = report.Stato,
            NotaGestione = report.NotaGestione,
            GestitoDaUtenteId = report.GestitoDaUtenteId,
            GestitoDaUtente = report.GestitoDaUtente != null
                ? report.GestitoDaUtente.Cognome + " " + report.GestitoDaUtente.Nome
                : null,
            GestitoAt = report.GestitoAt,
            CreatedAt = report.CreatedAt,
            UpdatedAt = report.UpdatedAt
        };
    }

    private static string? Pulisci(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}

using System.Linq.Expressions;
using Gestionale.Api.Common;
using Gestionale.Api.Common.Auth;
using Gestionale.Api.DTOs;
using Gestionale.Api.Models;
using Gestionale.Api.Repositories.Interfaces;
using Gestionale.Api.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Gestionale.Api.Services;

public class ExpenseRequestsService : IExpenseRequestsService
{
    private readonly IExpenseRequestRepository _repository;
    private readonly IWebHostEnvironment _environment;

    public ExpenseRequestsService(IExpenseRequestRepository repository, IWebHostEnvironment environment)
    {
        _repository = repository;
        _environment = environment;
    }

    public async Task<ServiceResult<List<ExpenseRequestDto>>> GetAllAsync(UserContext user)
    {
        var query = ApplyVisibility(_repository.Query().AsNoTracking(), user);
        if (query == null)
            return ServiceResult<List<ExpenseRequestDto>>.Fail("Permessi insufficienti o dipendente non associato all'utente.", 403);

        var result = await query
            .OrderByDescending(request => request.CreatedAt)
            .ThenByDescending(request => request.Id)
            .Select(ToDtoExpression())
            .ToListAsync();

        return ServiceResult<List<ExpenseRequestDto>>.Ok(result);
    }

    public async Task<ServiceResult<ExpenseRequestDto>> GetByIdAsync(int id, UserContext user)
    {
        var query = ApplyVisibility(_repository.Query().AsNoTracking(), user);
        if (query == null)
            return ServiceResult<ExpenseRequestDto>.Fail("Permessi insufficienti o dipendente non associato all'utente.", 403);

        var result = await query
            .Where(request => request.Id == id)
            .Select(ToDtoExpression())
            .FirstOrDefaultAsync();

        if (result == null)
            return ServiceResult<ExpenseRequestDto>.Fail("Richiesta rimborso spese non trovata.", 404);

        return ServiceResult<ExpenseRequestDto>.Ok(result);
    }

    public async Task<ServiceResult<ExpenseRequestDto>> CreateAsync(CreateExpenseRequestDto dto, UserContext user)
    {
        var dipendenteId = ResolveDipendenteId(dto.DipendenteId, user);
        if (!dipendenteId.HasValue)
            return ServiceResult<ExpenseRequestDto>.Fail("Dipendente non associato all'utente autenticato.", 403);

        var validation = ValidateCreate(dto);
        if (!validation.Success)
            return ServiceResult<ExpenseRequestDto>.Fail(validation.Message!, validation.StatusCode ?? 400);

        var attachment = await SaveAttachmentAsync(dto, dipendenteId.Value);
        var now = DateTime.UtcNow;
        var entity = new ExpenseRequest
        {
            DipendenteId = dipendenteId.Value,
            DataSpesa = dto.DataSpesa,
            CategoriaSpesa = Pulisci(dto.CategoriaSpesa)!,
            Descrizione = Pulisci(dto.Descrizione)!,
            Importo = dto.Importo,
            Valuta = Pulisci(dto.Valuta)!.ToUpperInvariant(),
            MetodoPagamento = Pulisci(dto.MetodoPagamento)!,
            Stato = RequestStatusCodes.InAttesa,
            AllegatoNomeFile = attachment.NomeFile,
            AllegatoPercorsoFile = attachment.PercorsoFile,
            AllegatoContentType = attachment.ContentType,
            AllegatoEstensione = attachment.Estensione,
            CreatedAt = now
        };

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        return await GetByIdAsync(entity.Id, user);
    }

    public async Task<ServiceResult<ExpenseRequestAttachmentDto>> GetAttachmentAsync(int id, UserContext user)
    {
        var query = ApplyVisibility(_repository.Query().AsNoTracking(), user);
        if (query == null)
            return ServiceResult<ExpenseRequestAttachmentDto>.Fail("Permessi insufficienti o dipendente non associato all'utente.", 403);

        var request = await query.FirstOrDefaultAsync(item => item.Id == id);
        if (request == null)
            return ServiceResult<ExpenseRequestAttachmentDto>.Fail("Richiesta rimborso spese non trovata.", 404);

        if (string.IsNullOrWhiteSpace(request.AllegatoPercorsoFile) || !File.Exists(request.AllegatoPercorsoFile))
            return ServiceResult<ExpenseRequestAttachmentDto>.Fail("Allegato non trovato.", 404);

        var result = new ExpenseRequestAttachmentDto
        {
            FileBytes = await File.ReadAllBytesAsync(request.AllegatoPercorsoFile),
            NomeFile = string.IsNullOrWhiteSpace(request.AllegatoNomeFile)
                ? Path.GetFileName(request.AllegatoPercorsoFile)
                : request.AllegatoNomeFile,
            ContentType = string.IsNullOrWhiteSpace(request.AllegatoContentType)
                ? "application/octet-stream"
                : request.AllegatoContentType
        };

        return ServiceResult<ExpenseRequestAttachmentDto>.Ok(result);
    }

    public async Task<ServiceResult<ExpenseRequestDto>> UpdateStatusAsync(int id, UpdateRequestStatusDto dto, UserContext user)
    {
        if (!user.IsAdminOrResponsabile())
            return ServiceResult<ExpenseRequestDto>.Fail("Permessi insufficienti per gestire la richiesta.", 403);

        var stato = Pulisci(dto.Stato)?.ToUpperInvariant();
        var nota = Pulisci(dto.Nota);

        if (stato == null || !RequestStatusCodes.StatiGestione.Contains(stato))
            return ServiceResult<ExpenseRequestDto>.Fail("Stato richiesta non valido. Valori ammessi: APPROVATA, RIFIUTATA, IN_REVISIONE.");

        if (RequestStatusCodes.RichiedeNota(stato) && string.IsNullOrWhiteSpace(nota))
            return ServiceResult<ExpenseRequestDto>.Fail("La nota è obbligatoria per rifiutare o mettere in revisione la richiesta.");

        var entity = await _repository.GetForUpdateAsync(id);
        if (entity == null)
            return ServiceResult<ExpenseRequestDto>.Fail("Richiesta rimborso spese non trovata.", 404);

        entity.Stato = stato;
        entity.NotaGestione = nota;
        entity.GestitoDaUtenteId = user.UserId;
        entity.GestitoAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();

        return await GetByIdAsync(entity.Id, user);
    }

    private static IQueryable<ExpenseRequest>? ApplyVisibility(IQueryable<ExpenseRequest> query, UserContext user)
    {
        if (user.IsAdminOrResponsabile())
            return query;

        return user.DipendenteId.HasValue
            ? query.Where(request => request.DipendenteId == user.DipendenteId.Value)
            : null;
    }

    private static ServiceResult<bool> ValidateCreate(CreateExpenseRequestDto dto)
    {
        if (dto.DataSpesa == default)
            return ServiceResult<bool>.Fail("La data spesa è obbligatoria.");
        if (string.IsNullOrWhiteSpace(dto.CategoriaSpesa))
            return ServiceResult<bool>.Fail("La categoria spesa è obbligatoria.");
        if (string.IsNullOrWhiteSpace(dto.Descrizione))
            return ServiceResult<bool>.Fail("La descrizione è obbligatoria.");
        if (dto.Importo <= 0)
            return ServiceResult<bool>.Fail("L'importo deve essere maggiore di zero.");
        if (string.IsNullOrWhiteSpace(dto.Valuta))
            return ServiceResult<bool>.Fail("La valuta è obbligatoria.");
        if (string.IsNullOrWhiteSpace(dto.MetodoPagamento))
            return ServiceResult<bool>.Fail("Il metodo di pagamento è obbligatorio.");

        return ServiceResult<bool>.Ok(true);
    }

    private static int? ResolveDipendenteId(int? requestedDipendenteId, UserContext user)
    {
        return user.IsAdminOrResponsabile()
            ? requestedDipendenteId
            : user.DipendenteId;
    }

    private async Task<(string? NomeFile, string? PercorsoFile, string? ContentType, string? Estensione)> SaveAttachmentAsync(CreateExpenseRequestDto dto, int dipendenteId)
    {
        if (dto.Allegato == null || dto.Allegato.Length == 0)
            return (null, null, null, null);

        var folder = Path.Combine(_environment.ContentRootPath, "UploadedFiles", "RimborsiSpese");
        Directory.CreateDirectory(folder);

        var extension = Path.GetExtension(dto.Allegato.FileName);
        var cleanName = Path.GetFileNameWithoutExtension(dto.Allegato.FileName);
        cleanName = string.IsNullOrWhiteSpace(cleanName) ? "allegato" : cleanName.Trim();

        foreach (var invalidChar in Path.GetInvalidFileNameChars())
        {
            cleanName = cleanName.Replace(invalidChar, '_');
        }

        var physicalFileName = $"{dipendenteId}_{DateTime.UtcNow:yyyyMMddHHmmssfff}_{cleanName}{extension}";
        var fullPath = Path.Combine(folder, physicalFileName);

        await using var stream = new FileStream(fullPath, FileMode.CreateNew);
        await dto.Allegato.CopyToAsync(stream);

        return (dto.Allegato.FileName, fullPath, Pulisci(dto.Allegato.ContentType), Pulisci(extension));
    }

    private static Expression<Func<ExpenseRequest, ExpenseRequestDto>> ToDtoExpression()
    {
        return request => new ExpenseRequestDto
        {
            Id = request.Id,
            DipendenteId = request.DipendenteId,
            Dipendente = request.Dipendente.Cognome + " " + request.Dipendente.Nome,
            DataSpesa = request.DataSpesa,
            CategoriaSpesa = request.CategoriaSpesa,
            Descrizione = request.Descrizione,
            Importo = request.Importo,
            Valuta = request.Valuta,
            MetodoPagamento = request.MetodoPagamento,
            Stato = request.Stato,
            AllegatoNomeFile = request.AllegatoNomeFile,
            AllegatoContentType = request.AllegatoContentType,
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

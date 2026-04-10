using Gestionale.Api.Common;
using Gestionale.Api.Data;
using Gestionale.Api.DTOs;
using Gestionale.Api.Models;
using Gestionale.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.IO;


namespace Gestionale.Api.Services;

public class DocumentiDipendentiService : IDocumentiDipendentiService
{
    private readonly AppDbContext _context;

    public DocumentiDipendentiService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<DocumentoDipendenteListDto>> GetAllAsync()
    {
        return await _context.DocumentiDipendentis
            .AsNoTracking()
            .Include(d => d.Dipendente)
            .Include(d => d.TipoDocumento)
            .OrderByDescending(d => d.CreatedAt)
            .ThenByDescending(d => d.Id)
            .Select(d => new DocumentoDipendenteListDto
            {
                Id = d.Id,
                DipendenteId = d.DipendenteId,
                Dipendente = d.Dipendente.Cognome + " " + d.Dipendente.Nome,
                TipoDocumentoId = d.TipoDocumentoId,
                TipoDocumento = d.TipoDocumento != null ? d.TipoDocumento.Nome : null,
                NomeFile = d.NomeFile,
                PercorsoFile = d.PercorsoFile,
                Estensione = d.Estensione,
                ContentType = d.ContentType,
                DataDocumento = d.DataDocumento,
                DataScadenza = d.DataScadenza
            })
            .ToListAsync();
    }

    public async Task<ServiceResult<DocumentoDipendenteDownloadDto>> GetDownloadAsync(int id)
    {
        var entity = await _context.DocumentiDipendentis
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id);

        if (entity == null)
            return ServiceResult<DocumentoDipendenteDownloadDto>.Fail("Documento dipendente non trovato.", 404);

        if (string.IsNullOrWhiteSpace(entity.PercorsoFile))
            return ServiceResult<DocumentoDipendenteDownloadDto>.Fail("Percorso file non valido.", 400);

        if (!File.Exists(entity.PercorsoFile))
            return ServiceResult<DocumentoDipendenteDownloadDto>.Fail("Il file non esiste sul disco.", 404);

        var fileBytes = await File.ReadAllBytesAsync(entity.PercorsoFile);

        var contentType = !string.IsNullOrWhiteSpace(entity.ContentType)
            ? entity.ContentType
            : "application/octet-stream";

        var nomeFile = !string.IsNullOrWhiteSpace(entity.NomeFile)
            ? entity.NomeFile
            : Path.GetFileName(entity.PercorsoFile);

        var result = new DocumentoDipendenteDownloadDto
        {
            FileBytes = fileBytes,
            NomeFile = nomeFile,
            ContentType = contentType
        };

        return ServiceResult<DocumentoDipendenteDownloadDto>.Ok(result);
    }
    public async Task<ServiceResult<DocumentoDipendenteDetailDto>> UploadAsync(UploadDocumentoDipendenteDto dto)
    {
        var validazione = await ValidaRelazioniAsync(dto.DipendenteId, dto.TipoDocumentoId);
        if (!validazione.Success)
            return ServiceResult<DocumentoDipendenteDetailDto>.Fail(validazione.Message!, validazione.StatusCode ?? 400);

        if (dto.File == null || dto.File.Length == 0)
            return ServiceResult<DocumentoDipendenteDetailDto>.Fail("File obbligatorio.", 400);

        if (dto.DataDocumento.HasValue && dto.DataScadenza.HasValue && dto.DataScadenza.Value < dto.DataDocumento.Value)
            return ServiceResult<DocumentoDipendenteDetailDto>.Fail("La data di scadenza non può essere precedente alla data documento.", 400);

        var cartellaBase = @"C:\Documenti\Dipendenti";

        if (!Directory.Exists(cartellaBase))
            Directory.CreateDirectory(cartellaBase);

        var estensione = Path.GetExtension(dto.File.FileName);
        var nomeOriginaleSenzaEstensione = Path.GetFileNameWithoutExtension(dto.File.FileName);

        var nomePulito = string.IsNullOrWhiteSpace(nomeOriginaleSenzaEstensione)
            ? "documento"
            : nomeOriginaleSenzaEstensione.Trim();

        foreach (var c in Path.GetInvalidFileNameChars())
        {
            nomePulito = nomePulito.Replace(c, '_');
        }

        var nomeFileFisico = $"{dto.DipendenteId}_{DateTime.Now:yyyyMMddHHmmssfff}_{nomePulito}{estensione}";
        var percorsoCompleto = Path.Combine(cartellaBase, nomeFileFisico);

        await using (var stream = new FileStream(percorsoCompleto, FileMode.Create))
        {
            await dto.File.CopyToAsync(stream);
        }

        var entity = new DocumentiDipendenti
        {
            DipendenteId = dto.DipendenteId,
            TipoDocumentoId = dto.TipoDocumentoId,
            NomeFile = dto.File.FileName,
            PercorsoFile = percorsoCompleto,
            Estensione = string.IsNullOrWhiteSpace(estensione) ? null : estensione,
            ContentType = string.IsNullOrWhiteSpace(dto.File.ContentType) ? null : dto.File.ContentType,
            DataDocumento = dto.DataDocumento,
            DataScadenza = dto.DataScadenza,
            Note = Pulisci(dto.Note),
            CreatedAt = DateTime.Now
        };

        _context.DocumentiDipendentis.Add(entity);
        await _context.SaveChangesAsync();

        var result = await GetByIdAsync(entity.Id);
        return ServiceResult<DocumentoDipendenteDetailDto>.Created(result!);
    }
    public async Task<DocumentoDipendenteDetailDto?> GetByIdAsync(int id)
    {
        return await _context.DocumentiDipendentis
            .AsNoTracking()
            .Include(d => d.Dipendente)
            .Include(d => d.TipoDocumento)
            .Where(d => d.Id == id)
            .Select(d => new DocumentoDipendenteDetailDto
            {
                Id = d.Id,
                DipendenteId = d.DipendenteId,
                Dipendente = d.Dipendente.Cognome + " " + d.Dipendente.Nome,
                TipoDocumentoId = d.TipoDocumentoId,
                TipoDocumento = d.TipoDocumento != null ? d.TipoDocumento.Nome : null,
                NomeFile = d.NomeFile,
                PercorsoFile = d.PercorsoFile,
                Estensione = d.Estensione,
                ContentType = d.ContentType,
                DataDocumento = d.DataDocumento,
                DataScadenza = d.DataScadenza,
                Note = d.Note,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ServiceResult<DocumentoDipendenteDetailDto>> CreateAsync(CreateDocumentoDipendenteDto dto)
    {
        var validazione = await ValidaRelazioniAsync(dto.DipendenteId, dto.TipoDocumentoId);
        if (!validazione.Success)
            return ServiceResult<DocumentoDipendenteDetailDto>.Fail(validazione.Message!, validazione.StatusCode ?? 400);

        var nomeFile = Pulisci(dto.NomeFile);
        var percorsoFile = Pulisci(dto.PercorsoFile);

        if (string.IsNullOrWhiteSpace(nomeFile))
            return ServiceResult<DocumentoDipendenteDetailDto>.Fail("Il nome file è obbligatorio.", 400);

        if (string.IsNullOrWhiteSpace(percorsoFile))
            return ServiceResult<DocumentoDipendenteDetailDto>.Fail("Il percorso file è obbligatorio.", 400);

        if (dto.DataDocumento.HasValue && dto.DataScadenza.HasValue && dto.DataScadenza.Value < dto.DataDocumento.Value)
            return ServiceResult<DocumentoDipendenteDetailDto>.Fail("La data di scadenza non può essere precedente alla data documento.", 400);

        var entity = new DocumentiDipendenti
        {
            DipendenteId = dto.DipendenteId,
            TipoDocumentoId = dto.TipoDocumentoId,
            NomeFile = nomeFile,
            PercorsoFile = percorsoFile,
            Estensione = Pulisci(dto.Estensione),
            ContentType = Pulisci(dto.ContentType),
            DataDocumento = dto.DataDocumento,
            DataScadenza = dto.DataScadenza,
            Note = Pulisci(dto.Note),
            CreatedAt = DateTime.Now
        };

        _context.DocumentiDipendentis.Add(entity);
        await _context.SaveChangesAsync();

        var result = await GetByIdAsync(entity.Id);
        return ServiceResult<DocumentoDipendenteDetailDto>.Created(result!);
    }

    public async Task<ServiceResult<bool>> UpdateAsync(int id, UpdateDocumentoDipendenteDto dto)
    {
        var entity = await _context.DocumentiDipendentis
            .FirstOrDefaultAsync(d => d.Id == id);

        if (entity == null)
            return ServiceResult<bool>.Fail("Documento dipendente non trovato.", 404);

        var validazione = await ValidaRelazioniAsync(dto.DipendenteId, dto.TipoDocumentoId);
        if (!validazione.Success)
            return ServiceResult<bool>.Fail(validazione.Message!, validazione.StatusCode ?? 400);

        var nomeFile = Pulisci(dto.NomeFile);
        var percorsoFile = Pulisci(dto.PercorsoFile);

        if (string.IsNullOrWhiteSpace(nomeFile))
            return ServiceResult<bool>.Fail("Il nome file è obbligatorio.", 400);

        if (string.IsNullOrWhiteSpace(percorsoFile))
            return ServiceResult<bool>.Fail("Il percorso file è obbligatorio.", 400);

        if (dto.DataDocumento.HasValue && dto.DataScadenza.HasValue && dto.DataScadenza.Value < dto.DataDocumento.Value)
            return ServiceResult<bool>.Fail("La data di scadenza non può essere precedente alla data documento.", 400);

        entity.DipendenteId = dto.DipendenteId;
        entity.TipoDocumentoId = dto.TipoDocumentoId;
        entity.NomeFile = nomeFile;
        entity.PercorsoFile = percorsoFile;
        entity.Estensione = Pulisci(dto.Estensione);
        entity.ContentType = Pulisci(dto.ContentType);
        entity.DataDocumento = dto.DataDocumento;
        entity.DataScadenza = dto.DataScadenza;
        entity.Note = Pulisci(dto.Note);
        entity.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var entity = await _context.DocumentiDipendentis
            .FirstOrDefaultAsync(d => d.Id == id);

        if (entity == null)
            return ServiceResult<bool>.Fail("Documento dipendente non trovato.", 404);

        try
        {
            if (!string.IsNullOrWhiteSpace(entity.PercorsoFile) && File.Exists(entity.PercorsoFile))
            {
                File.Delete(entity.PercorsoFile);
            }

            _context.DocumentiDipendentis.Remove(entity);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Fail($"Errore durante l'eliminazione del documento: {ex.Message}", 500);
        }
    }

    public async Task<List<DocumentoDipendenteListDto>> GetByDipendenteIdAsync(int dipendenteId)
    {
        return await _context.DocumentiDipendentis
            .AsNoTracking()
            .Include(d => d.Dipendente)
            .Include(d => d.TipoDocumento)
            .Where(d => d.DipendenteId == dipendenteId)
            .OrderByDescending(d => d.DataDocumento)
            .ThenByDescending(d => d.Id)
            .Select(d => new DocumentoDipendenteListDto
            {
                Id = d.Id,
                DipendenteId = d.DipendenteId,
                Dipendente = d.Dipendente.Cognome + " " + d.Dipendente.Nome,
                TipoDocumentoId = d.TipoDocumentoId,
                TipoDocumento = d.TipoDocumento != null ? d.TipoDocumento.Nome : null,
                NomeFile = d.NomeFile,
                PercorsoFile = d.PercorsoFile,
                Estensione = d.Estensione,
                ContentType = d.ContentType,
                DataDocumento = d.DataDocumento,
                DataScadenza = d.DataScadenza
            })
            .ToListAsync();
    }

    public async Task<List<DocumentoDipendenteScadenzaDto>> GetInScadenzaAsync(int giorni = 30)
    {
        if (giorni <= 0)
            giorni = 30;

        var oggi = DateOnly.FromDateTime(DateTime.Today);
        var limite = oggi.AddDays(giorni);

        return await _context.DocumentiDipendentis
            .AsNoTracking()
            .Include(d => d.Dipendente)
            .Include(d => d.TipoDocumento)
            .Where(d => d.DataScadenza.HasValue && d.DataScadenza.Value <= limite)
            .OrderBy(d => d.DataScadenza)
            .ThenBy(d => d.Dipendente.Cognome)
            .ThenBy(d => d.Dipendente.Nome)
            .Select(d => new DocumentoDipendenteScadenzaDto
            {
                Id = d.Id,
                DipendenteId = d.DipendenteId,
                Dipendente = d.Dipendente.Cognome + " " + d.Dipendente.Nome,
                TipoDocumentoId = d.TipoDocumentoId,
                TipoDocumento = d.TipoDocumento != null ? d.TipoDocumento.Nome : null,
                NomeFile = d.NomeFile,
                DataScadenza = d.DataScadenza,
                GiorniAllaScadenza = d.DataScadenza.HasValue ? d.DataScadenza.Value.DayNumber - oggi.DayNumber : 0,
                Scaduto = d.DataScadenza.HasValue && d.DataScadenza.Value < oggi,
                InScadenza = d.DataScadenza.HasValue && d.DataScadenza.Value >= oggi && d.DataScadenza.Value <= limite
            })
            .ToListAsync();
    }

    private async Task<ServiceResult<bool>> ValidaRelazioniAsync(int dipendenteId, int? tipoDocumentoId)
    {
        var dipendenteEsiste = await _context.Dipendentis
            .AnyAsync(d => d.Id == dipendenteId && d.Attivo);

        if (!dipendenteEsiste)
            return ServiceResult<bool>.Fail("Il dipendente selezionato non esiste o non è attivo.", 400);

        if (tipoDocumentoId.HasValue)
        {
            var tipoEsiste = await _context.TipiDocumentos
                .AnyAsync(t => t.Id == tipoDocumentoId.Value && t.Attivo);

            if (!tipoEsiste)
                return ServiceResult<bool>.Fail("Il tipo documento selezionato non esiste o non è attivo.", 400);
        }

        return ServiceResult<bool>.Ok(true);
    }

    private static string? Pulisci(string? valore)
    {
        return string.IsNullOrWhiteSpace(valore) ? null : valore.Trim();
    }
}
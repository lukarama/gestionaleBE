using System.IO;
using Gestionale.Api.Common;
using Gestionale.Api.Data;
using Gestionale.Api.DTOs;
using Gestionale.Api.Models;
using Gestionale.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gestionale.Api.Services;

public class DocumentiCantieriService : IDocumentiCantieriService
{
    private readonly AppDbContext _context;

    public DocumentiCantieriService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<DocumentoCantiereListDto>> GetAllAsync()
    {
        return await _context.DocumentiCantieris
            .AsNoTracking()
            .Include(d => d.Cantiere)
            .OrderByDescending(d => d.CreatedAt)
            .ThenByDescending(d => d.Id)
            .Select(d => new DocumentoCantiereListDto
            {
                Id = d.Id,
                CantiereId = d.CantiereId,
                Cantiere = d.Cantiere.Nome,
                NomeFile = d.NomeFile,
                PercorsoFile = d.PercorsoFile,
                Estensione = d.Estensione,
                ContentType = d.ContentType,
                DataDocumento = d.DataDocumento
            })
            .ToListAsync();
    }

    public async Task<DocumentoCantiereDetailDto?> GetByIdAsync(int id)
    {
        return await _context.DocumentiCantieris
            .AsNoTracking()
            .Include(d => d.Cantiere)
            .Where(d => d.Id == id)
            .Select(d => new DocumentoCantiereDetailDto
            {
                Id = d.Id,
                CantiereId = d.CantiereId,
                Cantiere = d.Cantiere.Nome,
                NomeFile = d.NomeFile,
                PercorsoFile = d.PercorsoFile,
                Estensione = d.Estensione,
                ContentType = d.ContentType,
                DataDocumento = d.DataDocumento,
                Note = d.Note,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<List<DocumentoCantiereListDto>> GetByCantiereIdAsync(int cantiereId)
    {
        return await _context.DocumentiCantieris
            .AsNoTracking()
            .Include(d => d.Cantiere)
            .Where(d => d.CantiereId == cantiereId)
            .OrderByDescending(d => d.DataDocumento)
            .ThenByDescending(d => d.Id)
            .Select(d => new DocumentoCantiereListDto
            {
                Id = d.Id,
                CantiereId = d.CantiereId,
                Cantiere = d.Cantiere.Nome,
                NomeFile = d.NomeFile,
                PercorsoFile = d.PercorsoFile,
                Estensione = d.Estensione,
                ContentType = d.ContentType,
                DataDocumento = d.DataDocumento
            })
            .ToListAsync();
    }

    public async Task<ServiceResult<DocumentoCantiereDetailDto>> CreateAsync(CreateDocumentoCantiereDto dto)
    {
        var validazione = await ValidaCantiereAsync(dto.CantiereId);
        if (!validazione.Success)
            return ServiceResult<DocumentoCantiereDetailDto>.Fail(validazione.Message!, validazione.StatusCode ?? 400);

        var nomeFile = Pulisci(dto.NomeFile);
        var percorsoFile = Pulisci(dto.PercorsoFile);

        if (string.IsNullOrWhiteSpace(nomeFile))
            return ServiceResult<DocumentoCantiereDetailDto>.Fail("Il nome file è obbligatorio.", 400);

        if (string.IsNullOrWhiteSpace(percorsoFile))
            return ServiceResult<DocumentoCantiereDetailDto>.Fail("Il percorso file è obbligatorio.", 400);

        var entity = new DocumentiCantieri
        {
            CantiereId = dto.CantiereId,
            NomeFile = nomeFile,
            PercorsoFile = percorsoFile,
            Estensione = Pulisci(dto.Estensione),
            ContentType = Pulisci(dto.ContentType),
            DataDocumento = dto.DataDocumento,
            Note = Pulisci(dto.Note),
            CreatedAt = DateTime.Now
        };

        _context.DocumentiCantieris.Add(entity);
        await _context.SaveChangesAsync();

        var result = await GetByIdAsync(entity.Id);
        return ServiceResult<DocumentoCantiereDetailDto>.Created(result!);
    }

    public async Task<ServiceResult<DocumentoCantiereDetailDto>> UploadAsync(UploadDocumentoCantiereDto dto)
    {
        var validazione = await ValidaCantiereAsync(dto.CantiereId);
        if (!validazione.Success)
            return ServiceResult<DocumentoCantiereDetailDto>.Fail(validazione.Message!, validazione.StatusCode ?? 400);

        if (dto.File == null || dto.File.Length == 0)
            return ServiceResult<DocumentoCantiereDetailDto>.Fail("File obbligatorio.", 400);

        var cartellaBase = @"C:\Documenti\Cantieri";

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

        var nomeFileFisico = $"{dto.CantiereId}_{DateTime.Now:yyyyMMddHHmmssfff}_{nomePulito}{estensione}";
        var percorsoCompleto = Path.Combine(cartellaBase, nomeFileFisico);

        await using (var stream = new FileStream(percorsoCompleto, FileMode.Create))
        {
            await dto.File.CopyToAsync(stream);
        }

        var entity = new DocumentiCantieri
        {
            CantiereId = dto.CantiereId,
            NomeFile = dto.File.FileName,
            PercorsoFile = percorsoCompleto,
            Estensione = string.IsNullOrWhiteSpace(estensione) ? null : estensione,
            ContentType = string.IsNullOrWhiteSpace(dto.File.ContentType) ? null : dto.File.ContentType,
            DataDocumento = dto.DataDocumento,
            Note = Pulisci(dto.Note),
            CreatedAt = DateTime.Now
        };

        _context.DocumentiCantieris.Add(entity);
        await _context.SaveChangesAsync();

        var result = await GetByIdAsync(entity.Id);
        return ServiceResult<DocumentoCantiereDetailDto>.Created(result!);
    }

    public async Task<ServiceResult<DocumentoCantiereDownloadDto>> GetDownloadAsync(int id)
    {
        var entity = await _context.DocumentiCantieris
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id);

        if (entity == null)
            return ServiceResult<DocumentoCantiereDownloadDto>.Fail("Documento cantiere non trovato.", 404);

        if (string.IsNullOrWhiteSpace(entity.PercorsoFile))
            return ServiceResult<DocumentoCantiereDownloadDto>.Fail("Percorso file non valido.", 400);

        if (!File.Exists(entity.PercorsoFile))
            return ServiceResult<DocumentoCantiereDownloadDto>.Fail("Il file non esiste sul disco.", 404);

        var fileBytes = await File.ReadAllBytesAsync(entity.PercorsoFile);
        var contentType = !string.IsNullOrWhiteSpace(entity.ContentType)
            ? entity.ContentType
            : "application/octet-stream";
        var nomeFile = !string.IsNullOrWhiteSpace(entity.NomeFile)
            ? entity.NomeFile
            : Path.GetFileName(entity.PercorsoFile);

        return ServiceResult<DocumentoCantiereDownloadDto>.Ok(new DocumentoCantiereDownloadDto
        {
            FileBytes = fileBytes,
            NomeFile = nomeFile,
            ContentType = contentType
        });
    }

    public async Task<ServiceResult<bool>> UpdateAsync(int id, UpdateDocumentoCantiereDto dto)
    {
        var entity = await _context.DocumentiCantieris
            .FirstOrDefaultAsync(d => d.Id == id);

        if (entity == null)
            return ServiceResult<bool>.Fail("Documento cantiere non trovato.", 404);

        var validazione = await ValidaCantiereAsync(dto.CantiereId);
        if (!validazione.Success)
            return ServiceResult<bool>.Fail(validazione.Message!, validazione.StatusCode ?? 400);

        var nomeFile = Pulisci(dto.NomeFile);
        var percorsoFile = Pulisci(dto.PercorsoFile);

        if (string.IsNullOrWhiteSpace(nomeFile))
            return ServiceResult<bool>.Fail("Il nome file è obbligatorio.", 400);

        if (string.IsNullOrWhiteSpace(percorsoFile))
            return ServiceResult<bool>.Fail("Il percorso file è obbligatorio.", 400);

        entity.CantiereId = dto.CantiereId;
        entity.NomeFile = nomeFile;
        entity.PercorsoFile = percorsoFile;
        entity.Estensione = Pulisci(dto.Estensione);
        entity.ContentType = Pulisci(dto.ContentType);
        entity.DataDocumento = dto.DataDocumento;
        entity.Note = Pulisci(dto.Note);
        entity.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var entity = await _context.DocumentiCantieris
            .FirstOrDefaultAsync(d => d.Id == id);

        if (entity == null)
            return ServiceResult<bool>.Fail("Documento cantiere non trovato.", 404);

        try
        {
            if (!string.IsNullOrWhiteSpace(entity.PercorsoFile) && File.Exists(entity.PercorsoFile))
            {
                File.Delete(entity.PercorsoFile);
            }

            _context.DocumentiCantieris.Remove(entity);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Fail($"Errore durante l'eliminazione del documento: {ex.Message}", 500);
        }
    }

    private async Task<ServiceResult<bool>> ValidaCantiereAsync(int cantiereId)
    {
        var cantiereEsiste = await _context.Cantieris
            .AnyAsync(c => c.Id == cantiereId && c.Attivo);

        if (!cantiereEsiste)
            return ServiceResult<bool>.Fail("Il cantiere selezionato non esiste o non è attivo.", 400);

        return ServiceResult<bool>.Ok(true);
    }

    private static string? Pulisci(string? valore)
    {
        return string.IsNullOrWhiteSpace(valore) ? null : valore.Trim();
    }
}

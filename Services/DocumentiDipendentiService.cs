using Gestionale.Api.Common;
using Gestionale.Api.Common.Auth;
using Gestionale.Api.Data;
using Gestionale.Api.DTOs;
using Gestionale.Api.Models;
using Gestionale.Api.Options;
using Gestionale.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Gestionale.Api.Services;

public class DocumentiDipendentiService : IDocumentiDipendentiService
{
    private readonly AppDbContext _context;
    private readonly IDocumentiDipendentiStorageService _storageService;
    private readonly DocumentiDipendentiOptions _options;

    public DocumentiDipendentiService(
        AppDbContext context,
        IDocumentiDipendentiStorageService storageService,
        IOptions<DocumentiDipendentiOptions> options)
    {
        _context = context;
        _storageService = storageService;
        _options = options.Value;
    }

    public async Task<List<DocumentoDipendenteListDto>> GetAllAsync()
    {
        return await BaseDocumentiQuery()
            .OrderByDescending(d => d.CreatedAt)
            .ThenByDescending(d => d.Id)
            .Select(d => ToListDto(d))
            .ToListAsync();
    }

    public async Task<ServiceResult<List<DipendenteSelectDto>>> GetDipendentiDocumentiSelectAsync(UserContext user)
    {
        if (!CanManageAll(user) && !CanReadAll(user))
            return ServiceResult<List<DipendenteSelectDto>>.Fail("Permessi insufficienti.", 403);

        var result = await _context.Dipendentis
            .AsNoTracking()
            .Where(d => d.Attivo)
            .OrderBy(d => d.Cognome)
            .ThenBy(d => d.Nome)
            .Select(d => new DipendenteSelectDto
            {
                Id = d.Id,
                Label = d.Cognome + " " + d.Nome,
                NomeCompleto = d.Cognome + " " + d.Nome
            })
            .ToListAsync();

        return ServiceResult<List<DipendenteSelectDto>>.Ok(result);
    }

    public async Task<ServiceResult<DocumentiDipendenteTreeDto>> GetTreeByDipendenteIdAsync(int dipendenteId, UserContext user)
    {
        if (!CanAccessDipendente(dipendenteId, user, allowAllRead: true))
            return ServiceResult<DocumentiDipendenteTreeDto>.Fail("Permessi insufficienti.", 403);

        var exists = await _context.Dipendentis.AnyAsync(d => d.Id == dipendenteId && d.Attivo);
        if (!exists)
            return ServiceResult<DocumentiDipendenteTreeDto>.Fail("Dipendente non trovato.", 404);

        var cartelle = await _context.CartelleDocumentiDipendentis
            .AsNoTracking()
            .Where(c => c.DipendenteId == dipendenteId)
            .OrderBy(c => c.ParentCartellaId)
            .ThenBy(c => c.Nome)
            .Select(c => new CartellaDocumentoDipendenteDto
            {
                Id = c.Id,
                DipendenteId = c.DipendenteId,
                ParentCartellaId = c.ParentCartellaId,
                Nome = c.Nome,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                CreatedByUtenteId = c.CreatedByUtenteId
            })
            .ToListAsync();

        var documenti = await BaseDocumentiQuery()
            .Where(d => d.DipendenteId == dipendenteId)
            .OrderBy(d => d.CartellaId)
            .ThenByDescending(d => d.CreatedAt)
            .ThenBy(d => d.NomeFile)
            .Select(d => ToListDto(d))
            .ToListAsync();

        return ServiceResult<DocumentiDipendenteTreeDto>.Ok(new DocumentiDipendenteTreeDto
        {
            DipendenteId = dipendenteId,
            Cartelle = cartelle,
            Documenti = documenti
        });
    }

    public Task<ServiceResult<DocumentiDipendenteTreeDto>> GetMyTreeAsync(UserContext user)
    {
        if (!user.DipendenteId.HasValue)
            return Task.FromResult(ServiceResult<DocumentiDipendenteTreeDto>.Fail("Dipendente non associato all'utente.", 403));

        return GetTreeByDipendenteIdAsync(user.DipendenteId.Value, user);
    }

    public async Task<DocumentoDipendenteDetailDto?> GetByIdAsync(int id)
    {
        return await BaseDocumentiQuery()
            .Where(d => d.Id == id)
            .Select(d => ToDetailDto(d))
            .FirstOrDefaultAsync();
    }

    public async Task<ServiceResult<CartellaDocumentoDipendenteDto>> CreateCartellaAsync(int dipendenteId, CreateCartellaDocumentoDipendenteDto dto, UserContext user)
    {
        if (!CanManageAll(user))
            return ServiceResult<CartellaDocumentoDipendenteDto>.Fail("Permessi insufficienti.", 403);

        var nome = Pulisci(dto.Nome);
        if (string.IsNullOrWhiteSpace(nome))
            return ServiceResult<CartellaDocumentoDipendenteDto>.Fail("Il nome cartella è obbligatorio.", 400);

        if (nome.Length > 150)
            return ServiceResult<CartellaDocumentoDipendenteDto>.Fail("Il nome cartella non può superare 150 caratteri.", 400);

        var dipendente = await _context.Dipendentis.FirstOrDefaultAsync(d => d.Id == dipendenteId && d.Attivo);
        if (dipendente == null)
            return ServiceResult<CartellaDocumentoDipendenteDto>.Fail("Dipendente non trovato.", 404);

        if (dto.ParentCartellaId.HasValue)
        {
            var parentOk = await _context.CartelleDocumentiDipendentis
                .AnyAsync(c => c.Id == dto.ParentCartellaId.Value && c.DipendenteId == dipendenteId);
            if (!parentOk)
                return ServiceResult<CartellaDocumentoDipendenteDto>.Fail("Cartella padre non valida.", 400);
        }

        var duplicate = await _context.CartelleDocumentiDipendentis.AnyAsync(c =>
            c.DipendenteId == dipendenteId &&
            c.ParentCartellaId == dto.ParentCartellaId &&
            c.Nome == nome);
        if (duplicate)
            return ServiceResult<CartellaDocumentoDipendenteDto>.Fail("Esiste già una cartella con questo nome.", 409);

        var entity = new CartelleDocumentiDipendenti
        {
            DipendenteId = dipendenteId,
            ParentCartellaId = dto.ParentCartellaId,
            Nome = nome,
            CreatedAt = DateTime.Now,
            CreatedByUtenteId = user.UserId
        };

        _context.CartelleDocumentiDipendentis.Add(entity);
        await _context.SaveChangesAsync();

        return ServiceResult<CartellaDocumentoDipendenteDto>.Created(ToCartellaDto(entity));
    }

    public async Task<ServiceResult<CartellaDocumentoDipendenteDto>> RenameCartellaAsync(int cartellaId, UpdateCartellaDocumentoDipendenteDto dto, UserContext user)
    {
        if (!CanManageAll(user))
            return ServiceResult<CartellaDocumentoDipendenteDto>.Fail("Permessi insufficienti.", 403);

        var entity = await _context.CartelleDocumentiDipendentis.FirstOrDefaultAsync(c => c.Id == cartellaId);
        if (entity == null)
            return ServiceResult<CartellaDocumentoDipendenteDto>.Fail("Cartella non trovata.", 404);

        var nome = Pulisci(dto.Nome);
        if (string.IsNullOrWhiteSpace(nome))
            return ServiceResult<CartellaDocumentoDipendenteDto>.Fail("Il nome cartella è obbligatorio.", 400);

        var duplicate = await _context.CartelleDocumentiDipendentis.AnyAsync(c =>
            c.Id != cartellaId &&
            c.DipendenteId == entity.DipendenteId &&
            c.ParentCartellaId == entity.ParentCartellaId &&
            c.Nome == nome);
        if (duplicate)
            return ServiceResult<CartellaDocumentoDipendenteDto>.Fail("Esiste già una cartella con questo nome.", 409);

        entity.Nome = nome;
        entity.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();

        return ServiceResult<CartellaDocumentoDipendenteDto>.Ok(ToCartellaDto(entity));
    }

    public async Task<ServiceResult<bool>> DeleteCartellaAsync(int cartellaId, UserContext user)
    {
        if (!CanManageAll(user))
            return ServiceResult<bool>.Fail("Permessi insufficienti.", 403);

        var entity = await _context.CartelleDocumentiDipendentis.FirstOrDefaultAsync(c => c.Id == cartellaId);
        if (entity == null)
            return ServiceResult<bool>.Fail("Cartella non trovata.", 404);

        var hasChildren = await _context.CartelleDocumentiDipendentis.AnyAsync(c => c.ParentCartellaId == cartellaId);
        var hasDocuments = await _context.DocumentiDipendentis.AnyAsync(d => d.CartellaId == cartellaId);
        if (hasChildren || hasDocuments)
            return ServiceResult<bool>.Fail("La cartella contiene sottocartelle o documenti.", 409);

        _context.CartelleDocumentiDipendentis.Remove(entity);
        await _context.SaveChangesAsync();

        return ServiceResult<bool>.Ok(true);
    }

    public async Task<ServiceResult<DocumentoDipendenteDetailDto>> UploadAsync(UploadDocumentoDipendenteDto dto, UserContext user)
    {
        if (!CanManageAll(user))
            return ServiceResult<DocumentoDipendenteDetailDto>.Fail("Permessi insufficienti.", 403);

        var validazione = await ValidaRelazioniAsync(dto.DipendenteId, dto.TipoDocumentoId, dto.CartellaId);
        if (!validazione.Success)
            return ServiceResult<DocumentoDipendenteDetailDto>.Fail(validazione.Message!, validazione.StatusCode ?? 400);

        var fileValidation = ValidateFile(dto.File);
        if (!fileValidation.Success)
            return ServiceResult<DocumentoDipendenteDetailDto>.Fail(fileValidation.Message!, fileValidation.StatusCode ?? 400);

        if (dto.DataDocumento.HasValue && dto.DataScadenza.HasValue && dto.DataScadenza.Value < dto.DataDocumento.Value)
            return ServiceResult<DocumentoDipendenteDetailDto>.Fail("La data di scadenza non può essere precedente alla data documento.", 400);

        var dipendente = await _context.Dipendentis.FirstAsync(d => d.Id == dto.DipendenteId);
        var cartella = dto.CartellaId.HasValue
            ? await _context.CartelleDocumentiDipendentis.FirstAsync(c => c.Id == dto.CartellaId.Value)
            : null;

        var saved = await _storageService.SaveFileAsync(dipendente, cartella, dto.File);
        var originalFileName = Path.GetFileName(dto.File.FileName);
        var extension = Path.GetExtension(originalFileName).ToLowerInvariant();

        var entity = new DocumentiDipendenti
        {
            DipendenteId = dto.DipendenteId,
            CartellaId = dto.CartellaId,
            TipoDocumentoId = dto.TipoDocumentoId,
            NomeFile = originalFileName,
            NomeFileSalvato = saved.SavedFileName,
            PercorsoFile = saved.FullPath,
            Estensione = string.IsNullOrWhiteSpace(extension) ? null : extension,
            ContentType = string.IsNullOrWhiteSpace(dto.File.ContentType) ? null : dto.File.ContentType,
            DimensioneBytes = dto.File.Length,
            DataDocumento = dto.DataDocumento,
            DataScadenza = dto.DataScadenza,
            Note = Pulisci(dto.Note),
            CreatedAt = DateTime.Now,
            UploadedByUtenteId = user.UserId
        };

        _context.DocumentiDipendentis.Add(entity);
        await _context.SaveChangesAsync();

        var result = await GetByIdAsync(entity.Id);
        return ServiceResult<DocumentoDipendenteDetailDto>.Created(result!);
    }

    public async Task<ServiceResult<DocumentoDipendenteDownloadDto>> GetDownloadAsync(int id, UserContext user)
    {
        var entity = await _context.DocumentiDipendentis
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id);

        if (entity == null)
            return ServiceResult<DocumentoDipendenteDownloadDto>.Fail("Documento dipendente non trovato.", 404);

        if (!CanAccessDipendente(entity.DipendenteId, user, allowAllRead: true))
            return ServiceResult<DocumentoDipendenteDownloadDto>.Fail("Permessi insufficienti.", 403);

        if (string.IsNullOrWhiteSpace(entity.PercorsoFile))
            return ServiceResult<DocumentoDipendenteDownloadDto>.Fail("Percorso file non valido.", 400);

        if (!File.Exists(entity.PercorsoFile))
            return ServiceResult<DocumentoDipendenteDownloadDto>.Fail("Il file non esiste sul disco.", 404);

        var fileBytes = await File.ReadAllBytesAsync(entity.PercorsoFile);
        return ServiceResult<DocumentoDipendenteDownloadDto>.Ok(new DocumentoDipendenteDownloadDto
        {
            FileBytes = fileBytes,
            NomeFile = !string.IsNullOrWhiteSpace(entity.NomeFile) ? entity.NomeFile : Path.GetFileName(entity.PercorsoFile),
            ContentType = !string.IsNullOrWhiteSpace(entity.ContentType) ? entity.ContentType : "application/octet-stream"
        });
    }

    public async Task<ServiceResult<DocumentoDipendenteDownloadDto>> GetFileDownloadByDipendenteIdAsync(int dipendenteId, string nomeFile)
    {
        var dipendente = await _context.Dipendentis
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == dipendenteId && d.Attivo);

        if (dipendente == null)
            return ServiceResult<DocumentoDipendenteDownloadDto>.Fail("Dipendente non trovato.", 404);

        var result = _storageService.GetDipendenteFile(dipendente, nomeFile);
        if (result == null)
            return ServiceResult<DocumentoDipendenteDownloadDto>.Fail("File non trovato nella cartella del dipendente.", 404);

        return ServiceResult<DocumentoDipendenteDownloadDto>.Ok(result);
    }

    public async Task<ServiceResult<DocumentoDipendenteDetailDto>> CreateAsync(CreateDocumentoDipendenteDto dto)
    {
        var validazione = await ValidaRelazioniAsync(dto.DipendenteId, dto.TipoDocumentoId, dto.CartellaId);
        if (!validazione.Success)
            return ServiceResult<DocumentoDipendenteDetailDto>.Fail(validazione.Message!, validazione.StatusCode ?? 400);

        var nomeFile = Path.GetFileName(Pulisci(dto.NomeFile) ?? string.Empty);
        var percorsoFile = Pulisci(dto.PercorsoFile);
        if (string.IsNullOrWhiteSpace(nomeFile) || string.IsNullOrWhiteSpace(percorsoFile))
            return ServiceResult<DocumentoDipendenteDetailDto>.Fail("Nome file e percorso file sono obbligatori.", 400);

        var entity = new DocumentiDipendenti
        {
            DipendenteId = dto.DipendenteId,
            CartellaId = dto.CartellaId,
            TipoDocumentoId = dto.TipoDocumentoId,
            NomeFile = nomeFile,
            PercorsoFile = percorsoFile,
            Estensione = Pulisci(dto.Estensione),
            ContentType = Pulisci(dto.ContentType),
            DimensioneBytes = Math.Max(0, dto.DimensioneBytes),
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
        var entity = await _context.DocumentiDipendentis.FirstOrDefaultAsync(d => d.Id == id);
        if (entity == null)
            return ServiceResult<bool>.Fail("Documento dipendente non trovato.", 404);

        var validazione = await ValidaRelazioniAsync(dto.DipendenteId, dto.TipoDocumentoId, dto.CartellaId);
        if (!validazione.Success)
            return ServiceResult<bool>.Fail(validazione.Message!, validazione.StatusCode ?? 400);

        entity.DipendenteId = dto.DipendenteId;
        entity.CartellaId = dto.CartellaId;
        entity.TipoDocumentoId = dto.TipoDocumentoId;
        entity.NomeFile = Path.GetFileName(Pulisci(dto.NomeFile) ?? entity.NomeFile);
        entity.PercorsoFile = Pulisci(dto.PercorsoFile) ?? entity.PercorsoFile;
        entity.Estensione = Pulisci(dto.Estensione);
        entity.ContentType = Pulisci(dto.ContentType);
        entity.DataDocumento = dto.DataDocumento;
        entity.DataScadenza = dto.DataScadenza;
        entity.Note = Pulisci(dto.Note);
        entity.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }

    public Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        return DeleteInternalAsync(id);
    }

    public Task<ServiceResult<bool>> DeleteAsync(int id, UserContext user)
    {
        if (!CanManageAll(user))
            return Task.FromResult(ServiceResult<bool>.Fail("Permessi insufficienti.", 403));

        return DeleteInternalAsync(id);
    }

    public async Task<ServiceResult<bool>> RenameDocumentoAsync(int id, RenameDocumentoDipendenteDto dto, UserContext user)
    {
        if (!CanManageAll(user))
            return ServiceResult<bool>.Fail("Permessi insufficienti.", 403);

        var entity = await _context.DocumentiDipendentis.FirstOrDefaultAsync(d => d.Id == id);
        if (entity == null)
            return ServiceResult<bool>.Fail("Documento dipendente non trovato.", 404);

        var nomeFile = Path.GetFileName(Pulisci(dto.NomeFile) ?? string.Empty);
        if (string.IsNullOrWhiteSpace(nomeFile))
            return ServiceResult<bool>.Fail("Il nome file è obbligatorio.", 400);

        entity.NomeFile = nomeFile;
        entity.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();

        return ServiceResult<bool>.Ok(true);
    }

    public async Task<List<DocumentoDipendenteListDto>> GetByDipendenteIdAsync(int dipendenteId)
    {
        return await BaseDocumentiQuery()
            .Where(d => d.DipendenteId == dipendenteId)
            .OrderByDescending(d => d.DataDocumento)
            .ThenByDescending(d => d.Id)
            .Select(d => ToListDto(d))
            .ToListAsync();
    }

    public async Task<ServiceResult<List<DocumentoDipendenteFileDto>>> GetFilesByDipendenteIdAsync(int dipendenteId)
    {
        var dipendente = await _context.Dipendentis
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == dipendenteId && d.Attivo);

        if (dipendente == null)
            return ServiceResult<List<DocumentoDipendenteFileDto>>.Fail("Dipendente non trovato.", 404);

        var files = _storageService.GetDipendenteFiles(dipendente);
        return ServiceResult<List<DocumentoDipendenteFileDto>>.Ok(files);
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

    private async Task<ServiceResult<bool>> DeleteInternalAsync(int id)
    {
        var entity = await _context.DocumentiDipendentis.FirstOrDefaultAsync(d => d.Id == id);
        if (entity == null)
            return ServiceResult<bool>.Fail("Documento dipendente non trovato.", 404);

        try
        {
            _storageService.DeleteFile(entity.PercorsoFile);
            _context.DocumentiDipendentis.Remove(entity);
            await _context.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return ServiceResult<bool>.Fail($"Errore durante l'eliminazione del documento: {ex.Message}", 500);
        }
    }

    private async Task<ServiceResult<bool>> ValidaRelazioniAsync(int dipendenteId, int? tipoDocumentoId, int? cartellaId)
    {
        var dipendenteEsiste = await _context.Dipendentis.AnyAsync(d => d.Id == dipendenteId && d.Attivo);
        if (!dipendenteEsiste)
            return ServiceResult<bool>.Fail("Il dipendente selezionato non esiste o non è attivo.", 400);

        if (cartellaId.HasValue)
        {
            var cartellaOk = await _context.CartelleDocumentiDipendentis
                .AnyAsync(c => c.Id == cartellaId.Value && c.DipendenteId == dipendenteId);
            if (!cartellaOk)
                return ServiceResult<bool>.Fail("La cartella selezionata non appartiene al dipendente.", 400);
        }

        if (tipoDocumentoId.HasValue)
        {
            var tipoEsiste = await _context.TipiDocumentos.AnyAsync(t => t.Id == tipoDocumentoId.Value && t.Attivo);
            if (!tipoEsiste)
                return ServiceResult<bool>.Fail("Il tipo documento selezionato non esiste o non è attivo.", 400);
        }

        return ServiceResult<bool>.Ok(true);
    }

    private ServiceResult<bool> ValidateFile(IFormFile? file)
    {
        if (file == null || file.Length == 0)
            return ServiceResult<bool>.Fail("File obbligatorio.", 400);

        if (_options.MaxFileSizeBytes > 0 && file.Length > _options.MaxFileSizeBytes)
            return ServiceResult<bool>.Fail($"Il file supera la dimensione massima consentita di {_options.MaxFileSizeBytes} byte.", 400);

        var originalFileName = Path.GetFileName(file.FileName);
        if (string.IsNullOrWhiteSpace(originalFileName))
            return ServiceResult<bool>.Fail("Nome file non valido.", 400);

        var extension = Path.GetExtension(originalFileName).ToLowerInvariant();
        var allowedExtensions = _options.AllowedExtensions.Select(e => e.StartsWith('.') ? e.ToLowerInvariant() : "." + e.ToLowerInvariant()).ToHashSet();
        if (allowedExtensions.Count > 0 && !allowedExtensions.Contains(extension))
            return ServiceResult<bool>.Fail("Estensione file non consentita.", 400);

        var contentType = file.ContentType?.Trim();
        var allowedMimeTypes = _options.AllowedMimeTypes.Select(m => m.ToLowerInvariant()).ToHashSet();
        if (allowedMimeTypes.Count > 0 && (string.IsNullOrWhiteSpace(contentType) || !allowedMimeTypes.Contains(contentType.ToLowerInvariant())))
            return ServiceResult<bool>.Fail("Tipo MIME non consentito.", 400);

        return ServiceResult<bool>.Ok(true);
    }

    private IQueryable<DocumentiDipendenti> BaseDocumentiQuery()
    {
        return _context.DocumentiDipendentis
            .AsNoTracking()
            .Include(d => d.Dipendente)
            .Include(d => d.Cartella)
            .Include(d => d.TipoDocumento);
    }

    private static DocumentoDipendenteListDto ToListDto(DocumentiDipendenti d)
    {
        return new DocumentoDipendenteListDto
        {
            Id = d.Id,
            DipendenteId = d.DipendenteId,
            Dipendente = d.Dipendente.Cognome + " " + d.Dipendente.Nome,
            CartellaId = d.CartellaId,
            Cartella = d.Cartella != null ? d.Cartella.Nome : null,
            TipoDocumentoId = d.TipoDocumentoId,
            TipoDocumento = d.TipoDocumento != null ? d.TipoDocumento.Nome : null,
            NomeFile = d.NomeFile,
            NomeFileSalvato = d.NomeFileSalvato,
            PercorsoFile = d.PercorsoFile,
            Estensione = d.Estensione,
            ContentType = d.ContentType,
            DimensioneBytes = d.DimensioneBytes,
            DataDocumento = d.DataDocumento,
            DataScadenza = d.DataScadenza
        };
    }

    private static DocumentoDipendenteDetailDto ToDetailDto(DocumentiDipendenti d)
    {
        return new DocumentoDipendenteDetailDto
        {
            Id = d.Id,
            DipendenteId = d.DipendenteId,
            Dipendente = d.Dipendente.Cognome + " " + d.Dipendente.Nome,
            CartellaId = d.CartellaId,
            Cartella = d.Cartella != null ? d.Cartella.Nome : null,
            TipoDocumentoId = d.TipoDocumentoId,
            TipoDocumento = d.TipoDocumento != null ? d.TipoDocumento.Nome : null,
            NomeFile = d.NomeFile,
            NomeFileSalvato = d.NomeFileSalvato,
            PercorsoFile = d.PercorsoFile,
            Estensione = d.Estensione,
            ContentType = d.ContentType,
            DimensioneBytes = d.DimensioneBytes,
            DataDocumento = d.DataDocumento,
            DataScadenza = d.DataScadenza,
            Note = d.Note,
            CreatedAt = d.CreatedAt,
            UpdatedAt = d.UpdatedAt,
            UploadedByUtenteId = d.UploadedByUtenteId
        };
    }

    private static CartellaDocumentoDipendenteDto ToCartellaDto(CartelleDocumentiDipendenti c)
    {
        return new CartellaDocumentoDipendenteDto
        {
            Id = c.Id,
            DipendenteId = c.DipendenteId,
            ParentCartellaId = c.ParentCartellaId,
            Nome = c.Nome,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt,
            CreatedByUtenteId = c.CreatedByUtenteId
        };
    }

    private static bool CanManageAll(UserContext user)
    {
        return user.Roles.Any(role =>
                   string.Equals(role, RoleCodes.Admin, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(role, RoleCodes.Segreteria, StringComparison.OrdinalIgnoreCase)) ||
               user.Roles.Contains(PermissionCodes.DocumentiDipendentiManageAll, StringComparer.OrdinalIgnoreCase);
    }

    private static bool CanReadAll(UserContext user)
    {
        return CanManageAll(user) ||
               user.Roles.Contains(PermissionCodes.DocumentiDipendentiReadAll, StringComparer.OrdinalIgnoreCase);
    }

    private static bool CanAccessDipendente(int dipendenteId, UserContext user, bool allowAllRead)
    {
        if (allowAllRead && CanReadAll(user))
            return true;

        return user.DipendenteId == dipendenteId;
    }

    private static string? Pulisci(string? valore)
    {
        return string.IsNullOrWhiteSpace(valore) ? null : valore.Trim();
    }
}

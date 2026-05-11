using System.Globalization;
using System.IO.Compression;
using System.Xml.Linq;
using Gestionale.Api.Common;
using Gestionale.Api.Data;
using Gestionale.Api.DTOs;
using Gestionale.Api.Models;
using Gestionale.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Gestionale.Api.Services;

public class ImportazioniMagazzinoService : IImportazioniMagazzinoService
{
    private static readonly TimeSpan PreviewDuration = TimeSpan.FromMinutes(30);
    private readonly AppDbContext _context;
    private readonly IMemoryCache _cache;

    public ImportazioniMagazzinoService(AppDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<ServiceResult<ImportMovimentiMagazzinoPreviewDto>> CreaAnteprimaAsync(UploadImportMovimentiMagazzinoDto dto)
    {
        if (dto.File == null || dto.File.Length == 0)
        {
            return ServiceResult<ImportMovimentiMagazzinoPreviewDto>.Fail("File Excel obbligatorio.", 400);
        }

        var extension = Path.GetExtension(dto.File.FileName);
        if (!string.Equals(extension, ".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            return ServiceResult<ImportMovimentiMagazzinoPreviewDto>.Fail(
                "Sono supportati solo file .xlsx. Il formato .xls richiede una libreria dedicata non presente nel progetto.",
                400);
        }

        List<ExcelImportRow> excelRows;
        try
        {
            await using var stream = dto.File.OpenReadStream();
            excelRows = LeggiRigheExcel(stream);
        }
        catch (InvalidOperationException ex)
        {
            return ServiceResult<ImportMovimentiMagazzinoPreviewDto>.Fail(ex.Message, 400);
        }
        catch (Exception ex)
        {
            return ServiceResult<ImportMovimentiMagazzinoPreviewDto>.Fail(
                $"Errore durante la lettura del file Excel: {ex.Message}",
                500);
        }

        if (excelRows.Count == 0)
        {
            return ServiceResult<ImportMovimentiMagazzinoPreviewDto>.Fail(
                "Il file Excel non contiene righe dati valide.",
                400);
        }

        var codici = excelRows
            .Select(r => r.Codice)
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var materiali = await _context.Materialis
            .AsNoTracking()
            .Where(m => m.Codice != null && codici.Contains(m.Codice))
            .ToListAsync();

        var materialiByCodice = materiali
            .GroupBy(m => m.Codice!, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

        var previewRows = new List<ImportPreviewCacheRow>();
        var giacenzeSimulate = new Dictionary<int, decimal>();

        foreach (var row in excelRows)
        {
            var previewRow = new ImportPreviewCacheRow
            {
                Riga = row.Riga,
                Codice = row.Codice,
                Descrizione = row.Descrizione,
                Quantita = row.Quantita,
                Esito = "ok",
                Messaggio = "Pronto per l'importazione."
            };

            if (string.IsNullOrWhiteSpace(row.Codice))
            {
                previewRow.Esito = "errore";
                previewRow.Messaggio = "Codice materiale mancante.";
                previewRows.Add(previewRow);
                continue;
            }

            if (row.Quantita <= 0)
            {
                previewRow.Esito = "errore";
                previewRow.Messaggio = "La quantità deve essere maggiore di zero.";
                previewRows.Add(previewRow);
                continue;
            }

            if (!materialiByCodice.TryGetValue(row.Codice, out var materiale))
            {
                previewRow.Esito = "errore";
                previewRow.Messaggio = "Materiale non trovato per il codice indicato.";
                previewRows.Add(previewRow);
                continue;
            }

            if (!materiale.Attivo)
            {
                previewRow.Esito = "errore";
                previewRow.Messaggio = "Il materiale è presente ma non attivo.";
                previewRow.MaterialeId = materiale.Id;
                previewRow.GiacenzaAttuale = materiale.QuantitaAttuale;
                previewRow.GiacenzaDopoScarico = materiale.QuantitaAttuale;
                previewRows.Add(previewRow);
                continue;
            }

            var giacenzaAttuale = giacenzeSimulate.TryGetValue(materiale.Id, out var giacenzaSimulata)
                ? giacenzaSimulata
                : materiale.QuantitaAttuale;

            var giacenzaDopoScarico = giacenzaAttuale - row.Quantita;

            previewRow.MaterialeId = materiale.Id;
            previewRow.GiacenzaAttuale = giacenzaAttuale;
            previewRow.GiacenzaDopoScarico = giacenzaDopoScarico;

            if (giacenzaDopoScarico < 0)
            {
                previewRow.Esito = "errore";
                previewRow.Messaggio = "Lo scarico porterebbe la giacenza sotto zero.";
                previewRows.Add(previewRow);
                continue;
            }

            if (!string.IsNullOrWhiteSpace(row.Descrizione) &&
                !string.Equals(row.Descrizione.Trim(), materiale.Nome, StringComparison.OrdinalIgnoreCase))
            {
                previewRow.Esito = "warning";
                previewRow.Messaggio = $"Descrizione Excel diversa dal materiale anagrafico: '{materiale.Nome}'.";
            }

            giacenzeSimulate[materiale.Id] = giacenzaDopoScarico;
            previewRows.Add(previewRow);
        }

        var previewToken = Guid.NewGuid().ToString("N");
        var cacheItem = new ImportPreviewCacheItem
        {
            FileName = dto.File.FileName,
            Rows = previewRows
        };

        _cache.Set(GetCacheKey(previewToken), cacheItem, PreviewDuration);

        var result = new ImportMovimentiMagazzinoPreviewDto
        {
            PreviewToken = previewToken,
            FileName = dto.File.FileName,
            TotaleRighe = previewRows.Count,
            RigheValide = previewRows.Count(r => r.Esito is "ok" or "warning"),
            RigheConErrore = previewRows.Count(r => r.Esito == "errore"),
            QuantitaTotale = previewRows
                .Where(r => r.Esito is "ok" or "warning")
                .Sum(r => r.Quantita),
            Righe = previewRows.Select(ToDto).ToList()
        };

        return ServiceResult<ImportMovimentiMagazzinoPreviewDto>.Ok(result);
    }

    public async Task<ServiceResult<ImportMovimentiMagazzinoResultDto>> ConfermaImportazioneAsync(ConfermaImportMovimentiMagazzinoDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.PreviewToken))
        {
            return ServiceResult<ImportMovimentiMagazzinoResultDto>.Fail("PreviewToken obbligatorio.", 400);
        }

        if (!_cache.TryGetValue<ImportPreviewCacheItem>(GetCacheKey(dto.PreviewToken), out var preview) || preview == null)
        {
            return ServiceResult<ImportMovimentiMagazzinoResultDto>.Fail(
                "Anteprima non trovata o scaduta. Rigenera la preview prima di confermare.",
                404);
        }

        var validRows = preview.Rows
            .Where(r => r.Esito is "ok" or "warning")
            .ToList();

        if (validRows.Count == 0)
        {
            return ServiceResult<ImportMovimentiMagazzinoResultDto>.Fail(
                "L'anteprima non contiene righe importabili.",
                400);
        }

        var tipoScaricoId = await _context.TipiMovimentoMateriales
            .Where(t => t.Attivo && t.Segno == -1)
            .OrderBy(t => t.Id)
            .Select(t => t.Id)
            .FirstOrDefaultAsync();

        if (tipoScaricoId == 0)
        {
            return ServiceResult<ImportMovimentiMagazzinoResultDto>.Fail("Tipo movimento di scarico non configurato.", 400);
        }

        var materialeIds = validRows
            .Where(r => r.MaterialeId.HasValue)
            .Select(r => r.MaterialeId!.Value)
            .Distinct()
            .ToList();

        var materiali = await _context.Materialis
            .Where(m => materialeIds.Contains(m.Id))
            .ToDictionaryAsync(m => m.Id);

        var saldiSimulati = new Dictionary<int, decimal>();

        foreach (var row in validRows)
        {
            if (!row.MaterialeId.HasValue || !materiali.TryGetValue(row.MaterialeId.Value, out var materiale))
            {
                return ServiceResult<ImportMovimentiMagazzinoResultDto>.Fail(
                    $"Materiale non più disponibile per la riga {row.Riga}. Rigenera l'anteprima.",
                    409);
            }

            if (!materiale.Attivo)
            {
                return ServiceResult<ImportMovimentiMagazzinoResultDto>.Fail(
                    $"Il materiale della riga {row.Riga} non è più attivo. Rigenera l'anteprima.",
                    409);
            }

            var saldoAttuale = saldiSimulati.TryGetValue(materiale.Id, out var saldo)
                ? saldo
                : materiale.QuantitaAttuale;

            var saldoDopoScarico = saldoAttuale - row.Quantita;
            if (saldoDopoScarico < 0)
            {
                return ServiceResult<ImportMovimentiMagazzinoResultDto>.Fail(
                    $"La riga {row.Riga} non è più importabile: la giacenza attuale del materiale non basta più.",
                    409);
            }

            saldiSimulati[materiale.Id] = saldoDopoScarico;
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            foreach (var row in validRows)
            {
                var materiale = materiali[row.MaterialeId!.Value];
                materiale.QuantitaAttuale -= row.Quantita;

                _context.MovimentiMateriales.Add(new MovimentiMateriale
                {
                    MaterialeId = materiale.Id,
                    TipoMovimentoMaterialeId = tipoScaricoId,
                    Quantita = row.Quantita,
                    DataMovimento = DateTime.Now,
                    RiferimentoTabella = "ImportExcelMagazzino",
                    Note = $"Import Excel '{preview.FileName}' - riga {row.Riga}",
                    CreatedAt = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            _cache.Remove(GetCacheKey(dto.PreviewToken));

            var result = new ImportMovimentiMagazzinoResultDto
            {
                FileName = preview.FileName,
                TotaleRighe = preview.Rows.Count,
                RigheImportate = validRows.Count,
                RigheScartate = preview.Rows.Count(r => r.Esito == "errore"),
                MovimentiCreati = validRows.Count,
                QuantitaTotaleScaricata = validRows.Sum(r => r.Quantita)
            };

            return ServiceResult<ImportMovimentiMagazzinoResultDto>.Ok(result);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return ServiceResult<ImportMovimentiMagazzinoResultDto>.Fail(
                $"Errore durante la conferma dell'importazione: {ex.Message}",
                500);
        }
    }

    private static List<ExcelImportRow> LeggiRigheExcel(Stream stream)
    {
        using var archive = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: true);
        var workbookEntry = archive.GetEntry("xl/workbook.xml")
            ?? throw new InvalidOperationException("Workbook Excel non valido.");
        var workbookRelsEntry = archive.GetEntry("xl/_rels/workbook.xml.rels")
            ?? throw new InvalidOperationException("Relazioni workbook Excel non valide.");

        var workbook = XDocument.Load(workbookEntry.Open());
        var workbookRels = XDocument.Load(workbookRelsEntry.Open());

        XNamespace spreadsheetNs = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
        XNamespace relNs = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
        XNamespace packageRelNs = "http://schemas.openxmlformats.org/package/2006/relationships";

        var firstSheet = workbook.Root?
            .Element(spreadsheetNs + "sheets")?
            .Elements(spreadsheetNs + "sheet")
            .FirstOrDefault()
            ?? throw new InvalidOperationException("Il file Excel non contiene fogli.");

        var relationshipId = firstSheet.Attribute(relNs + "id")?.Value
            ?? throw new InvalidOperationException("Relazione foglio Excel non valida.");

        var worksheetTarget = workbookRels.Root?
            .Elements(packageRelNs + "Relationship")
            .FirstOrDefault(r => string.Equals(r.Attribute("Id")?.Value, relationshipId, StringComparison.Ordinal))?
            .Attribute("Target")?.Value
            ?? throw new InvalidOperationException("Foglio Excel non trovato.");

        var normalizedTarget = worksheetTarget.Replace('\\', '/').TrimStart('/');
        var worksheetPath = normalizedTarget.StartsWith("xl/", StringComparison.OrdinalIgnoreCase)
            ? normalizedTarget
            : "xl/" + normalizedTarget;
        var worksheetEntry = archive.GetEntry(worksheetPath)
            ?? throw new InvalidOperationException("Foglio Excel non leggibile.");

        var sharedStrings = LeggiSharedStrings(archive);
        var worksheet = XDocument.Load(worksheetEntry.Open());
        var sheetRows = worksheet.Root?
            .Element(spreadsheetNs + "sheetData")?
            .Elements(spreadsheetNs + "row")
            .ToList()
            ?? [];

        if (sheetRows.Count == 0)
        {
            return [];
        }

        var intestazioni = LeggiIntestazioni(sheetRows[0], sharedStrings);
        if (!TryResolveHeader(intestazioni, ["codice", "codicemateriale", "articolo", "codicearticolo"], out var codiceHeader) ||
            !TryResolveHeader(intestazioni, ["quantita", "qta", "qt", "quantita(prelevata)", "quantitaprelevata"], out var quantitaHeader))
        {
            throw new InvalidOperationException("Il file Excel deve contenere almeno le colonne 'Codice' e 'Quantita'.");
        }

        TryResolveHeader(intestazioni, ["descrizione", "nome", "descrizionemateriale", "materiale"], out var descrizioneHeader);

        var righe = new List<ExcelImportRow>();
        foreach (var row in sheetRows.Skip(1))
        {
            var numeroRiga = int.TryParse(row.Attribute("r")?.Value, out var rigaCorrente)
                ? rigaCorrente
                : righe.Count + 2;

            var values = LeggiValoriRiga(row, sharedStrings);
            var codice = GetCellValue(values, codiceHeader!);
            var descrizione = descrizioneHeader != null ? GetCellValue(values, descrizioneHeader) : null;
            var quantitaText = GetCellValue(values, quantitaHeader!);

            if (string.IsNullOrWhiteSpace(codice) &&
                string.IsNullOrWhiteSpace(descrizione) &&
                string.IsNullOrWhiteSpace(quantitaText))
            {
                continue;
            }

            var quantita = ParseDecimal(quantitaText);

            righe.Add(new ExcelImportRow
            {
                Riga = numeroRiga,
                Codice = codice?.Trim() ?? string.Empty,
                Descrizione = descrizione?.Trim() ?? string.Empty,
                Quantita = quantita
            });
        }

        return righe;
    }

    private static Dictionary<string, string> LeggiIntestazioni(XElement headerRow, IReadOnlyList<string> sharedStrings)
    {
        var values = LeggiValoriRiga(headerRow, sharedStrings);
        return values.ToDictionary(
            kvp => NormalizzaHeader(kvp.Value),
            kvp => kvp.Key,
            StringComparer.OrdinalIgnoreCase);
    }

    private static Dictionary<string, string> LeggiValoriRiga(XElement row, IReadOnlyList<string> sharedStrings)
    {
        XNamespace spreadsheetNs = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var cell in row.Elements(spreadsheetNs + "c"))
        {
            var reference = cell.Attribute("r")?.Value ?? string.Empty;
            var column = new string(reference.TakeWhile(char.IsLetter).ToArray());
            if (string.IsNullOrWhiteSpace(column))
            {
                continue;
            }

            result[column] = LeggiValoreCella(cell, sharedStrings);
        }

        return result;
    }

    private static string LeggiValoreCella(XElement cell, IReadOnlyList<string> sharedStrings)
    {
        XNamespace spreadsheetNs = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
        var cellType = cell.Attribute("t")?.Value;

        if (cellType == "inlineStr")
        {
            return cell.Element(spreadsheetNs + "is")?.Value ?? string.Empty;
        }

        var rawValue = cell.Element(spreadsheetNs + "v")?.Value ?? string.Empty;
        if (cellType == "s" && int.TryParse(rawValue, out var sharedStringIndex) && sharedStringIndex >= 0 && sharedStringIndex < sharedStrings.Count)
        {
            return sharedStrings[sharedStringIndex];
        }

        return rawValue;
    }

    private static IReadOnlyList<string> LeggiSharedStrings(ZipArchive archive)
    {
        var sharedStringsEntry = archive.GetEntry("xl/sharedStrings.xml");
        if (sharedStringsEntry == null)
        {
            return [];
        }

        XNamespace spreadsheetNs = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
        var document = XDocument.Load(sharedStringsEntry.Open());

        return document.Root?
            .Elements(spreadsheetNs + "si")
            .Select(si => string.Concat(si.Descendants(spreadsheetNs + "t").Select(t => t.Value)))
            .ToList()
            ?? [];
    }

    private static bool TryResolveHeader(
        IReadOnlyDictionary<string, string> headers,
        IEnumerable<string> aliases,
        out string? column)
    {
        foreach (var alias in aliases)
        {
            if (headers.TryGetValue(alias, out var resolvedColumn))
            {
                column = resolvedColumn;
                return true;
            }
        }

        column = null;
        return false;
    }

    private static string? GetCellValue(IReadOnlyDictionary<string, string> rowValues, string column)
    {
        return rowValues.TryGetValue(column, out var value)
            ? value
            : null;
    }

    private static decimal ParseDecimal(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return 0;
        }

        var normalized = value.Trim().Replace(" ", string.Empty);
        if (decimal.TryParse(normalized, NumberStyles.Any, CultureInfo.InvariantCulture, out var invariant))
        {
            return invariant;
        }

        if (decimal.TryParse(normalized, NumberStyles.Any, CultureInfo.GetCultureInfo("it-IT"), out var italian))
        {
            return italian;
        }

        return 0;
    }

    private static string NormalizzaHeader(string? header)
    {
        return (header ?? string.Empty)
            .Trim()
            .ToLowerInvariant()
            .Replace("à", "a")
            .Replace("è", "e")
            .Replace("é", "e")
            .Replace("ì", "i")
            .Replace("ò", "o")
            .Replace("ù", "u")
            .Replace(" ", string.Empty);
    }

    private static ImportMovimentiMagazzinoPreviewRowDto ToDto(ImportPreviewCacheRow row)
    {
        return new ImportMovimentiMagazzinoPreviewRowDto
        {
            Riga = row.Riga,
            Codice = row.Codice,
            Descrizione = row.Descrizione,
            Quantita = row.Quantita,
            GiacenzaAttuale = row.GiacenzaAttuale,
            GiacenzaDopoScarico = row.GiacenzaDopoScarico,
            Esito = row.Esito,
            Messaggio = row.Messaggio
        };
    }

    private static string GetCacheKey(string previewToken)
    {
        return $"import-magazzino-preview:{previewToken}";
    }

    private sealed class ExcelImportRow
    {
        public int Riga { get; set; }
        public string Codice { get; set; } = string.Empty;
        public string Descrizione { get; set; } = string.Empty;
        public decimal Quantita { get; set; }
    }

    private sealed class ImportPreviewCacheItem
    {
        public string FileName { get; set; } = string.Empty;
        public List<ImportPreviewCacheRow> Rows { get; set; } = [];
    }

    private sealed class ImportPreviewCacheRow
    {
        public int Riga { get; set; }
        public int? MaterialeId { get; set; }
        public string Codice { get; set; } = string.Empty;
        public string Descrizione { get; set; } = string.Empty;
        public decimal Quantita { get; set; }
        public decimal GiacenzaAttuale { get; set; }
        public decimal GiacenzaDopoScarico { get; set; }
        public string Esito { get; set; } = string.Empty;
        public string Messaggio { get; set; } = string.Empty;
    }
}

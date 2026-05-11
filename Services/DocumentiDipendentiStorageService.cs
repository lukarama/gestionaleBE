using Gestionale.Api.DTOs;
using Gestionale.Api.Models;
using Gestionale.Api.Options;
using Gestionale.Api.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.StaticFiles;
using System.Text;

namespace Gestionale.Api.Services;

public class DocumentiDipendentiStorageService : IDocumentiDipendentiStorageService
{
    private const int MaxDescrizioneLength = 120;

    private readonly DocumentiDipendentiOptions _options;
    private readonly IWebHostEnvironment _environment;

    public DocumentiDipendentiStorageService(
        IOptions<DocumentiDipendentiOptions> options,
        IWebHostEnvironment environment)
    {
        _options = options.Value;
        _environment = environment;
    }

    public void EnsureDipendenteFolder(Dipendenti dipendente)
    {
        if (string.IsNullOrWhiteSpace(_options.BasePath))
        {
            return;
        }

        var basePath = ResolveBasePath(_options.BasePath);
        Directory.CreateDirectory(basePath);

        var folderName = BuildFolderName(dipendente);
        var folderPath = Path.Combine(basePath, folderName);

        Directory.CreateDirectory(folderPath);
    }

    public List<DocumentoDipendenteFileDto> GetDipendenteFiles(Dipendenti dipendente)
    {
        if (string.IsNullOrWhiteSpace(_options.BasePath))
        {
            return [];
        }

        var basePath = ResolveBasePath(_options.BasePath);
        var folderPath = ResolveDipendenteFolderPath(basePath, dipendente);

        if (folderPath == null)
        {
            return [];
        }

        return Directory
            .EnumerateFiles(folderPath)
            .Select(path =>
            {
                var file = new FileInfo(path);

                return new DocumentoDipendenteFileDto
                {
                    NomeFile = file.Name,
                    Estensione = file.Extension,
                    DimensioneBytes = file.Length,
                    UltimaModifica = file.LastWriteTime
                };
            })
            .OrderByDescending(file => file.UltimaModifica)
            .ThenBy(file => file.NomeFile)
            .ToList();
    }

    public DocumentoDipendenteDownloadDto? GetDipendenteFile(Dipendenti dipendente, string nomeFile)
    {
        if (string.IsNullOrWhiteSpace(_options.BasePath) || string.IsNullOrWhiteSpace(nomeFile))
        {
            return null;
        }

        var safeFileName = Path.GetFileName(nomeFile.Trim());
        if (string.IsNullOrWhiteSpace(safeFileName))
        {
            return null;
        }

        var basePath = ResolveBasePath(_options.BasePath);
        var folderPath = ResolveDipendenteFolderPath(basePath, dipendente);
        if (folderPath == null)
        {
            return null;
        }

        var filePath = Path.Combine(folderPath, safeFileName);
        var fullFolderPath = Path.GetFullPath(folderPath);
        var fullFilePath = Path.GetFullPath(filePath);

        if (!fullFilePath.StartsWith(fullFolderPath, StringComparison.OrdinalIgnoreCase) || !File.Exists(fullFilePath))
        {
            return null;
        }

        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(fullFilePath, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        return new DocumentoDipendenteDownloadDto
        {
            FileBytes = File.ReadAllBytes(fullFilePath),
            NomeFile = safeFileName,
            ContentType = contentType
        };
    }

    public async Task<(string SavedFileName, string FullPath)> SaveFileAsync(Dipendenti dipendente, CartelleDocumentiDipendenti? cartella, IFormFile file)
    {
        if (string.IsNullOrWhiteSpace(_options.BasePath))
        {
            throw new InvalidOperationException("Percorso base documenti non configurato.");
        }

        var basePath = ResolveBasePath(_options.BasePath);
        Directory.CreateDirectory(basePath);

        var dipendenteFolderPath = Path.Combine(basePath, BuildFolderName(dipendente));
        Directory.CreateDirectory(dipendenteFolderPath);

        var targetFolderPath = dipendenteFolderPath;
        if (cartella != null)
        {
            targetFolderPath = Path.Combine(dipendenteFolderPath, BuildDocumentFolderName(cartella));
            Directory.CreateDirectory(targetFolderPath);
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var savedFileName = $"{Guid.NewGuid():N}{extension}";
        var fullPath = Path.GetFullPath(Path.Combine(targetFolderPath, savedFileName));
        var fullTargetFolderPath = Path.GetFullPath(targetFolderPath);

        if (!fullPath.StartsWith(fullTargetFolderPath, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Percorso file non valido.");
        }

        await using var stream = new FileStream(fullPath, FileMode.CreateNew);
        await file.CopyToAsync(stream);

        return (savedFileName, fullPath);
    }

    public void DeleteFile(string? fullPath)
    {
        if (string.IsNullOrWhiteSpace(fullPath))
        {
            return;
        }

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }

    private static string? ResolveDipendenteFolderPath(string basePath, Dipendenti dipendente)
    {
        if (!Directory.Exists(basePath))
        {
            return null;
        }

        var expectedPath = Path.Combine(basePath, BuildFolderName(dipendente));
        if (Directory.Exists(expectedPath))
        {
            return expectedPath;
        }

        return Directory
            .EnumerateDirectories(basePath, $"DIP-{dipendente.Id}*")
            .OrderBy(path => path.Length)
            .FirstOrDefault();
    }

    private string ResolveBasePath(string configuredPath)
    {
        var trimmedPath = configuredPath.Trim();

        return Path.IsPathRooted(trimmedPath)
            ? trimmedPath
            : Path.Combine(_environment.ContentRootPath, trimmedPath);
    }

    private static string BuildFolderName(Dipendenti dipendente)
    {
        var descrizione = SanitizeFolderSegment($"{dipendente.Cognome} {dipendente.Nome}");

        if (descrizione.Length > MaxDescrizioneLength)
        {
            descrizione = descrizione[..MaxDescrizioneLength].Trim();
        }

        return string.IsNullOrWhiteSpace(descrizione)
            ? $"DIP-{dipendente.Id}"
            : $"DIP-{dipendente.Id} - {descrizione}";
    }

    private static string BuildDocumentFolderName(CartelleDocumentiDipendenti cartella)
    {
        var descrizione = SanitizeFolderSegment(cartella.Nome);

        if (descrizione.Length > MaxDescrizioneLength)
        {
            descrizione = descrizione[..MaxDescrizioneLength].Trim();
        }

        return string.IsNullOrWhiteSpace(descrizione)
            ? $"CART-{cartella.Id}"
            : $"CART-{cartella.Id} - {descrizione}";
    }

    private static string SanitizeFolderSegment(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var invalidChars = Path.GetInvalidFileNameChars();
        var builder = new StringBuilder(value.Length);
        var lastWasWhiteSpace = false;

        foreach (var character in value.Trim())
        {
            if (invalidChars.Contains(character))
            {
                builder.Append('_');
                lastWasWhiteSpace = false;
                continue;
            }

            if (char.IsWhiteSpace(character))
            {
                if (!lastWasWhiteSpace)
                {
                    builder.Append(' ');
                    lastWasWhiteSpace = true;
                }

                continue;
            }

            builder.Append(character);
            lastWasWhiteSpace = false;
        }

        return builder.ToString().Trim(' ', '.');
    }
}

using Gestionale.Api.Models;
using Gestionale.Api.DTOs;
using Microsoft.AspNetCore.Http;

namespace Gestionale.Api.Services.Interfaces;

public interface IDocumentiDipendentiStorageService
{
    void EnsureDipendenteFolder(Dipendenti dipendente);
    List<DocumentoDipendenteFileDto> GetDipendenteFiles(Dipendenti dipendente);
    DocumentoDipendenteDownloadDto? GetDipendenteFile(Dipendenti dipendente, string nomeFile);
    Task<(string SavedFileName, string FullPath)> SaveFileAsync(Dipendenti dipendente, CartelleDocumentiDipendenti? cartella, IFormFile file);
    void DeleteFile(string? fullPath);
}

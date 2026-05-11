using Microsoft.AspNetCore.Http;

namespace Gestionale.Api.DTOs;

public class UploadImportMovimentiMagazzinoDto
{
    public IFormFile File { get; set; } = null!;
}

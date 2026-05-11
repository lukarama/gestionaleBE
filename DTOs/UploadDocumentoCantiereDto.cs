using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Gestionale.Api.DTOs;

public class UploadDocumentoCantiereDto
{
    [Required]
    public int CantiereId { get; set; }

    [Required]
    public IFormFile File { get; set; } = null!;

    public DateOnly? DataDocumento { get; set; }

    [MaxLength(1000)]
    public string? Note { get; set; }
}

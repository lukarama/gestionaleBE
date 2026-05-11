using System.ComponentModel.DataAnnotations;

namespace Gestionale.Api.DTOs;

public class CreateDocumentoCantiereDto
{
    [Required]
    public int CantiereId { get; set; }

    [Required]
    [MaxLength(255)]
    public string NomeFile { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string PercorsoFile { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Estensione { get; set; }

    [MaxLength(100)]
    public string? ContentType { get; set; }

    public DateOnly? DataDocumento { get; set; }

    [MaxLength(1000)]
    public string? Note { get; set; }
}

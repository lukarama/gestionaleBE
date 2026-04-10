namespace Gestionale.Api.DTOs;

public class DocumentoMezzoDownloadDto
{
    public byte[] FileBytes { get; set; } = null!;
    public string NomeFile { get; set; } = null!;
    public string ContentType { get; set; } = null!;
}
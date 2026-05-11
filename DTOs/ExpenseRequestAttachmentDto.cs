namespace Gestionale.Api.DTOs;

public class ExpenseRequestAttachmentDto
{
    public byte[] FileBytes { get; set; } = [];
    public string NomeFile { get; set; } = string.Empty;
    public string ContentType { get; set; } = "application/octet-stream";
}

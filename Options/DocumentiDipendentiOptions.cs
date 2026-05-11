namespace Gestionale.Api.Options;

public class DocumentiDipendentiOptions
{
    public const string SectionName = "DocumentiDipendenti";

    public string BasePath { get; set; } = string.Empty;
    public long MaxFileSizeBytes { get; set; } = 10 * 1024 * 1024;
    public string[] AllowedExtensions { get; set; } = [".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx", ".xls", ".xlsx"];
    public string[] AllowedMimeTypes { get; set; } =
    [
        "application/pdf",
        "image/jpeg",
        "image/png",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
    ];
}

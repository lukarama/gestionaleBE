namespace Gestionale.Api.DTOs;

public class DocumentoDipendenteFileDto
{
    public string NomeFile { get; set; } = string.Empty;
    public string Estensione { get; set; } = string.Empty;
    public long DimensioneBytes { get; set; }
    public DateTime UltimaModifica { get; set; }
}

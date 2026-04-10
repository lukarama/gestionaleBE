namespace Gestionale.Api.DTOs;

public class TipoDocumentoSelectDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Label { get; set; } = null!;
}
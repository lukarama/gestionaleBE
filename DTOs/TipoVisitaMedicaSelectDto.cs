namespace Gestionale.Api.DTOs;

public class TipoVisitaMedicaSelectDto
{
    public int Id { get; set; }
    public string Label { get; set; } = null!;
    public string Nome { get; set; } = null!;
}
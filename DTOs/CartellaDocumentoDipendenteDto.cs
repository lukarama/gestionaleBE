namespace Gestionale.Api.DTOs;

public class CartellaDocumentoDipendenteDto
{
    public int Id { get; set; }
    public int DipendenteId { get; set; }
    public int? ParentCartellaId { get; set; }
    public string Nome { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? CreatedByUtenteId { get; set; }
}

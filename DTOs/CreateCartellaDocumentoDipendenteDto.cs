namespace Gestionale.Api.DTOs;

public class CreateCartellaDocumentoDipendenteDto
{
    public string Nome { get; set; } = null!;
    public int? ParentCartellaId { get; set; }
}

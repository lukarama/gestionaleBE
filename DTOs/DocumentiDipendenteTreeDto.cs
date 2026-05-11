namespace Gestionale.Api.DTOs;

public class DocumentiDipendenteTreeDto
{
    public int DipendenteId { get; set; }
    public List<CartellaDocumentoDipendenteDto> Cartelle { get; set; } = [];
    public List<DocumentoDipendenteListDto> Documenti { get; set; } = [];
}

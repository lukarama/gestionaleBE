namespace Gestionale.Api.DTOs;

public class CantiereSchedaDto
{
    public CantiereDetailDto Cantiere { get; set; } = null!;
    public List<DocumentoCantiereListDto> Documenti { get; set; } = [];
    public List<MovimentoMaterialeListDto> MovimentiMateriale { get; set; } = [];
}

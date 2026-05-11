namespace Gestionale.Api.DTOs;

public class DipendenteSchedaDto
{
    public DipendenteDetailDto Dipendente { get; set; } = null!;
    public List<DocumentoDipendenteListDto> Documenti { get; set; } = [];
    public List<VisitaMedicaListDto> VisiteMediche { get; set; } = [];
    public List<AssegnazioneDpiListDto> DpiAssegnati { get; set; } = [];
}

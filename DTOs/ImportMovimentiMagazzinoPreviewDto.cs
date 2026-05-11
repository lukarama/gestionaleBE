namespace Gestionale.Api.DTOs;

public class ImportMovimentiMagazzinoPreviewDto
{
    public string PreviewToken { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public int TotaleRighe { get; set; }
    public int RigheValide { get; set; }
    public int RigheConErrore { get; set; }
    public decimal QuantitaTotale { get; set; }
    public List<ImportMovimentiMagazzinoPreviewRowDto> Righe { get; set; } = [];
}

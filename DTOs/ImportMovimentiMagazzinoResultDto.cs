namespace Gestionale.Api.DTOs;

public class ImportMovimentiMagazzinoResultDto
{
    public string FileName { get; set; } = string.Empty;
    public int TotaleRighe { get; set; }
    public int RigheImportate { get; set; }
    public int RigheScartate { get; set; }
    public int MovimentiCreati { get; set; }
    public decimal QuantitaTotaleScaricata { get; set; }
}

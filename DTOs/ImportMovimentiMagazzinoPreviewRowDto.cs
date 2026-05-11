namespace Gestionale.Api.DTOs;

public class ImportMovimentiMagazzinoPreviewRowDto
{
    public int Riga { get; set; }
    public string Codice { get; set; } = string.Empty;
    public string Descrizione { get; set; } = string.Empty;
    public decimal Quantita { get; set; }
    public decimal GiacenzaAttuale { get; set; }
    public decimal GiacenzaDopoScarico { get; set; }
    public string Esito { get; set; } = string.Empty;
    public string Messaggio { get; set; } = string.Empty;
}

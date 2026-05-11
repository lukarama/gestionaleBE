namespace Gestionale.Api.DTOs;

public class ExpenseRequestDto
{
    public int Id { get; set; }
    public int DipendenteId { get; set; }
    public string Dipendente { get; set; } = string.Empty;
    public DateOnly DataSpesa { get; set; }
    public string CategoriaSpesa { get; set; } = string.Empty;
    public string Descrizione { get; set; } = string.Empty;
    public decimal Importo { get; set; }
    public string Valuta { get; set; } = string.Empty;
    public string MetodoPagamento { get; set; } = string.Empty;
    public string Stato { get; set; } = string.Empty;
    public string? AllegatoNomeFile { get; set; }
    public string? AllegatoContentType { get; set; }
    public string? NotaGestione { get; set; }
    public int? GestitoDaUtenteId { get; set; }
    public string? GestitoDaUtente { get; set; }
    public DateTime? GestitoAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

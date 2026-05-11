using Microsoft.AspNetCore.Http;

namespace Gestionale.Api.DTOs;

public class CreateExpenseRequestDto
{
    public int? DipendenteId { get; set; }
    public DateOnly DataSpesa { get; set; }
    public string CategoriaSpesa { get; set; } = string.Empty;
    public string Descrizione { get; set; } = string.Empty;
    public decimal Importo { get; set; }
    public string Valuta { get; set; } = string.Empty;
    public string MetodoPagamento { get; set; } = string.Empty;
    public IFormFile? Allegato { get; set; }
}

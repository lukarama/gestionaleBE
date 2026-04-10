namespace Gestionale.Api.DTOs;

public class DashboardMagazzinoDto
{
    public int TotaleMateriali { get; set; }
    public int MaterialiAttivi { get; set; }
    public int MaterialiSottoScorta { get; set; }
    public int MaterialiEsauriti { get; set; }
    public decimal QuantitaTotale { get; set; }
}
namespace Gestionale.Api.DTOs
{
    public class DpiDetailDto
    {
        public int Id { get; set; }
        public string? Codice { get; set; }
        public string Nome { get; set; } = string.Empty;

        public int? CategoriaDpiId { get; set; }
        public string? CategoriaDpi { get; set; }

        public string? Descrizione { get; set; }
        public string? Taglia { get; set; }
        public string? Marca { get; set; }
        public string? Modello { get; set; }
        public string? Barcode { get; set; }

        public int? FornitoreId { get; set; }
        public string? Fornitore { get; set; }

        public int? DurataGiorni { get; set; }
        public bool HaScadenza { get; set; }
        public bool Attivo { get; set; }
        public string? Note { get; set; }
    }
}
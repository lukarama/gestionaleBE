namespace Gestionale.Api.DTOs
{
    public class MaterialeDetailDto
    {
        public int Id { get; set; }
        public string? Codice { get; set; }
        public string Nome { get; set; } = string.Empty;

        public int? CategoriaMaterialeId { get; set; }
        public string? CategoriaMateriale { get; set; }

        public string? Descrizione { get; set; }
        public string? UnitaMisura { get; set; }
        public decimal QuantitaAttuale { get; set; }
        public decimal? ScortaMinima { get; set; }
        public bool SottoScorta { get; set; }

        public string? Barcode { get; set; }

        public int? FornitoreId { get; set; }
        public string? Fornitore { get; set; }

        public bool Attivo { get; set; }
        public string? Note { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace Gestionale.Api.DTOs
{
    public class CreateMaterialeDto
    {
        public string? Codice { get; set; }

        [Required]
        public string Nome { get; set; } = string.Empty;

        public int? CategoriaMaterialeId { get; set; }
        public string? Descrizione { get; set; }
        public string? UnitaMisura { get; set; }

        public decimal QuantitaAttuale { get; set; }
        public decimal? ScortaMinima { get; set; }

        public string? Barcode { get; set; }
        public int? FornitoreId { get; set; }

        public bool Attivo { get; set; } = true;
        public string? Note { get; set; }
    }
}
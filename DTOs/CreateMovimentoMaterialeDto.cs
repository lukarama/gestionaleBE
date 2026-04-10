using System.ComponentModel.DataAnnotations;

namespace Gestionale.Api.DTOs
{
    public class CreateMovimentoMaterialeDto
    {
        [Required]
        public int MaterialeId { get; set; }

        [Required]
        public int TipoMovimentoMaterialeId { get; set; }

        [Range(typeof(decimal), "0.0001", "999999999")]
        public decimal Quantita { get; set; }

        [Required]
        public DateTime DataMovimento { get; set; }

        public int? DipendenteId { get; set; }
        public int? CantiereId { get; set; }

        public string? RiferimentoTabella { get; set; }
        public int? RiferimentoId { get; set; }

        public string? Note { get; set; }
    }
}
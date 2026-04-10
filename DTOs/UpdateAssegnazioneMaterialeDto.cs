using System.ComponentModel.DataAnnotations;

namespace Gestionale.Api.DTOs
{
    public class UpdateAssegnazioneMaterialeDto
    {
        [Required]
        public int MaterialeId { get; set; }

        public int? DipendenteId { get; set; }

        public int? CantiereId { get; set; }

        [Range(typeof(decimal), "0.0001", "999999999")]
        public decimal Quantita { get; set; }

        [Required]
        public int StatoAssegnazioneId { get; set; }

        [Required]
        public DateTime DataAssegnazione { get; set; }

        public DateTime? DataRestituzione { get; set; }

        public string? Note { get; set; }
    }
}
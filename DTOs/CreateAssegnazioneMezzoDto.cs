using System.ComponentModel.DataAnnotations;

namespace Gestionale.Api.DTOs
{
    public class CreateAssegnazioneMezzoDto
    {
        [Required]
        public int MezzoId { get; set; }

        public int? DipendenteId { get; set; }

        public int? CantiereId { get; set; }

        [Required]
        public int StatoAssegnazioneId { get; set; }

        [Required]
        public DateTime DataInizio { get; set; }

        public DateTime? DataFine { get; set; }

        public int? KmConsegna { get; set; }

        public int? KmRientro { get; set; }

        public string? Note { get; set; }
    }
}
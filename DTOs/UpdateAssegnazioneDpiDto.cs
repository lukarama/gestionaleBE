using System.ComponentModel.DataAnnotations;

namespace Gestionale.Api.DTOs
{
    public class UpdateAssegnazioneDpiDto
    {
        [Required]
        public int DipendenteId { get; set; }

        [Required]
        public int DpiId { get; set; }

        public int? CantiereId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantita { get; set; }

        [Required]
        public int StatoAssegnazioneId { get; set; }

        [Required]
        public DateOnly DataConsegna { get; set; }

        public DateOnly? DataScadenza { get; set; }
        public DateOnly? DataRestituzione { get; set; }

        public bool FirmaConsegna { get; set; }
        public string? Note { get; set; }
    }
}
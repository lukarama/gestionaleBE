namespace Gestionale.Api.DTOs
{
    public class AssegnazioneMaterialeDetailDto
    {
        public int Id { get; set; }

        public int MaterialeId { get; set; }
        public string Materiale { get; set; } = string.Empty;

        public int? DipendenteId { get; set; }
        public string? Dipendente { get; set; }

        public int? CantiereId { get; set; }
        public string? Cantiere { get; set; }

        public decimal Quantita { get; set; }

        public int StatoAssegnazioneId { get; set; }
        public string StatoAssegnazione { get; set; } = string.Empty;

        public DateTime DataAssegnazione { get; set; }
        public DateTime? DataRestituzione { get; set; }

        public string? Note { get; set; }
    }
}
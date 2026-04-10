namespace Gestionale.Api.DTOs
{
    public class AssegnazioneDpiDetailDto
    {
        public int Id { get; set; }

        public int DipendenteId { get; set; }
        public string Dipendente { get; set; } = string.Empty;

        public int DpiId { get; set; }
        public string Dpi { get; set; } = string.Empty;

        public int? CantiereId { get; set; }
        public string? Cantiere { get; set; }

        public int Quantita { get; set; }

        public int StatoAssegnazioneId { get; set; }
        public string StatoAssegnazione { get; set; } = string.Empty;

        public DateOnly DataConsegna { get; set; }
        public DateOnly? DataScadenza { get; set; }
        public DateOnly? DataRestituzione { get; set; }

        public bool FirmaConsegna { get; set; }
        public string? Note { get; set; }
    }
}
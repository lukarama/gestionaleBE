namespace Gestionale.Api.DTOs
{
    public class AssegnazioneMezzoListDto
    {
        public int Id { get; set; }

        public int MezzoId { get; set; }
        public string Mezzo { get; set; } = string.Empty;

        public int? DipendenteId { get; set; }
        public string? Dipendente { get; set; }

        public int? CantiereId { get; set; }
        public string? Cantiere { get; set; }

        public int StatoAssegnazioneId { get; set; }
        public string StatoAssegnazione { get; set; } = string.Empty;

        public DateTime DataInizio { get; set; }
        public DateTime? DataFine { get; set; }

        public int? KmConsegna { get; set; }
        public int? KmRientro { get; set; }
    }
}
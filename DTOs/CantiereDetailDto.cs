namespace Gestionale.Api.DTOs
{
    public class CantiereDetailDto
    {
        public int Id { get; set; }
        public string? Codice { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Cliente { get; set; }
        public string? Indirizzo { get; set; }
        public string? Citta { get; set; }
        public string? Provincia { get; set; }
        public string? Cap { get; set; }
        public DateOnly? DataInizio { get; set; }
        public DateOnly? DataFine { get; set; }
        public bool Attivo { get; set; }
        public string? Note { get; set; }
    }
}
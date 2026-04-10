namespace Gestionale.Api.DTOs
{
    public class CantiereListDto
    {
        public int Id { get; set; }
        public string? Codice { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Cliente { get; set; }
        public string? Citta { get; set; }
        public string? Provincia { get; set; }
        public DateOnly? DataInizio { get; set; }
        public DateOnly? DataFine { get; set; }
        public bool Attivo { get; set; }
    }
}
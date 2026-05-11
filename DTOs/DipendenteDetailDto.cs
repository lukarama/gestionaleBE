namespace Gestionale.Api.DTOs
{
    public class DipendenteDetailDto
    {
        public int Id { get; set; }
        public string? Matricola { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Cognome { get; set; } = string.Empty;
        public string? CodiceFiscale { get; set; }
        public DateOnly? DataNascita { get; set; }
        public string? LuogoNascita { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? Indirizzo { get; set; }
        public string? Citta { get; set; }
        public string? Provincia { get; set; }
        public string? Cap { get; set; }
        public DateOnly? DataAssunzione { get; set; }
        public DateOnly? DataCessazione { get; set; }
        public bool HaPatente { get; set; }
        public string? CategoriaPatente { get; set; }
        public int? MansioneId { get; set; }
        public string? Mansione { get; set; }
        public bool Attivo { get; set; }
        public string? Note { get; set; }
    }
}

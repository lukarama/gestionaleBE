namespace Gestionale.Api.DTOs
{
    public class DipendenteListDto
    {
        public int Id { get; set; }
        public string? Matricola { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Cognome { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public bool Attivo { get; set; }
        public string? Mansione { get; set; }
    }
}
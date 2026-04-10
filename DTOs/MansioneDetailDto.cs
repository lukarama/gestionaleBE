namespace Gestionale.Api.DTOs
{
    public class MansioneDetailDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descrizione { get; set; }
        public bool Attivo { get; set; }
    }
}
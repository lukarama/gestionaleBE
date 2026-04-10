namespace Gestionale.Api.DTOs
{
    public class FornitoreListDto
    {
        public int Id { get; set; }
        public string RagioneSociale { get; set; } = string.Empty;
        public string? PartitaIva { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? Citta { get; set; }
        public bool Attivo { get; set; }
    }
}
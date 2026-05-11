namespace Gestionale.Api.DTOs
{
    public class CantiereDetailDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Indirizzo { get; set; }
        public string? ResponsabileCantiere { get; set; }
        public string? DirezioneLavori { get; set; }
        public string? Committente { get; set; }
        public string? Appaltatore { get; set; }
        public DateOnly? DataInizioLavori { get; set; }
        public DateOnly? DataPrevistaFineLavori { get; set; }
        public bool Attivo { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

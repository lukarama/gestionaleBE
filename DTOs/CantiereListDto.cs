namespace Gestionale.Api.DTOs
{
    public class CantiereListDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Indirizzo { get; set; }
        public string? ResponsabileCantiere { get; set; }
        public string? Committente { get; set; }
        public DateOnly? DataInizioLavori { get; set; }
        public DateOnly? DataPrevistaFineLavori { get; set; }
        public bool Attivo { get; set; }
    }
}

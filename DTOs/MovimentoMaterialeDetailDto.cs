namespace Gestionale.Api.DTOs
{
    public class MovimentoMaterialeDetailDto
    {
        public int Id { get; set; }

        public int MaterialeId { get; set; }
        public string Materiale { get; set; } = string.Empty;

        public int TipoMovimentoMaterialeId { get; set; }
        public string TipoMovimentoMateriale { get; set; } = string.Empty;

        public decimal Quantita { get; set; }
        public DateTime DataMovimento { get; set; }

        public int? DipendenteId { get; set; }
        public string? Dipendente { get; set; }

        public int? CantiereId { get; set; }
        public string? Cantiere { get; set; }

        public string? RiferimentoTabella { get; set; }
        public int? RiferimentoId { get; set; }

        public string? Note { get; set; }
    }
}
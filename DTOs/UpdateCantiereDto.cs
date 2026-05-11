using System.ComponentModel.DataAnnotations;

namespace Gestionale.Api.DTOs
{
    public class UpdateCantiereDto
    {
        [Required]
        [MaxLength(200)]
        public string Nome { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Indirizzo { get; set; }

        [MaxLength(200)]
        public string? ResponsabileCantiere { get; set; }

        [MaxLength(200)]
        public string? DirezioneLavori { get; set; }

        [MaxLength(200)]
        public string? Committente { get; set; }

        [MaxLength(200)]
        public string? Appaltatore { get; set; }

        public DateOnly? DataInizioLavori { get; set; }
        public DateOnly? DataPrevistaFineLavori { get; set; }
        public bool Attivo { get; set; }

        [MaxLength(1000)]
        public string? Note { get; set; }
    }
}

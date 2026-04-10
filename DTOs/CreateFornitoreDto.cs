using System.ComponentModel.DataAnnotations;

namespace Gestionale.Api.DTOs
{
    public class CreateFornitoreDto
    {
        [Required]
        public string RagioneSociale { get; set; } = string.Empty;

        public string? PartitaIva { get; set; }
        public string? CodiceFiscale { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? Indirizzo { get; set; }
        public string? Citta { get; set; }
        public string? Provincia { get; set; }
        public string? Cap { get; set; }
        public string? Note { get; set; }
        public bool Attivo { get; set; } = true;
    }
}
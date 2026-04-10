using System.ComponentModel.DataAnnotations;

namespace Gestionale.Api.DTOs
{
    public class UpdateTipologiaMezzoDto
    {
        [Required]
        public string Nome { get; set; } = string.Empty;

        public string? Descrizione { get; set; }

        public bool Attivo { get; set; }
    }
}
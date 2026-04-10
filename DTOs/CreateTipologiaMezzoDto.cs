using System.ComponentModel.DataAnnotations;

namespace Gestionale.Api.DTOs
{
    public class CreateTipologiaMezzoDto
    {
        [Required]
        public string Nome { get; set; } = string.Empty;

        public string? Descrizione { get; set; }

        public bool Attivo { get; set; } = true;
    }
}
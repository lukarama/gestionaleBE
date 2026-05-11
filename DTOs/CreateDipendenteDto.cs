using System.ComponentModel.DataAnnotations;

namespace Gestionale.Api.DTOs
{
    public class CreateDipendenteDto
    {
        public string? Matricola { get; set; }

        [Required]
        public string Nome { get; set; } = string.Empty;

        [Required]
        public string Cognome { get; set; } = string.Empty;

        public string? CodiceFiscale { get; set; }
        public DateOnly? DataNascita { get; set; }
        public string? LuogoNascita { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? Indirizzo { get; set; }
        public string? Citta { get; set; }
        public string? Provincia { get; set; }
        public string? Cap { get; set; }
        public DateOnly? DataAssunzione { get; set; }
        public DateOnly? DataCessazione { get; set; }
        public bool HaPatente { get; set; }
        public string? CategoriaPatente { get; set; }
        public int? MansioneId { get; set; }
        public bool Attivo { get; set; } = true;
        public string? Note { get; set; }
    }
}

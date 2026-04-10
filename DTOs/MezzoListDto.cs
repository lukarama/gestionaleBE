namespace Gestionale.Api.DTOs
{
    public class MezzoListDto
    {
        public int Id { get; set; }
        public string? Targa { get; set; }
        public string? CodiceInterno { get; set; }
        public string? Marca { get; set; }
        public string? Modello { get; set; }
        public int? AnnoImmatricolazione { get; set; }
        public DateOnly? DataRevisione { get; set; }
        public DateOnly? DataScadenzaBollo { get; set; }
        public DateOnly? DataScadenzaAssicurazione { get; set; }
        public DateOnly? DataTagliando { get; set; }

        public int? TipologiaMezzoId { get; set; }
        public string? TipologiaMezzo { get; set; }

        public int? FornitoreId { get; set; }
        public string? Fornitore { get; set; }

        public bool Attivo { get; set; }
    }
}
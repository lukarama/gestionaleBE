namespace Gestionale.Api.DTOs
{
    public class UpdateMezzoDto
    {
        public string? Targa { get; set; }
        public string? NumeroTelaio { get; set; }
        public string? CodiceInterno { get; set; }
        public int? TipologiaMezzoId { get; set; }
        public string? Marca { get; set; }
        public string? Modello { get; set; }
        public int? AnnoImmatricolazione { get; set; }
        public DateOnly? DataImmatricolazione { get; set; }
        public DateOnly? DataRevisione { get; set; }
        public DateOnly? DataScadenzaBollo { get; set; }
        public DateOnly? DataScadenzaAssicurazione { get; set; }
        public DateOnly? DataTagliando { get; set; }
        public int? FornitoreId { get; set; }
        public bool Attivo { get; set; }
        public string? Note { get; set; }
    }
}
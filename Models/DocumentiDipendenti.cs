using System;
using System.Collections.Generic;

namespace Gestionale.Api.Models;

public partial class DocumentiDipendenti
{
    public int Id { get; set; }

    public int DipendenteId { get; set; }

    public int? CartellaId { get; set; }

    public int? TipoDocumentoId { get; set; }

    public string NomeFile { get; set; } = null!;

    public string? NomeFileSalvato { get; set; }

    public string PercorsoFile { get; set; } = null!;

    public string? Estensione { get; set; }

    public string? ContentType { get; set; }

    public long DimensioneBytes { get; set; }

    public DateOnly? DataDocumento { get; set; }

    public DateOnly? DataScadenza { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UploadedByUtenteId { get; set; }

    public virtual CartelleDocumentiDipendenti? Cartella { get; set; }

    public virtual Dipendenti Dipendente { get; set; } = null!;

    public virtual TipiDocumento? TipoDocumento { get; set; }

    public virtual Utenti? UploadedByUtente { get; set; }
}

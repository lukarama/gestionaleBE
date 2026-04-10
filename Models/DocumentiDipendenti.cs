using System;
using System.Collections.Generic;

namespace Gestionale.Api.Models;

public partial class DocumentiDipendenti
{
    public int Id { get; set; }

    public int DipendenteId { get; set; }

    public int? TipoDocumentoId { get; set; }

    public string NomeFile { get; set; } = null!;

    public string PercorsoFile { get; set; } = null!;

    public string? Estensione { get; set; }

    public string? ContentType { get; set; }

    public DateOnly? DataDocumento { get; set; }

    public DateOnly? DataScadenza { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Dipendenti Dipendente { get; set; } = null!;

    public virtual TipiDocumento? TipoDocumento { get; set; }
}

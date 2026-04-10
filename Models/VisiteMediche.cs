using System;
using System.Collections.Generic;

namespace Gestionale.Api.Models;

public partial class VisiteMediche
{
    public int Id { get; set; }

    public int DipendenteId { get; set; }

    public int TipoVisitaMedicaId { get; set; }

    public DateOnly DataVisita { get; set; }

    public DateOnly? DataScadenza { get; set; }

    public int? EsitoVisitaMedicaId { get; set; }

    public bool? Idoneo { get; set; }

    public string? Prescrizioni { get; set; }

    public string? MedicoCompetente { get; set; }

    public string? StrutturaSanitaria { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Dipendenti Dipendente { get; set; } = null!;

    public virtual EsitiVisitaMedica? EsitoVisitaMedica { get; set; }

    public virtual TipiVisitaMedica TipoVisitaMedica { get; set; } = null!;
}

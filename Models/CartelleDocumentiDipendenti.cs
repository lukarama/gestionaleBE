using System;
using System.Collections.Generic;

namespace Gestionale.Api.Models;

public partial class CartelleDocumentiDipendenti
{
    public int Id { get; set; }

    public int DipendenteId { get; set; }

    public int? ParentCartellaId { get; set; }

    public string Nome { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedByUtenteId { get; set; }

    public virtual Utenti? CreatedByUtente { get; set; }

    public virtual Dipendenti Dipendente { get; set; } = null!;

    public virtual CartelleDocumentiDipendenti? ParentCartella { get; set; }

    public virtual ICollection<CartelleDocumentiDipendenti> InverseParentCartella { get; set; } = new List<CartelleDocumentiDipendenti>();

    public virtual ICollection<DocumentiDipendenti> DocumentiDipendentis { get; set; } = new List<DocumentiDipendenti>();
}

using System;
using System.Collections.Generic;

namespace Gestionale.Api.Models;

public partial class Utenti
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string? Email { get; set; }

    public string PasswordHash { get; set; } = null!;

    public string? Nome { get; set; }

    public string? Cognome { get; set; }

    public int? DipendenteId { get; set; }

    public bool Attivo { get; set; }

    public bool MustChangePassword { get; set; }

    public DateTime? UltimoAccessoAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Dipendenti? Dipendente { get; set; }

    public virtual ICollection<CartelleDocumentiDipendenti> CartelleDocumentiDipendentiCreatedByUtentes { get; set; } = new List<CartelleDocumentiDipendenti>();

    public virtual ICollection<DocumentiDipendenti> DocumentiDipendentiUploadedByUtentes { get; set; } = new List<DocumentiDipendenti>();

    public virtual ICollection<ExpenseRequest> ExpenseRequestsGestite { get; set; } = new List<ExpenseRequest>();

    public virtual ICollection<IssueReport> IssueReportsGestite { get; set; } = new List<IssueReport>();

    public virtual ICollection<MaterialRequest> MaterialRequestsGestite { get; set; } = new List<MaterialRequest>();

    public virtual ICollection<RefreshTokens> RefreshTokens { get; set; } = new List<RefreshTokens>();

    public virtual ICollection<UtentiRuoli> UtentiRuolis { get; set; } = new List<UtentiRuoli>();

    public virtual ICollection<UtentiVisibilita> UtentiVisibilitas { get; set; } = new List<UtentiVisibilita>();
}

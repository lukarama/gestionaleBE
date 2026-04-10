using System;
using System.Collections.Generic;
using Gestionale.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Gestionale.Api.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AssegnazioniDpi> AssegnazioniDpis { get; set; }

    public virtual DbSet<AssegnazioniMateriali> AssegnazioniMaterialis { get; set; }

    public virtual DbSet<AssegnazioniMezzi> AssegnazioniMezzis { get; set; }

    public virtual DbSet<Cantieri> Cantieris { get; set; }

    public virtual DbSet<CategorieDpi> CategorieDpis { get; set; }

    public virtual DbSet<CategorieMateriale> CategorieMateriales { get; set; }

    public virtual DbSet<Dipendenti> Dipendentis { get; set; }

    public virtual DbSet<DocumentiDipendenti> DocumentiDipendentis { get; set; }

    public virtual DbSet<DocumentiMezzi> DocumentiMezzis { get; set; }

    public virtual DbSet<Dpi> Dpis { get; set; }

    public virtual DbSet<EsitiVisitaMedica> EsitiVisitaMedicas { get; set; }

    public virtual DbSet<Fornitori> Fornitoris { get; set; }

    public virtual DbSet<Mansioni> Mansionis { get; set; }

    public virtual DbSet<Materiali> Materialis { get; set; }

    public virtual DbSet<Mezzi> Mezzis { get; set; }

    public virtual DbSet<MovimentiMateriale> MovimentiMateriales { get; set; }

    public virtual DbSet<Ruoli> Ruolis { get; set; }

    public virtual DbSet<StatiAssegnazione> StatiAssegnaziones { get; set; }

    public virtual DbSet<TipiDocumento> TipiDocumentos { get; set; }

    public virtual DbSet<TipiMovimentoMateriale> TipiMovimentoMateriales { get; set; }

    public virtual DbSet<TipiVisitaMedica> TipiVisitaMedicas { get; set; }

    public virtual DbSet<TipologieMezzo> TipologieMezzos { get; set; }

    public virtual DbSet<Utenti> Utentis { get; set; }

    public virtual DbSet<UtentiRuoli> UtentiRuolis { get; set; }

    public virtual DbSet<VisiteMediche> VisiteMediches { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost\\GESTIONALE;Database=GestionaleAziendale;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AssegnazioniDpi>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Assegnaz__3214EC07C61ED0C1");

            entity.ToTable("AssegnazioniDpi");

            entity.HasIndex(e => e.CantiereId, "IX_AssegnazioniDpi_CantiereId");

            entity.HasIndex(e => e.DataScadenza, "IX_AssegnazioniDpi_DataScadenza");

            entity.HasIndex(e => e.DipendenteId, "IX_AssegnazioniDpi_DipendenteId");

            entity.HasIndex(e => e.DpiId, "IX_AssegnazioniDpi_DpiId");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())", "DF_AssegnazioniDpi_CreatedAt");
            entity.Property(e => e.Note).HasMaxLength(1000);
            entity.Property(e => e.Quantita).HasDefaultValue(1, "DF_AssegnazioniDpi_Quantita");

            entity.HasOne(d => d.Cantiere).WithMany(p => p.AssegnazioniDpis)
                .HasForeignKey(d => d.CantiereId)
                .HasConstraintName("FK_AssegnazioniDpi_Cantieri");

            entity.HasOne(d => d.Dipendente).WithMany(p => p.AssegnazioniDpis)
                .HasForeignKey(d => d.DipendenteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AssegnazioniDpi_Dipendenti");

            entity.HasOne(d => d.Dpi).WithMany(p => p.AssegnazioniDpis)
                .HasForeignKey(d => d.DpiId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AssegnazioniDpi_Dpi");

            entity.HasOne(d => d.StatoAssegnazione).WithMany(p => p.AssegnazioniDpis)
                .HasForeignKey(d => d.StatoAssegnazioneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AssegnazioniDpi_StatiAssegnazione");
        });

        modelBuilder.Entity<AssegnazioniMateriali>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Assegnaz__3214EC0713A04FEE");

            entity.ToTable("AssegnazioniMateriali");

            entity.HasIndex(e => e.CantiereId, "IX_AssegnazioniMateriali_CantiereId");

            entity.HasIndex(e => e.DataAssegnazione, "IX_AssegnazioniMateriali_DataAssegnazione");

            entity.HasIndex(e => e.DipendenteId, "IX_AssegnazioniMateriali_DipendenteId");

            entity.HasIndex(e => e.MaterialeId, "IX_AssegnazioniMateriali_MaterialeId");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())", "DF_AssegnazioniMateriali_CreatedAt");
            entity.Property(e => e.Note).HasMaxLength(1000);
            entity.Property(e => e.Quantita).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Cantiere).WithMany(p => p.AssegnazioniMaterialis)
                .HasForeignKey(d => d.CantiereId)
                .HasConstraintName("FK_AssegnazioniMateriali_Cantieri");

            entity.HasOne(d => d.Dipendente).WithMany(p => p.AssegnazioniMaterialis)
                .HasForeignKey(d => d.DipendenteId)
                .HasConstraintName("FK_AssegnazioniMateriali_Dipendenti");

            entity.HasOne(d => d.Materiale).WithMany(p => p.AssegnazioniMaterialis)
                .HasForeignKey(d => d.MaterialeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AssegnazioniMateriali_Materiali");

            entity.HasOne(d => d.StatoAssegnazione).WithMany(p => p.AssegnazioniMaterialis)
                .HasForeignKey(d => d.StatoAssegnazioneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AssegnazioniMateriali_StatiAssegnazione");
        });

        modelBuilder.Entity<AssegnazioniMezzi>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Assegnaz__3214EC0706704BD8");

            entity.ToTable("AssegnazioniMezzi");

            entity.HasIndex(e => e.CantiereId, "IX_AssegnazioniMezzi_CantiereId");

            entity.HasIndex(e => e.DataInizio, "IX_AssegnazioniMezzi_DataInizio");

            entity.HasIndex(e => e.DipendenteId, "IX_AssegnazioniMezzi_DipendenteId");

            entity.HasIndex(e => e.MezzoId, "IX_AssegnazioniMezzi_MezzoId");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())", "DF_AssegnazioniMezzi_CreatedAt");
            entity.Property(e => e.Note).HasMaxLength(1000);

            entity.HasOne(d => d.Cantiere).WithMany(p => p.AssegnazioniMezzis)
                .HasForeignKey(d => d.CantiereId)
                .HasConstraintName("FK_AssegnazioniMezzi_Cantieri");

            entity.HasOne(d => d.Dipendente).WithMany(p => p.AssegnazioniMezzis)
                .HasForeignKey(d => d.DipendenteId)
                .HasConstraintName("FK_AssegnazioniMezzi_Dipendenti");

            entity.HasOne(d => d.Mezzo).WithMany(p => p.AssegnazioniMezzis)
                .HasForeignKey(d => d.MezzoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AssegnazioniMezzi_Mezzi");

            entity.HasOne(d => d.StatoAssegnazione).WithMany(p => p.AssegnazioniMezzis)
                .HasForeignKey(d => d.StatoAssegnazioneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AssegnazioniMezzi_StatiAssegnazione");
        });

        modelBuilder.Entity<Cantieri>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cantieri__3214EC078EDC8A94");

            entity.ToTable("Cantieri");

            entity.HasIndex(e => e.Nome, "IX_Cantieri_Nome");

            entity.HasIndex(e => e.Codice, "UX_Cantieri_Codice")
                .IsUnique()
                .HasFilter("([Codice] IS NOT NULL)");

            entity.Property(e => e.Attivo).HasDefaultValue(true, "DF_Cantieri_Attivo");
            entity.Property(e => e.Cap).HasMaxLength(5);
            entity.Property(e => e.Citta).HasMaxLength(100);
            entity.Property(e => e.Cliente).HasMaxLength(200);
            entity.Property(e => e.Codice).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())", "DF_Cantieri_CreatedAt");
            entity.Property(e => e.Indirizzo).HasMaxLength(255);
            entity.Property(e => e.Nome).HasMaxLength(200);
            entity.Property(e => e.Note).HasMaxLength(1000);
            entity.Property(e => e.Provincia).HasMaxLength(2);
        });

        modelBuilder.Entity<CategorieDpi>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC0788DB269E");

            entity.ToTable("CategorieDpi");

            entity.HasIndex(e => e.Nome, "UX_CategorieDpi_Nome").IsUnique();

            entity.Property(e => e.Attivo).HasDefaultValue(true, "DF_CategorieDpi_Attivo");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())", "DF_CategorieDpi_CreatedAt");
            entity.Property(e => e.Descrizione).HasMaxLength(255);
            entity.Property(e => e.Nome).HasMaxLength(100);
        });

        modelBuilder.Entity<CategorieMateriale>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC0722FDA59A");

            entity.ToTable("CategorieMateriale");

            entity.HasIndex(e => e.Nome, "UX_CategorieMateriale_Nome").IsUnique();

            entity.Property(e => e.Attivo).HasDefaultValue(true, "DF_CategorieMateriale_Attivo");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())", "DF_CategorieMateriale_CreatedAt");
            entity.Property(e => e.Descrizione).HasMaxLength(255);
            entity.Property(e => e.Nome).HasMaxLength(100);
        });

        modelBuilder.Entity<Dipendenti>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Dipenden__3214EC079E4CF650");

            entity.ToTable("Dipendenti");

            entity.HasIndex(e => new { e.Cognome, e.Nome }, "IX_Dipendenti_Cognome_Nome");

            entity.HasIndex(e => e.CodiceFiscale, "UX_Dipendenti_CodiceFiscale")
                .IsUnique()
                .HasFilter("([CodiceFiscale] IS NOT NULL)");

            entity.HasIndex(e => e.Matricola, "UX_Dipendenti_Matricola")
                .IsUnique()
                .HasFilter("([Matricola] IS NOT NULL)");

            entity.Property(e => e.Attivo).HasDefaultValue(true, "DF_Dipendenti_Attivo");
            entity.Property(e => e.Cap).HasMaxLength(5);
            entity.Property(e => e.Citta).HasMaxLength(100);
            entity.Property(e => e.CodiceFiscale).HasMaxLength(16);
            entity.Property(e => e.Cognome).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())", "DF_Dipendenti_CreatedAt");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Indirizzo).HasMaxLength(255);
            entity.Property(e => e.LuogoNascita).HasMaxLength(150);
            entity.Property(e => e.Matricola).HasMaxLength(50);
            entity.Property(e => e.Nome).HasMaxLength(100);
            entity.Property(e => e.Note).HasMaxLength(1000);
            entity.Property(e => e.Provincia).HasMaxLength(2);
            entity.Property(e => e.Telefono).HasMaxLength(50);

            entity.HasOne(d => d.Mansione).WithMany(p => p.Dipendentis)
                .HasForeignKey(d => d.MansioneId)
                .HasConstraintName("FK_Dipendenti_Mansioni");
        });

        modelBuilder.Entity<DocumentiDipendenti>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Document__3214EC07260F04D2");

            entity.ToTable("DocumentiDipendenti");

            entity.HasIndex(e => e.DataScadenza, "IX_DocumentiDipendenti_DataScadenza");

            entity.HasIndex(e => e.DipendenteId, "IX_DocumentiDipendenti_DipendenteId");

            entity.Property(e => e.ContentType).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())", "DF_DocumentiDipendenti_CreatedAt");
            entity.Property(e => e.Estensione).HasMaxLength(20);
            entity.Property(e => e.NomeFile).HasMaxLength(255);
            entity.Property(e => e.Note).HasMaxLength(1000);
            entity.Property(e => e.PercorsoFile).HasMaxLength(500);

            entity.HasOne(d => d.Dipendente).WithMany(p => p.DocumentiDipendentis)
                .HasForeignKey(d => d.DipendenteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DocumentiDipendenti_Dipendenti");

            entity.HasOne(d => d.TipoDocumento).WithMany(p => p.DocumentiDipendentis)
                .HasForeignKey(d => d.TipoDocumentoId)
                .HasConstraintName("FK_DocumentiDipendenti_TipiDocumento");
        });

        modelBuilder.Entity<DocumentiMezzi>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Document__3214EC07D88B4A2B");

            entity.ToTable("DocumentiMezzi");

            entity.HasIndex(e => e.DataScadenza, "IX_DocumentiMezzi_DataScadenza");

            entity.HasIndex(e => e.MezzoId, "IX_DocumentiMezzi_MezzoId");

            entity.Property(e => e.ContentType).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())", "DF_DocumentiMezzi_CreatedAt");
            entity.Property(e => e.Estensione).HasMaxLength(20);
            entity.Property(e => e.NomeFile).HasMaxLength(255);
            entity.Property(e => e.Note).HasMaxLength(1000);
            entity.Property(e => e.PercorsoFile).HasMaxLength(500);

            entity.HasOne(d => d.Mezzo).WithMany(p => p.DocumentiMezzis)
                .HasForeignKey(d => d.MezzoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DocumentiMezzi_Mezzi");

            entity.HasOne(d => d.TipoDocumento).WithMany(p => p.DocumentiMezzis)
                .HasForeignKey(d => d.TipoDocumentoId)
                .HasConstraintName("FK_DocumentiMezzi_TipiDocumento");
        });

        modelBuilder.Entity<Dpi>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Dpi__3214EC0738F9D218");

            entity.ToTable("Dpi");

            entity.HasIndex(e => e.Nome, "IX_Dpi_Nome");

            entity.HasIndex(e => e.Barcode, "UX_Dpi_Barcode")
                .IsUnique()
                .HasFilter("([Barcode] IS NOT NULL)");

            entity.HasIndex(e => e.Codice, "UX_Dpi_Codice")
                .IsUnique()
                .HasFilter("([Codice] IS NOT NULL)");

            entity.Property(e => e.Attivo).HasDefaultValue(true, "DF_Dpi_Attivo");
            entity.Property(e => e.Barcode).HasMaxLength(100);
            entity.Property(e => e.Codice).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())", "DF_Dpi_CreatedAt");
            entity.Property(e => e.Descrizione).HasMaxLength(500);
            entity.Property(e => e.Marca).HasMaxLength(100);
            entity.Property(e => e.Modello).HasMaxLength(100);
            entity.Property(e => e.Nome).HasMaxLength(150);
            entity.Property(e => e.Note).HasMaxLength(1000);
            entity.Property(e => e.Taglia).HasMaxLength(50);

            entity.HasOne(d => d.CategoriaDpi).WithMany(p => p.Dpis)
                .HasForeignKey(d => d.CategoriaDpiId)
                .HasConstraintName("FK_Dpi_CategorieDpi");

            entity.HasOne(d => d.Fornitore).WithMany(p => p.Dpis)
                .HasForeignKey(d => d.FornitoreId)
                .HasConstraintName("FK_Dpi_Fornitori");
        });

        modelBuilder.Entity<EsitiVisitaMedica>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EsitiVis__3214EC0777431164");

            entity.ToTable("EsitiVisitaMedica");

            entity.HasIndex(e => e.Nome, "UX_EsitiVisitaMedica_Nome").IsUnique();

            entity.Property(e => e.Attivo).HasDefaultValue(true, "DF_EsitiVisitaMedica_Attivo");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())", "DF_EsitiVisitaMedica_CreatedAt");
            entity.Property(e => e.Descrizione).HasMaxLength(255);
            entity.Property(e => e.Nome).HasMaxLength(100);
        });

        modelBuilder.Entity<Fornitori>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Fornitor__3214EC0793EC0173");

            entity.ToTable("Fornitori");

            entity.HasIndex(e => e.PartitaIva, "UX_Fornitori_PartitaIva")
                .IsUnique()
                .HasFilter("([PartitaIva] IS NOT NULL)");

            entity.Property(e => e.Attivo).HasDefaultValue(true, "DF_Fornitori_Attivo");
            entity.Property(e => e.Cap).HasMaxLength(5);
            entity.Property(e => e.Citta).HasMaxLength(100);
            entity.Property(e => e.CodiceFiscale).HasMaxLength(16);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())", "DF_Fornitori_CreatedAt");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Indirizzo).HasMaxLength(255);
            entity.Property(e => e.Note).HasMaxLength(1000);
            entity.Property(e => e.PartitaIva).HasMaxLength(11);
            entity.Property(e => e.Provincia).HasMaxLength(2);
            entity.Property(e => e.RagioneSociale).HasMaxLength(200);
            entity.Property(e => e.Telefono).HasMaxLength(50);
        });

        modelBuilder.Entity<Mansioni>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Mansioni__3214EC0711E4A07C");

            entity.ToTable("Mansioni");

            entity.HasIndex(e => e.Nome, "UX_Mansioni_Nome").IsUnique();

            entity.Property(e => e.Attivo).HasDefaultValue(true, "DF_Mansioni_Attivo");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())", "DF_Mansioni_CreatedAt");
            entity.Property(e => e.Descrizione).HasMaxLength(255);
            entity.Property(e => e.Nome).HasMaxLength(150);
        });

        modelBuilder.Entity<Materiali>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Material__3214EC07764A8871");

            entity.ToTable("Materiali");

            entity.HasIndex(e => e.Nome, "IX_Materiali_Nome");

            entity.HasIndex(e => e.Barcode, "UX_Materiali_Barcode")
                .IsUnique()
                .HasFilter("([Barcode] IS NOT NULL)");

            entity.HasIndex(e => e.Codice, "UX_Materiali_Codice")
                .IsUnique()
                .HasFilter("([Codice] IS NOT NULL)");

            entity.Property(e => e.Attivo).HasDefaultValue(true, "DF_Materiali_Attivo");
            entity.Property(e => e.Barcode).HasMaxLength(100);
            entity.Property(e => e.Codice).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())", "DF_Materiali_CreatedAt");
            entity.Property(e => e.Descrizione).HasMaxLength(500);
            entity.Property(e => e.Nome).HasMaxLength(150);
            entity.Property(e => e.Note).HasMaxLength(1000);
            entity.Property(e => e.QuantitaAttuale).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ScortaMinima).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitaMisura).HasMaxLength(30);

            entity.HasOne(d => d.CategoriaMateriale).WithMany(p => p.Materialis)
                .HasForeignKey(d => d.CategoriaMaterialeId)
                .HasConstraintName("FK_Materiali_CategorieMateriale");

            entity.HasOne(d => d.Fornitore).WithMany(p => p.Materialis)
                .HasForeignKey(d => d.FornitoreId)
                .HasConstraintName("FK_Materiali_Fornitori");
        });

        modelBuilder.Entity<Mezzi>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Mezzi__3214EC07B3567637");

            entity.ToTable("Mezzi");

            entity.HasIndex(e => e.DataRevisione, "IX_Mezzi_DataRevisione");

            entity.HasIndex(e => e.DataScadenzaAssicurazione, "IX_Mezzi_DataScadenzaAssicurazione");

            entity.HasIndex(e => e.CodiceInterno, "UX_Mezzi_CodiceInterno")
                .IsUnique()
                .HasFilter("([CodiceInterno] IS NOT NULL)");

            entity.HasIndex(e => e.NumeroTelaio, "UX_Mezzi_NumeroTelaio")
                .IsUnique()
                .HasFilter("([NumeroTelaio] IS NOT NULL)");

            entity.HasIndex(e => e.Targa, "UX_Mezzi_Targa")
                .IsUnique()
                .HasFilter("([Targa] IS NOT NULL)");

            entity.Property(e => e.Attivo).HasDefaultValue(true, "DF_Mezzi_Attivo");
            entity.Property(e => e.CodiceInterno).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())", "DF_Mezzi_CreatedAt");
            entity.Property(e => e.Marca).HasMaxLength(100);
            entity.Property(e => e.Modello).HasMaxLength(100);
            entity.Property(e => e.Note).HasMaxLength(1000);
            entity.Property(e => e.NumeroTelaio).HasMaxLength(100);
            entity.Property(e => e.Targa).HasMaxLength(20);

            entity.HasOne(d => d.Fornitore).WithMany(p => p.Mezzis)
                .HasForeignKey(d => d.FornitoreId)
                .HasConstraintName("FK_Mezzi_Fornitori");

            entity.HasOne(d => d.TipologiaMezzo).WithMany(p => p.Mezzis)
                .HasForeignKey(d => d.TipologiaMezzoId)
                .HasConstraintName("FK_Mezzi_TipologieMezzo");
        });

        modelBuilder.Entity<MovimentiMateriale>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Moviment__3214EC07522EA9D9");

            entity.ToTable("MovimentiMateriale");

            entity.HasIndex(e => e.DataMovimento, "IX_MovimentiMateriale_DataMovimento");

            entity.HasIndex(e => e.MaterialeId, "IX_MovimentiMateriale_MaterialeId");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())", "DF_MovimentiMateriale_CreatedAt");
            entity.Property(e => e.DataMovimento).HasDefaultValueSql("(sysdatetime())", "DF_MovimentiMateriale_DataMovimento");
            entity.Property(e => e.Note).HasMaxLength(1000);
            entity.Property(e => e.Quantita).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RiferimentoTabella).HasMaxLength(100);

            entity.HasOne(d => d.Cantiere).WithMany(p => p.MovimentiMateriales)
                .HasForeignKey(d => d.CantiereId)
                .HasConstraintName("FK_MovimentiMateriale_Cantieri");

            entity.HasOne(d => d.Dipendente).WithMany(p => p.MovimentiMateriales)
                .HasForeignKey(d => d.DipendenteId)
                .HasConstraintName("FK_MovimentiMateriale_Dipendenti");

            entity.HasOne(d => d.Materiale).WithMany(p => p.MovimentiMateriales)
                .HasForeignKey(d => d.MaterialeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MovimentiMateriale_Materiali");

            entity.HasOne(d => d.TipoMovimentoMateriale).WithMany(p => p.MovimentiMateriales)
                .HasForeignKey(d => d.TipoMovimentoMaterialeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MovimentiMateriale_TipiMovimentoMateriale");
        });

        modelBuilder.Entity<Ruoli>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Ruoli__3214EC0763C9D54C");

            entity.ToTable("Ruoli");

            entity.HasIndex(e => e.Nome, "UX_Ruoli_Nome").IsUnique();

            entity.Property(e => e.Attivo).HasDefaultValue(true, "DF_Ruoli_Attivo");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())", "DF_Ruoli_CreatedAt");
            entity.Property(e => e.Descrizione).HasMaxLength(255);
            entity.Property(e => e.Nome).HasMaxLength(100);
        });

        modelBuilder.Entity<StatiAssegnazione>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__StatiAss__3214EC07CE1ACDDF");

            entity.ToTable("StatiAssegnazione");

            entity.HasIndex(e => e.Nome, "UX_StatiAssegnazione_Nome").IsUnique();

            entity.Property(e => e.Attivo).HasDefaultValue(true, "DF_StatiAssegnazione_Attivo");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())", "DF_StatiAssegnazione_CreatedAt");
            entity.Property(e => e.Descrizione).HasMaxLength(255);
            entity.Property(e => e.Nome).HasMaxLength(100);
        });

        modelBuilder.Entity<TipiDocumento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TipiDocu__3214EC07CA798AC6");

            entity.ToTable("TipiDocumento");

            entity.HasIndex(e => e.Nome, "UX_TipiDocumento_Nome").IsUnique();

            entity.Property(e => e.Attivo).HasDefaultValue(true, "DF_TipiDocumento_Attivo");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())", "DF_TipiDocumento_CreatedAt");
            entity.Property(e => e.Descrizione).HasMaxLength(255);
            entity.Property(e => e.Nome).HasMaxLength(150);
        });

        modelBuilder.Entity<TipiMovimentoMateriale>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TipiMovi__3214EC07863B191C");

            entity.ToTable("TipiMovimentoMateriale");

            entity.HasIndex(e => e.Nome, "UX_TipiMovimentoMateriale_Nome").IsUnique();

            entity.Property(e => e.Attivo).HasDefaultValue(true, "DF_TipiMovimentoMateriale_Attivo");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())", "DF_TipiMovimentoMateriale_CreatedAt");
            entity.Property(e => e.Descrizione).HasMaxLength(255);
            entity.Property(e => e.Nome).HasMaxLength(100);
        });

        modelBuilder.Entity<TipiVisitaMedica>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TipiVisi__3214EC07FEB77A1C");

            entity.ToTable("TipiVisitaMedica");

            entity.HasIndex(e => e.Nome, "UX_TipiVisitaMedica_Nome").IsUnique();

            entity.Property(e => e.Attivo).HasDefaultValue(true, "DF_TipiVisitaMedica_Attivo");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())", "DF_TipiVisitaMedica_CreatedAt");
            entity.Property(e => e.Descrizione).HasMaxLength(255);
            entity.Property(e => e.Nome).HasMaxLength(150);
        });

        modelBuilder.Entity<TipologieMezzo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tipologi__3214EC0789568B9B");

            entity.ToTable("TipologieMezzo");

            entity.HasIndex(e => e.Nome, "UX_TipologieMezzo_Nome").IsUnique();

            entity.Property(e => e.Attivo).HasDefaultValue(true, "DF_TipologieMezzo_Attivo");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())", "DF_TipologieMezzo_CreatedAt");
            entity.Property(e => e.Descrizione).HasMaxLength(255);
            entity.Property(e => e.Nome).HasMaxLength(100);
        });

        modelBuilder.Entity<Utenti>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Utenti__3214EC07AC96C369");

            entity.ToTable("Utenti");

            entity.HasIndex(e => e.Email, "UX_Utenti_Email").IsUnique();

            entity.HasIndex(e => e.Username, "UX_Utenti_Username").IsUnique();

            entity.Property(e => e.Attivo).HasDefaultValue(true, "DF_Utenti_Attivo");
            entity.Property(e => e.Cognome).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())", "DF_Utenti_CreatedAt");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Nome).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(500);
            entity.Property(e => e.Username).HasMaxLength(100);
        });

        modelBuilder.Entity<UtentiRuoli>(entity =>
        {
            entity.HasKey(e => new { e.UtenteId, e.RuoloId });

            entity.ToTable("UtentiRuoli");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())", "DF_UtentiRuoli_CreatedAt");

            entity.HasOne(d => d.Ruolo).WithMany(p => p.UtentiRuolis)
                .HasForeignKey(d => d.RuoloId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UtentiRuoli_Ruoli");

            entity.HasOne(d => d.Utente).WithMany(p => p.UtentiRuolis)
                .HasForeignKey(d => d.UtenteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UtentiRuoli_Utenti");
        });

        modelBuilder.Entity<VisiteMediche>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__VisiteMe__3214EC07C60CE5B8");

            entity.ToTable("VisiteMediche");

            entity.HasIndex(e => e.DataScadenza, "IX_VisiteMediche_DataScadenza");

            entity.HasIndex(e => e.DipendenteId, "IX_VisiteMediche_DipendenteId");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())", "DF_VisiteMediche_CreatedAt");
            entity.Property(e => e.MedicoCompetente).HasMaxLength(200);
            entity.Property(e => e.Note).HasMaxLength(1000);
            entity.Property(e => e.Prescrizioni).HasMaxLength(2000);
            entity.Property(e => e.StrutturaSanitaria).HasMaxLength(200);

            entity.HasOne(d => d.Dipendente).WithMany(p => p.VisiteMediches)
                .HasForeignKey(d => d.DipendenteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VisiteMediche_Dipendenti");

            entity.HasOne(d => d.EsitoVisitaMedica).WithMany(p => p.VisiteMediches)
                .HasForeignKey(d => d.EsitoVisitaMedicaId)
                .HasConstraintName("FK_VisiteMediche_EsitiVisitaMedica");

            entity.HasOne(d => d.TipoVisitaMedica).WithMany(p => p.VisiteMediches)
                .HasForeignKey(d => d.TipoVisitaMedicaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VisiteMediche_TipiVisitaMedica");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Gestionale.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgresSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cantieri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Indirizzo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ResponsabileCantiere = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DirezioneLavori = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Committente = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Appaltatore = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DataInizioLavori = table.Column<DateOnly>(type: "date", nullable: true),
                    DataPrevistaFineLavori = table.Column<DateOnly>(type: "date", nullable: true),
                    Attivo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Note = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Cantieri__3214EC078EDC8A94", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategorieDpi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descrizione = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Attivo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Categori__3214EC0788DB269E", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategorieMateriale",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descrizione = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Attivo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Categori__3214EC0722FDA59A", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EsitiVisitaMedica",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descrizione = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Attivo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__EsitiVis__3214EC0777431164", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Fornitori",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RagioneSociale = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PartitaIva = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    CodiceFiscale = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    Telefono = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Indirizzo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Citta = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Provincia = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    Cap = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    Note = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Attivo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Fornitor__3214EC0793EC0173", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Mansioni",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Descrizione = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Attivo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Mansioni__3214EC0711E4A07C", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permessi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codice = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Risorsa = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Azione = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descrizione = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Attivo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Permessi__3214EC07A0B017E1", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ruoli",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descrizione = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Attivo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Ruoli__3214EC0763C9D54C", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StatiAssegnazione",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descrizione = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Attivo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__StatiAss__3214EC07CE1ACDDF", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipiAssenza",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descrizione = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Attivo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipiAssenza", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipiDocumento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Descrizione = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Attivo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TipiDocu__3214EC07CA798AC6", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipiMovimentoMateriale",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descrizione = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Segno = table.Column<short>(type: "smallint", nullable: false),
                    Attivo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TipiMovi__3214EC07863B191C", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipiVisitaMedica",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Descrizione = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Attivo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TipiVisi__3214EC07FEB77A1C", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipologieMezzo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descrizione = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Attivo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Tipologi__3214EC0789568B9B", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentiCantieri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CantiereId = table.Column<int>(type: "integer", nullable: false),
                    NomeFile = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PercorsoFile = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Estensione = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DataDocumento = table.Column<DateOnly>(type: "date", nullable: true),
                    Note = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Document__3214EC07CANTIERE", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentiCantieri_Cantieri",
                        column: x => x.CantiereId,
                        principalTable: "Cantieri",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Dpi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codice = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    CategoriaDpiId = table.Column<int>(type: "integer", nullable: true),
                    Descrizione = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Taglia = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Marca = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Modello = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Barcode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FornitoreId = table.Column<int>(type: "integer", nullable: true),
                    DurataGiorni = table.Column<int>(type: "integer", nullable: true),
                    QuantitaDisponibile = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    QuantitaMinima = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    RichiedeTaglia = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    HaScadenza = table.Column<bool>(type: "boolean", nullable: false),
                    Attivo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Note = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Dpi__3214EC0738F9D218", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dpi_CategorieDpi",
                        column: x => x.CategoriaDpiId,
                        principalTable: "CategorieDpi",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Dpi_Fornitori",
                        column: x => x.FornitoreId,
                        principalTable: "Fornitori",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Materiali",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codice = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    CategoriaMaterialeId = table.Column<int>(type: "integer", nullable: true),
                    Descrizione = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    UnitaMisura = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    QuantitaAttuale = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ScortaMinima = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Barcode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FornitoreId = table.Column<int>(type: "integer", nullable: true),
                    Attivo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Note = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Material__3214EC07764A8871", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Materiali_CategorieMateriale",
                        column: x => x.CategoriaMaterialeId,
                        principalTable: "CategorieMateriale",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Materiali_Fornitori",
                        column: x => x.FornitoreId,
                        principalTable: "Fornitori",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Dipendenti",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Matricola = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Cognome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CodiceFiscale = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    DataNascita = table.Column<DateOnly>(type: "date", nullable: true),
                    LuogoNascita = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Telefono = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Indirizzo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Citta = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Provincia = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    Cap = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    DataAssunzione = table.Column<DateOnly>(type: "date", nullable: true),
                    DataCessazione = table.Column<DateOnly>(type: "date", nullable: true),
                    HaPatente = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CategoriaPatente = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    MansioneId = table.Column<int>(type: "integer", nullable: true),
                    Attivo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Note = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Dipenden__3214EC079E4CF650", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dipendenti_Mansioni",
                        column: x => x.MansioneId,
                        principalTable: "Mansioni",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RuoliPermessi",
                columns: table => new
                {
                    RuoloId = table.Column<int>(type: "integer", nullable: false),
                    PermessoId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RuoliPermessi", x => new { x.RuoloId, x.PermessoId });
                    table.ForeignKey(
                        name: "FK_RuoliPermessi_Permessi",
                        column: x => x.PermessoId,
                        principalTable: "Permessi",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RuoliPermessi_Ruoli",
                        column: x => x.RuoloId,
                        principalTable: "Ruoli",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Mezzi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Targa = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    NumeroTelaio = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CodiceInterno = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TipologiaMezzoId = table.Column<int>(type: "integer", nullable: true),
                    Marca = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Modello = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    AnnoImmatricolazione = table.Column<int>(type: "integer", nullable: true),
                    DataImmatricolazione = table.Column<DateOnly>(type: "date", nullable: true),
                    DataRevisione = table.Column<DateOnly>(type: "date", nullable: true),
                    DataScadenzaBollo = table.Column<DateOnly>(type: "date", nullable: true),
                    DataScadenzaAssicurazione = table.Column<DateOnly>(type: "date", nullable: true),
                    DataTagliando = table.Column<DateOnly>(type: "date", nullable: true),
                    FornitoreId = table.Column<int>(type: "integer", nullable: true),
                    TipoPossesso = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Attivo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Note = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Mezzi__3214EC07B3567637", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mezzi_Fornitori",
                        column: x => x.FornitoreId,
                        principalTable: "Fornitori",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Mezzi_TipologieMezzo",
                        column: x => x.TipologiaMezzoId,
                        principalTable: "TipologieMezzo",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AssegnazioniDpi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DipendenteId = table.Column<int>(type: "integer", nullable: false),
                    DpiId = table.Column<int>(type: "integer", nullable: false),
                    CantiereId = table.Column<int>(type: "integer", nullable: true),
                    Quantita = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    StatoAssegnazioneId = table.Column<int>(type: "integer", nullable: false),
                    DataConsegna = table.Column<DateOnly>(type: "date", nullable: false),
                    DataScadenza = table.Column<DateOnly>(type: "date", nullable: true),
                    DataRestituzione = table.Column<DateOnly>(type: "date", nullable: true),
                    FirmaConsegna = table.Column<bool>(type: "boolean", nullable: false),
                    Note = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Assegnaz__3214EC07C61ED0C1", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssegnazioniDpi_Cantieri",
                        column: x => x.CantiereId,
                        principalTable: "Cantieri",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssegnazioniDpi_Dipendenti",
                        column: x => x.DipendenteId,
                        principalTable: "Dipendenti",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssegnazioniDpi_Dpi",
                        column: x => x.DpiId,
                        principalTable: "Dpi",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssegnazioniDpi_StatiAssegnazione",
                        column: x => x.StatoAssegnazioneId,
                        principalTable: "StatiAssegnazione",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AssegnazioniMateriali",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaterialeId = table.Column<int>(type: "integer", nullable: false),
                    DipendenteId = table.Column<int>(type: "integer", nullable: true),
                    CantiereId = table.Column<int>(type: "integer", nullable: true),
                    Quantita = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    StatoAssegnazioneId = table.Column<int>(type: "integer", nullable: false),
                    DataAssegnazione = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataRestituzione = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Note = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Assegnaz__3214EC0713A04FEE", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssegnazioniMateriali_Cantieri",
                        column: x => x.CantiereId,
                        principalTable: "Cantieri",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssegnazioniMateriali_Dipendenti",
                        column: x => x.DipendenteId,
                        principalTable: "Dipendenti",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssegnazioniMateriali_Materiali",
                        column: x => x.MaterialeId,
                        principalTable: "Materiali",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssegnazioniMateriali_StatiAssegnazione",
                        column: x => x.StatoAssegnazioneId,
                        principalTable: "StatiAssegnazione",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Assenze",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DipendenteId = table.Column<int>(type: "integer", nullable: false),
                    TipoAssenzaId = table.Column<int>(type: "integer", nullable: false),
                    DataInizio = table.Column<DateOnly>(type: "date", nullable: false),
                    DataFine = table.Column<DateOnly>(type: "date", nullable: false),
                    Giorni = table.Column<int>(type: "integer", nullable: false),
                    Note = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    DataRichiesta = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Stato = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValue: "richiesto"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assenze", x => x.Id);
                    table.CheckConstraint("CK_Assenze_DataFine_DataInizio", "\"DataFine\" >= \"DataInizio\"");
                    table.CheckConstraint("CK_Assenze_Giorni", "\"Giorni\" > 0");
                    table.CheckConstraint("CK_Assenze_Stato", "\"Stato\" IN ('richiesto', 'approvato', 'rimandato', 'rifiutato')");
                    table.ForeignKey(
                        name: "FK_Assenze_Dipendenti",
                        column: x => x.DipendenteId,
                        principalTable: "Dipendenti",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Assenze_TipiAssenza",
                        column: x => x.TipoAssenzaId,
                        principalTable: "TipiAssenza",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MovimentiMateriale",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaterialeId = table.Column<int>(type: "integer", nullable: false),
                    TipoMovimentoMaterialeId = table.Column<int>(type: "integer", nullable: false),
                    Quantita = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DataMovimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    DipendenteId = table.Column<int>(type: "integer", nullable: true),
                    CantiereId = table.Column<int>(type: "integer", nullable: true),
                    RiferimentoTabella = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RiferimentoId = table.Column<int>(type: "integer", nullable: true),
                    Note = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Moviment__3214EC07522EA9D9", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovimentiMateriale_Cantieri",
                        column: x => x.CantiereId,
                        principalTable: "Cantieri",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MovimentiMateriale_Dipendenti",
                        column: x => x.DipendenteId,
                        principalTable: "Dipendenti",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MovimentiMateriale_Materiali",
                        column: x => x.MaterialeId,
                        principalTable: "Materiali",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MovimentiMateriale_TipiMovimentoMateriale",
                        column: x => x.TipoMovimentoMaterialeId,
                        principalTable: "TipiMovimentoMateriale",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Utenti",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PasswordHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Cognome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DipendenteId = table.Column<int>(type: "integer", nullable: true),
                    Attivo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    MustChangePassword = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    UltimoAccessoAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Utenti__3214EC07AC96C369", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Utenti_Dipendenti",
                        column: x => x.DipendenteId,
                        principalTable: "Dipendenti",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VisiteMediche",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DipendenteId = table.Column<int>(type: "integer", nullable: false),
                    TipoVisitaMedicaId = table.Column<int>(type: "integer", nullable: false),
                    DataVisita = table.Column<DateOnly>(type: "date", nullable: false),
                    DataScadenza = table.Column<DateOnly>(type: "date", nullable: true),
                    EsitoVisitaMedicaId = table.Column<int>(type: "integer", nullable: true),
                    Idoneo = table.Column<bool>(type: "boolean", nullable: true),
                    Prescrizioni = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    MedicoCompetente = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    StrutturaSanitaria = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Note = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__VisiteMe__3214EC07C60CE5B8", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VisiteMediche_Dipendenti",
                        column: x => x.DipendenteId,
                        principalTable: "Dipendenti",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VisiteMediche_EsitiVisitaMedica",
                        column: x => x.EsitoVisitaMedicaId,
                        principalTable: "EsitiVisitaMedica",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VisiteMediche_TipiVisitaMedica",
                        column: x => x.TipoVisitaMedicaId,
                        principalTable: "TipiVisitaMedica",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AssegnazioniMezzi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MezzoId = table.Column<int>(type: "integer", nullable: false),
                    DipendenteId = table.Column<int>(type: "integer", nullable: true),
                    CantiereId = table.Column<int>(type: "integer", nullable: true),
                    StatoAssegnazioneId = table.Column<int>(type: "integer", nullable: false),
                    DataInizio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataFine = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    KmConsegna = table.Column<int>(type: "integer", nullable: true),
                    KmRientro = table.Column<int>(type: "integer", nullable: true),
                    Note = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Assegnaz__3214EC0706704BD8", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssegnazioniMezzi_Cantieri",
                        column: x => x.CantiereId,
                        principalTable: "Cantieri",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssegnazioniMezzi_Dipendenti",
                        column: x => x.DipendenteId,
                        principalTable: "Dipendenti",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssegnazioniMezzi_Mezzi",
                        column: x => x.MezzoId,
                        principalTable: "Mezzi",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssegnazioniMezzi_StatiAssegnazione",
                        column: x => x.StatoAssegnazioneId,
                        principalTable: "StatiAssegnazione",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DocumentiMezzi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MezzoId = table.Column<int>(type: "integer", nullable: false),
                    TipoDocumentoId = table.Column<int>(type: "integer", nullable: true),
                    NomeFile = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PercorsoFile = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Estensione = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DataDocumento = table.Column<DateOnly>(type: "date", nullable: true),
                    DataScadenza = table.Column<DateOnly>(type: "date", nullable: true),
                    Note = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Document__3214EC07D88B4A2B", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentiMezzi_Mezzi",
                        column: x => x.MezzoId,
                        principalTable: "Mezzi",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DocumentiMezzi_TipiDocumento",
                        column: x => x.TipoDocumentoId,
                        principalTable: "TipiDocumento",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CartelleDocumentiDipendenti",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DipendenteId = table.Column<int>(type: "integer", nullable: false),
                    ParentCartellaId = table.Column<int>(type: "integer", nullable: true),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByUtenteId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartelleDocumentiDipendenti", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartelleDocumentiDipendenti_Dipendenti",
                        column: x => x.DipendenteId,
                        principalTable: "Dipendenti",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CartelleDocumentiDipendenti_Parent",
                        column: x => x.ParentCartellaId,
                        principalTable: "CartelleDocumentiDipendenti",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CartelleDocumentiDipendenti_Utenti_CreatedBy",
                        column: x => x.CreatedByUtenteId,
                        principalTable: "Utenti",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ExpenseRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DipendenteId = table.Column<int>(type: "integer", nullable: false),
                    DataSpesa = table.Column<DateOnly>(type: "date", nullable: false),
                    CategoriaSpesa = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Descrizione = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Importo = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Valuta = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    MetodoPagamento = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Stato = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValue: "IN_ATTESA"),
                    AllegatoNomeFile = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AllegatoPercorsoFile = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    AllegatoContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    AllegatoEstensione = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    NotaGestione = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    GestitoDaUtenteId = table.Column<int>(type: "integer", nullable: true),
                    GestitoAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseRequests", x => x.Id);
                    table.CheckConstraint("CK_ExpenseRequests_Importo", "\"Importo\" > 0");
                    table.CheckConstraint("CK_ExpenseRequests_Stato", "\"Stato\" IN ('IN_ATTESA', 'APPROVATA', 'RIFIUTATA', 'IN_REVISIONE')");
                    table.ForeignKey(
                        name: "FK_ExpenseRequests_Dipendenti",
                        column: x => x.DipendenteId,
                        principalTable: "Dipendenti",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ExpenseRequests_Utenti_GestitoDa",
                        column: x => x.GestitoDaUtenteId,
                        principalTable: "Utenti",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IssueReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DipendenteId = table.Column<int>(type: "integer", nullable: false),
                    Categoria = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Oggetto = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Luogo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Descrizione = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Priorita = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Note = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Stato = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValue: "IN_ATTESA"),
                    NotaGestione = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    GestitoDaUtenteId = table.Column<int>(type: "integer", nullable: true),
                    GestitoAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueReports", x => x.Id);
                    table.CheckConstraint("CK_IssueReports_Stato", "\"Stato\" IN ('IN_ATTESA', 'APPROVATA', 'RIFIUTATA', 'IN_REVISIONE')");
                    table.ForeignKey(
                        name: "FK_IssueReports_Dipendenti",
                        column: x => x.DipendenteId,
                        principalTable: "Dipendenti",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IssueReports_Utenti_GestitoDa",
                        column: x => x.GestitoDaUtenteId,
                        principalTable: "Utenti",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MaterialRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DipendenteId = table.Column<int>(type: "integer", nullable: false),
                    MaterialeRichiesto = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Quantita = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Motivazione = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Priorita = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    DataDesiderata = table.Column<DateOnly>(type: "date", nullable: false),
                    Note = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Stato = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValue: "IN_ATTESA"),
                    NotaGestione = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    GestitoDaUtenteId = table.Column<int>(type: "integer", nullable: true),
                    GestitoAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialRequests", x => x.Id);
                    table.CheckConstraint("CK_MaterialRequests_Quantita", "\"Quantita\" > 0");
                    table.CheckConstraint("CK_MaterialRequests_Stato", "\"Stato\" IN ('IN_ATTESA', 'APPROVATA', 'RIFIUTATA', 'IN_REVISIONE')");
                    table.ForeignKey(
                        name: "FK_MaterialRequests_Dipendenti",
                        column: x => x.DipendenteId,
                        principalTable: "Dipendenti",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MaterialRequests_Utenti_GestitoDa",
                        column: x => x.GestitoDaUtenteId,
                        principalTable: "Utenti",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UtenteId = table.Column<int>(type: "integer", nullable: false),
                    TokenHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    RevokedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByIp = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__RefreshT__3214EC078E3ABF53", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Utenti",
                        column: x => x.UtenteId,
                        principalTable: "Utenti",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UtentiRuoli",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UtenteId = table.Column<int>(type: "integer", nullable: false),
                    RuoloId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UtentiRuoli", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UtentiRuoli_Ruoli",
                        column: x => x.RuoloId,
                        principalTable: "Ruoli",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UtentiRuoli_Utenti",
                        column: x => x.UtenteId,
                        principalTable: "Utenti",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UtentiVisibilita",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UtenteId = table.Column<int>(type: "integer", nullable: false),
                    Chiave = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UtentiVisibilita", x => x.Id);
                    table.CheckConstraint("CK_UtentiVisibilita_Chiave", "\"Chiave\" IN ('dashboard', 'dipendenti', 'magazzino', 'attrezzature', 'dpi', 'mezzi', 'cantieri', 'segreteria')");
                    table.ForeignKey(
                        name: "FK_UtentiVisibilita_Utenti",
                        column: x => x.UtenteId,
                        principalTable: "Utenti",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DocumentiDipendenti",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DipendenteId = table.Column<int>(type: "integer", nullable: false),
                    CartellaId = table.Column<int>(type: "integer", nullable: true),
                    TipoDocumentoId = table.Column<int>(type: "integer", nullable: true),
                    NomeFile = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    NomeFileSalvato = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PercorsoFile = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Estensione = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DimensioneBytes = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    DataDocumento = table.Column<DateOnly>(type: "date", nullable: true),
                    DataScadenza = table.Column<DateOnly>(type: "date", nullable: true),
                    Note = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UploadedByUtenteId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Document__3214EC07260F04D2", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentiDipendenti_Cartelle",
                        column: x => x.CartellaId,
                        principalTable: "CartelleDocumentiDipendenti",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DocumentiDipendenti_Dipendenti",
                        column: x => x.DipendenteId,
                        principalTable: "Dipendenti",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DocumentiDipendenti_TipiDocumento",
                        column: x => x.TipoDocumentoId,
                        principalTable: "TipiDocumento",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DocumentiDipendenti_Utenti_UploadedBy",
                        column: x => x.UploadedByUtenteId,
                        principalTable: "Utenti",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssegnazioniDpi_CantiereId",
                table: "AssegnazioniDpi",
                column: "CantiereId");

            migrationBuilder.CreateIndex(
                name: "IX_AssegnazioniDpi_DataScadenza",
                table: "AssegnazioniDpi",
                column: "DataScadenza");

            migrationBuilder.CreateIndex(
                name: "IX_AssegnazioniDpi_DipendenteId",
                table: "AssegnazioniDpi",
                column: "DipendenteId");

            migrationBuilder.CreateIndex(
                name: "IX_AssegnazioniDpi_DpiId",
                table: "AssegnazioniDpi",
                column: "DpiId");

            migrationBuilder.CreateIndex(
                name: "IX_AssegnazioniDpi_StatoAssegnazioneId",
                table: "AssegnazioniDpi",
                column: "StatoAssegnazioneId");

            migrationBuilder.CreateIndex(
                name: "IX_AssegnazioniMateriali_CantiereId",
                table: "AssegnazioniMateriali",
                column: "CantiereId");

            migrationBuilder.CreateIndex(
                name: "IX_AssegnazioniMateriali_DataAssegnazione",
                table: "AssegnazioniMateriali",
                column: "DataAssegnazione");

            migrationBuilder.CreateIndex(
                name: "IX_AssegnazioniMateriali_DipendenteId",
                table: "AssegnazioniMateriali",
                column: "DipendenteId");

            migrationBuilder.CreateIndex(
                name: "IX_AssegnazioniMateriali_MaterialeId",
                table: "AssegnazioniMateriali",
                column: "MaterialeId");

            migrationBuilder.CreateIndex(
                name: "IX_AssegnazioniMateriali_StatoAssegnazioneId",
                table: "AssegnazioniMateriali",
                column: "StatoAssegnazioneId");

            migrationBuilder.CreateIndex(
                name: "IX_AssegnazioniMezzi_CantiereId",
                table: "AssegnazioniMezzi",
                column: "CantiereId");

            migrationBuilder.CreateIndex(
                name: "IX_AssegnazioniMezzi_DataInizio",
                table: "AssegnazioniMezzi",
                column: "DataInizio");

            migrationBuilder.CreateIndex(
                name: "IX_AssegnazioniMezzi_DipendenteId",
                table: "AssegnazioniMezzi",
                column: "DipendenteId");

            migrationBuilder.CreateIndex(
                name: "IX_AssegnazioniMezzi_MezzoId",
                table: "AssegnazioniMezzi",
                column: "MezzoId");

            migrationBuilder.CreateIndex(
                name: "IX_AssegnazioniMezzi_StatoAssegnazioneId",
                table: "AssegnazioniMezzi",
                column: "StatoAssegnazioneId");

            migrationBuilder.CreateIndex(
                name: "IX_Assenze_DataRichiesta",
                table: "Assenze",
                column: "DataRichiesta");

            migrationBuilder.CreateIndex(
                name: "IX_Assenze_DipendenteId",
                table: "Assenze",
                column: "DipendenteId");

            migrationBuilder.CreateIndex(
                name: "IX_Assenze_Stato",
                table: "Assenze",
                column: "Stato");

            migrationBuilder.CreateIndex(
                name: "IX_Assenze_TipoAssenzaId",
                table: "Assenze",
                column: "TipoAssenzaId");

            migrationBuilder.CreateIndex(
                name: "IX_Cantieri_Nome",
                table: "Cantieri",
                column: "Nome");

            migrationBuilder.CreateIndex(
                name: "IX_CartelleDocumentiDipendenti_CreatedByUtenteId",
                table: "CartelleDocumentiDipendenti",
                column: "CreatedByUtenteId");

            migrationBuilder.CreateIndex(
                name: "IX_CartelleDocumentiDipendenti_DipendenteId",
                table: "CartelleDocumentiDipendenti",
                column: "DipendenteId");

            migrationBuilder.CreateIndex(
                name: "IX_CartelleDocumentiDipendenti_ParentCartellaId",
                table: "CartelleDocumentiDipendenti",
                column: "ParentCartellaId");

            migrationBuilder.CreateIndex(
                name: "UX_CartelleDocumentiDipendenti_Dipendente_Nome_Root",
                table: "CartelleDocumentiDipendenti",
                columns: new[] { "DipendenteId", "Nome" },
                unique: true,
                filter: "\"ParentCartellaId\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "UX_CartelleDocumentiDipendenti_Dipendente_Parent_Nome",
                table: "CartelleDocumentiDipendenti",
                columns: new[] { "DipendenteId", "ParentCartellaId", "Nome" },
                unique: true,
                filter: "\"ParentCartellaId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_CategorieDpi_Nome",
                table: "CategorieDpi",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_CategorieMateriale_Nome",
                table: "CategorieMateriale",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dipendenti_Cognome_Nome",
                table: "Dipendenti",
                columns: new[] { "Cognome", "Nome" });

            migrationBuilder.CreateIndex(
                name: "IX_Dipendenti_MansioneId",
                table: "Dipendenti",
                column: "MansioneId");

            migrationBuilder.CreateIndex(
                name: "UX_Dipendenti_CodiceFiscale",
                table: "Dipendenti",
                column: "CodiceFiscale",
                unique: true,
                filter: "(\"CodiceFiscale\" IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "UX_Dipendenti_Matricola",
                table: "Dipendenti",
                column: "Matricola",
                unique: true,
                filter: "(\"Matricola\" IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentiCantieri_CantiereId",
                table: "DocumentiCantieri",
                column: "CantiereId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentiDipendenti_CartellaId",
                table: "DocumentiDipendenti",
                column: "CartellaId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentiDipendenti_DataScadenza",
                table: "DocumentiDipendenti",
                column: "DataScadenza");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentiDipendenti_DipendenteId",
                table: "DocumentiDipendenti",
                column: "DipendenteId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentiDipendenti_TipoDocumentoId",
                table: "DocumentiDipendenti",
                column: "TipoDocumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentiDipendenti_UploadedByUtenteId",
                table: "DocumentiDipendenti",
                column: "UploadedByUtenteId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentiMezzi_DataScadenza",
                table: "DocumentiMezzi",
                column: "DataScadenza");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentiMezzi_MezzoId",
                table: "DocumentiMezzi",
                column: "MezzoId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentiMezzi_TipoDocumentoId",
                table: "DocumentiMezzi",
                column: "TipoDocumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Dpi_CategoriaDpiId",
                table: "Dpi",
                column: "CategoriaDpiId");

            migrationBuilder.CreateIndex(
                name: "IX_Dpi_FornitoreId",
                table: "Dpi",
                column: "FornitoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Dpi_Nome",
                table: "Dpi",
                column: "Nome");

            migrationBuilder.CreateIndex(
                name: "UX_Dpi_Barcode",
                table: "Dpi",
                column: "Barcode",
                unique: true,
                filter: "(\"Barcode\" IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "UX_Dpi_Codice",
                table: "Dpi",
                column: "Codice",
                unique: true,
                filter: "(\"Codice\" IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "UX_EsitiVisitaMedica_Nome",
                table: "EsitiVisitaMedica",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseRequests_CreatedAt",
                table: "ExpenseRequests",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseRequests_DipendenteId",
                table: "ExpenseRequests",
                column: "DipendenteId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseRequests_GestitoDaUtenteId",
                table: "ExpenseRequests",
                column: "GestitoDaUtenteId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseRequests_Stato",
                table: "ExpenseRequests",
                column: "Stato");

            migrationBuilder.CreateIndex(
                name: "UX_Fornitori_PartitaIva",
                table: "Fornitori",
                column: "PartitaIva",
                unique: true,
                filter: "(\"PartitaIva\" IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_IssueReports_CreatedAt",
                table: "IssueReports",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_IssueReports_DipendenteId",
                table: "IssueReports",
                column: "DipendenteId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueReports_GestitoDaUtenteId",
                table: "IssueReports",
                column: "GestitoDaUtenteId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueReports_Stato",
                table: "IssueReports",
                column: "Stato");

            migrationBuilder.CreateIndex(
                name: "UX_Mansioni_Nome",
                table: "Mansioni",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Materiali_CategoriaMaterialeId",
                table: "Materiali",
                column: "CategoriaMaterialeId");

            migrationBuilder.CreateIndex(
                name: "IX_Materiali_FornitoreId",
                table: "Materiali",
                column: "FornitoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Materiali_Nome",
                table: "Materiali",
                column: "Nome");

            migrationBuilder.CreateIndex(
                name: "UX_Materiali_Barcode",
                table: "Materiali",
                column: "Barcode",
                unique: true,
                filter: "(\"Barcode\" IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "UX_Materiali_Codice",
                table: "Materiali",
                column: "Codice",
                unique: true,
                filter: "(\"Codice\" IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialRequests_CreatedAt",
                table: "MaterialRequests",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialRequests_DipendenteId",
                table: "MaterialRequests",
                column: "DipendenteId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialRequests_GestitoDaUtenteId",
                table: "MaterialRequests",
                column: "GestitoDaUtenteId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialRequests_Stato",
                table: "MaterialRequests",
                column: "Stato");

            migrationBuilder.CreateIndex(
                name: "IX_Mezzi_DataRevisione",
                table: "Mezzi",
                column: "DataRevisione");

            migrationBuilder.CreateIndex(
                name: "IX_Mezzi_DataScadenzaAssicurazione",
                table: "Mezzi",
                column: "DataScadenzaAssicurazione");

            migrationBuilder.CreateIndex(
                name: "IX_Mezzi_FornitoreId",
                table: "Mezzi",
                column: "FornitoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Mezzi_TipologiaMezzoId",
                table: "Mezzi",
                column: "TipologiaMezzoId");

            migrationBuilder.CreateIndex(
                name: "UX_Mezzi_CodiceInterno",
                table: "Mezzi",
                column: "CodiceInterno",
                unique: true,
                filter: "(\"CodiceInterno\" IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "UX_Mezzi_NumeroTelaio",
                table: "Mezzi",
                column: "NumeroTelaio",
                unique: true,
                filter: "(\"NumeroTelaio\" IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "UX_Mezzi_Targa",
                table: "Mezzi",
                column: "Targa",
                unique: true,
                filter: "(\"Targa\" IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_MovimentiMateriale_CantiereId",
                table: "MovimentiMateriale",
                column: "CantiereId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimentiMateriale_DataMovimento",
                table: "MovimentiMateriale",
                column: "DataMovimento");

            migrationBuilder.CreateIndex(
                name: "IX_MovimentiMateriale_DipendenteId",
                table: "MovimentiMateriale",
                column: "DipendenteId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimentiMateriale_MaterialeId",
                table: "MovimentiMateriale",
                column: "MaterialeId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimentiMateriale_TipoMovimentoMaterialeId",
                table: "MovimentiMateriale",
                column: "TipoMovimentoMaterialeId");

            migrationBuilder.CreateIndex(
                name: "UX_Permessi_Codice",
                table: "Permessi",
                column: "Codice",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UtenteId_ExpiresAt",
                table: "RefreshTokens",
                columns: new[] { "UtenteId", "ExpiresAt" });

            migrationBuilder.CreateIndex(
                name: "UX_RefreshTokens_TokenHash",
                table: "RefreshTokens",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Ruoli_Nome",
                table: "Ruoli",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RuoliPermessi_PermessoId",
                table: "RuoliPermessi",
                column: "PermessoId");

            migrationBuilder.CreateIndex(
                name: "UX_StatiAssegnazione_Nome",
                table: "StatiAssegnazione",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_TipiDocumento_Nome",
                table: "TipiDocumento",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_TipiMovimentoMateriale_Nome",
                table: "TipiMovimentoMateriale",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_TipiVisitaMedica_Nome",
                table: "TipiVisitaMedica",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_TipologieMezzo_Nome",
                table: "TipologieMezzo",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Utenti_DipendenteId",
                table: "Utenti",
                column: "DipendenteId",
                unique: true,
                filter: "(\"DipendenteId\" IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "UX_Utenti_Email",
                table: "Utenti",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Utenti_Username",
                table: "Utenti",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UtentiRuoli_RuoloId",
                table: "UtentiRuoli",
                column: "RuoloId");

            migrationBuilder.CreateIndex(
                name: "UX_UtentiRuoli_UtenteId_RuoloId",
                table: "UtentiRuoli",
                columns: new[] { "UtenteId", "RuoloId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_UtentiVisibilita_UtenteId_Chiave",
                table: "UtentiVisibilita",
                columns: new[] { "UtenteId", "Chiave" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VisiteMediche_DataScadenza",
                table: "VisiteMediche",
                column: "DataScadenza");

            migrationBuilder.CreateIndex(
                name: "IX_VisiteMediche_DipendenteId",
                table: "VisiteMediche",
                column: "DipendenteId");

            migrationBuilder.CreateIndex(
                name: "IX_VisiteMediche_EsitoVisitaMedicaId",
                table: "VisiteMediche",
                column: "EsitoVisitaMedicaId");

            migrationBuilder.CreateIndex(
                name: "IX_VisiteMediche_TipoVisitaMedicaId",
                table: "VisiteMediche",
                column: "TipoVisitaMedicaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssegnazioniDpi");

            migrationBuilder.DropTable(
                name: "AssegnazioniMateriali");

            migrationBuilder.DropTable(
                name: "AssegnazioniMezzi");

            migrationBuilder.DropTable(
                name: "Assenze");

            migrationBuilder.DropTable(
                name: "DocumentiCantieri");

            migrationBuilder.DropTable(
                name: "DocumentiDipendenti");

            migrationBuilder.DropTable(
                name: "DocumentiMezzi");

            migrationBuilder.DropTable(
                name: "ExpenseRequests");

            migrationBuilder.DropTable(
                name: "IssueReports");

            migrationBuilder.DropTable(
                name: "MaterialRequests");

            migrationBuilder.DropTable(
                name: "MovimentiMateriale");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "RuoliPermessi");

            migrationBuilder.DropTable(
                name: "UtentiRuoli");

            migrationBuilder.DropTable(
                name: "UtentiVisibilita");

            migrationBuilder.DropTable(
                name: "VisiteMediche");

            migrationBuilder.DropTable(
                name: "Dpi");

            migrationBuilder.DropTable(
                name: "StatiAssegnazione");

            migrationBuilder.DropTable(
                name: "TipiAssenza");

            migrationBuilder.DropTable(
                name: "CartelleDocumentiDipendenti");

            migrationBuilder.DropTable(
                name: "Mezzi");

            migrationBuilder.DropTable(
                name: "TipiDocumento");

            migrationBuilder.DropTable(
                name: "Cantieri");

            migrationBuilder.DropTable(
                name: "Materiali");

            migrationBuilder.DropTable(
                name: "TipiMovimentoMateriale");

            migrationBuilder.DropTable(
                name: "Permessi");

            migrationBuilder.DropTable(
                name: "Ruoli");

            migrationBuilder.DropTable(
                name: "EsitiVisitaMedica");

            migrationBuilder.DropTable(
                name: "TipiVisitaMedica");

            migrationBuilder.DropTable(
                name: "CategorieDpi");

            migrationBuilder.DropTable(
                name: "Utenti");

            migrationBuilder.DropTable(
                name: "TipologieMezzo");

            migrationBuilder.DropTable(
                name: "CategorieMateriale");

            migrationBuilder.DropTable(
                name: "Fornitori");

            migrationBuilder.DropTable(
                name: "Dipendenti");

            migrationBuilder.DropTable(
                name: "Mansioni");
        }
    }
}

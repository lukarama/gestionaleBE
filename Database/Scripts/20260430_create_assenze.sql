SET XACT_ABORT ON;

BEGIN TRANSACTION;

IF OBJECT_ID(N'dbo.TipiAssenza', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TipiAssenza
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_TipiAssenza PRIMARY KEY,
        Nome nvarchar(100) NOT NULL,
        Descrizione nvarchar(255) NULL,
        Attivo bit NOT NULL CONSTRAINT DF_TipiAssenza_Attivo DEFAULT (1),
        CreatedAt datetime2 NOT NULL CONSTRAINT DF_TipiAssenza_CreatedAt DEFAULT (sysutcdatetime()),
        UpdatedAt datetime2 NULL
    );
END;

IF OBJECT_ID(N'dbo.Assenze', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Assenze
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Assenze PRIMARY KEY,
        DipendenteId int NOT NULL,
        TipoAssenzaId int NOT NULL,
        DataInizio date NOT NULL,
        DataFine date NOT NULL,
        Giorni int NOT NULL,
        Note nvarchar(1000) NULL,
        DataRichiesta datetime2 NOT NULL CONSTRAINT DF_Assenze_DataRichiesta DEFAULT (sysutcdatetime()),
        Stato nvarchar(30) NOT NULL CONSTRAINT DF_Assenze_Stato DEFAULT (N'richiesto'),
        CreatedAt datetime2 NOT NULL CONSTRAINT DF_Assenze_CreatedAt DEFAULT (sysutcdatetime()),
        UpdatedAt datetime2 NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Assenze_Dipendenti' AND parent_object_id = OBJECT_ID(N'dbo.Assenze'))
BEGIN
    ALTER TABLE dbo.Assenze WITH CHECK ADD CONSTRAINT FK_Assenze_Dipendenti
        FOREIGN KEY (DipendenteId) REFERENCES dbo.Dipendenti(Id);
END;

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Assenze_TipiAssenza' AND parent_object_id = OBJECT_ID(N'dbo.Assenze'))
BEGIN
    ALTER TABLE dbo.Assenze WITH CHECK ADD CONSTRAINT FK_Assenze_TipiAssenza
        FOREIGN KEY (TipoAssenzaId) REFERENCES dbo.TipiAssenza(Id);
END;

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_Assenze_DataFine_DataInizio' AND parent_object_id = OBJECT_ID(N'dbo.Assenze'))
BEGIN
    ALTER TABLE dbo.Assenze WITH CHECK ADD CONSTRAINT CK_Assenze_DataFine_DataInizio
        CHECK (DataFine >= DataInizio);
END;

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_Assenze_Giorni' AND parent_object_id = OBJECT_ID(N'dbo.Assenze'))
BEGIN
    ALTER TABLE dbo.Assenze WITH CHECK ADD CONSTRAINT CK_Assenze_Giorni
        CHECK (Giorni > 0);
END;

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_Assenze_Stato' AND parent_object_id = OBJECT_ID(N'dbo.Assenze'))
BEGIN
    ALTER TABLE dbo.Assenze WITH CHECK ADD CONSTRAINT CK_Assenze_Stato
        CHECK (Stato IN (N'richiesto', N'approvato', N'rifiutato'));
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Assenze_DipendenteId' AND object_id = OBJECT_ID(N'dbo.Assenze'))
    CREATE INDEX IX_Assenze_DipendenteId ON dbo.Assenze(DipendenteId);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Assenze_TipoAssenzaId' AND object_id = OBJECT_ID(N'dbo.Assenze'))
    CREATE INDEX IX_Assenze_TipoAssenzaId ON dbo.Assenze(TipoAssenzaId);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Assenze_Stato' AND object_id = OBJECT_ID(N'dbo.Assenze'))
    CREATE INDEX IX_Assenze_Stato ON dbo.Assenze(Stato);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Assenze_DataRichiesta' AND object_id = OBJECT_ID(N'dbo.Assenze'))
    CREATE INDEX IX_Assenze_DataRichiesta ON dbo.Assenze(DataRichiesta);

INSERT INTO dbo.TipiAssenza (Nome, Descrizione, Attivo)
SELECT v.Nome, v.Descrizione, 1
FROM (VALUES
    (N'Ferie', NULL),
    (N'Permesso', NULL),
    (N'Malattia', NULL),
    (N'Permesso legge 104', NULL),
    (N'Congedo parentale', NULL),
    (N'Altro', NULL)
) AS v(Nome, Descrizione)
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.TipiAssenza t
    WHERE t.Nome = v.Nome
);

COMMIT TRANSACTION;

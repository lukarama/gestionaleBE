SET XACT_ABORT ON;

BEGIN TRANSACTION;

IF OBJECT_ID(N'dbo.Ruoli', N'U') IS NOT NULL
BEGIN
    INSERT INTO dbo.Ruoli (Nome, Descrizione, Attivo)
    SELECT N'RESPONSABILE', N'Può approvare, rifiutare o mettere in revisione le richieste aziendali.', 1
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.Ruoli
        WHERE Nome = N'RESPONSABILE'
    );
END;

IF OBJECT_ID(N'dbo.ExpenseRequests', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ExpenseRequests
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_ExpenseRequests PRIMARY KEY,
        DipendenteId int NOT NULL,
        DataSpesa date NOT NULL,
        CategoriaSpesa nvarchar(150) NOT NULL,
        Descrizione nvarchar(1000) NOT NULL,
        Importo decimal(18,2) NOT NULL,
        Valuta nvarchar(3) NOT NULL,
        MetodoPagamento nvarchar(100) NOT NULL,
        Stato nvarchar(30) NOT NULL CONSTRAINT DF_ExpenseRequests_Stato DEFAULT (N'IN_ATTESA'),
        AllegatoNomeFile nvarchar(255) NULL,
        AllegatoPercorsoFile nvarchar(500) NULL,
        AllegatoContentType nvarchar(100) NULL,
        AllegatoEstensione nvarchar(20) NULL,
        NotaGestione nvarchar(1000) NULL,
        GestitoDaUtenteId int NULL,
        GestitoAt datetime2 NULL,
        CreatedAt datetime2 NOT NULL CONSTRAINT DF_ExpenseRequests_CreatedAt DEFAULT (sysutcdatetime()),
        UpdatedAt datetime2 NULL
    );
END;

IF OBJECT_ID(N'dbo.MaterialRequests', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.MaterialRequests
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_MaterialRequests PRIMARY KEY,
        DipendenteId int NOT NULL,
        MaterialeRichiesto nvarchar(200) NOT NULL,
        Quantita decimal(18,2) NOT NULL,
        Motivazione nvarchar(1000) NOT NULL,
        Priorita nvarchar(30) NOT NULL,
        DataDesiderata date NOT NULL,
        Note nvarchar(1000) NULL,
        Stato nvarchar(30) NOT NULL CONSTRAINT DF_MaterialRequests_Stato DEFAULT (N'IN_ATTESA'),
        NotaGestione nvarchar(1000) NULL,
        GestitoDaUtenteId int NULL,
        GestitoAt datetime2 NULL,
        CreatedAt datetime2 NOT NULL CONSTRAINT DF_MaterialRequests_CreatedAt DEFAULT (sysutcdatetime()),
        UpdatedAt datetime2 NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_ExpenseRequests_Dipendenti' AND parent_object_id = OBJECT_ID(N'dbo.ExpenseRequests'))
BEGIN
    ALTER TABLE dbo.ExpenseRequests WITH CHECK ADD CONSTRAINT FK_ExpenseRequests_Dipendenti
        FOREIGN KEY (DipendenteId) REFERENCES dbo.Dipendenti(Id);
END;

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_ExpenseRequests_Utenti_GestitoDa' AND parent_object_id = OBJECT_ID(N'dbo.ExpenseRequests'))
BEGIN
    ALTER TABLE dbo.ExpenseRequests WITH CHECK ADD CONSTRAINT FK_ExpenseRequests_Utenti_GestitoDa
        FOREIGN KEY (GestitoDaUtenteId) REFERENCES dbo.Utenti(Id);
END;

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_MaterialRequests_Dipendenti' AND parent_object_id = OBJECT_ID(N'dbo.MaterialRequests'))
BEGIN
    ALTER TABLE dbo.MaterialRequests WITH CHECK ADD CONSTRAINT FK_MaterialRequests_Dipendenti
        FOREIGN KEY (DipendenteId) REFERENCES dbo.Dipendenti(Id);
END;

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_MaterialRequests_Utenti_GestitoDa' AND parent_object_id = OBJECT_ID(N'dbo.MaterialRequests'))
BEGIN
    ALTER TABLE dbo.MaterialRequests WITH CHECK ADD CONSTRAINT FK_MaterialRequests_Utenti_GestitoDa
        FOREIGN KEY (GestitoDaUtenteId) REFERENCES dbo.Utenti(Id);
END;

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_ExpenseRequests_Importo' AND parent_object_id = OBJECT_ID(N'dbo.ExpenseRequests'))
BEGIN
    ALTER TABLE dbo.ExpenseRequests WITH CHECK ADD CONSTRAINT CK_ExpenseRequests_Importo
        CHECK (Importo > 0);
END;

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_ExpenseRequests_Stato' AND parent_object_id = OBJECT_ID(N'dbo.ExpenseRequests'))
BEGIN
    ALTER TABLE dbo.ExpenseRequests WITH CHECK ADD CONSTRAINT CK_ExpenseRequests_Stato
        CHECK (Stato IN (N'IN_ATTESA', N'APPROVATA', N'RIFIUTATA', N'IN_REVISIONE'));
END;

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_MaterialRequests_Quantita' AND parent_object_id = OBJECT_ID(N'dbo.MaterialRequests'))
BEGIN
    ALTER TABLE dbo.MaterialRequests WITH CHECK ADD CONSTRAINT CK_MaterialRequests_Quantita
        CHECK (Quantita > 0);
END;

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_MaterialRequests_Stato' AND parent_object_id = OBJECT_ID(N'dbo.MaterialRequests'))
BEGIN
    ALTER TABLE dbo.MaterialRequests WITH CHECK ADD CONSTRAINT CK_MaterialRequests_Stato
        CHECK (Stato IN (N'IN_ATTESA', N'APPROVATA', N'RIFIUTATA', N'IN_REVISIONE'));
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ExpenseRequests_DipendenteId' AND object_id = OBJECT_ID(N'dbo.ExpenseRequests'))
    CREATE INDEX IX_ExpenseRequests_DipendenteId ON dbo.ExpenseRequests(DipendenteId);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ExpenseRequests_Stato' AND object_id = OBJECT_ID(N'dbo.ExpenseRequests'))
    CREATE INDEX IX_ExpenseRequests_Stato ON dbo.ExpenseRequests(Stato);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ExpenseRequests_CreatedAt' AND object_id = OBJECT_ID(N'dbo.ExpenseRequests'))
    CREATE INDEX IX_ExpenseRequests_CreatedAt ON dbo.ExpenseRequests(CreatedAt);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ExpenseRequests_GestitoDaUtenteId' AND object_id = OBJECT_ID(N'dbo.ExpenseRequests'))
    CREATE INDEX IX_ExpenseRequests_GestitoDaUtenteId ON dbo.ExpenseRequests(GestitoDaUtenteId);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_MaterialRequests_DipendenteId' AND object_id = OBJECT_ID(N'dbo.MaterialRequests'))
    CREATE INDEX IX_MaterialRequests_DipendenteId ON dbo.MaterialRequests(DipendenteId);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_MaterialRequests_Stato' AND object_id = OBJECT_ID(N'dbo.MaterialRequests'))
    CREATE INDEX IX_MaterialRequests_Stato ON dbo.MaterialRequests(Stato);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_MaterialRequests_CreatedAt' AND object_id = OBJECT_ID(N'dbo.MaterialRequests'))
    CREATE INDEX IX_MaterialRequests_CreatedAt ON dbo.MaterialRequests(CreatedAt);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_MaterialRequests_GestitoDaUtenteId' AND object_id = OBJECT_ID(N'dbo.MaterialRequests'))
    CREATE INDEX IX_MaterialRequests_GestitoDaUtenteId ON dbo.MaterialRequests(GestitoDaUtenteId);

COMMIT TRANSACTION;

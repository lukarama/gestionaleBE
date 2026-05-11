SET XACT_ABORT ON;

BEGIN TRANSACTION;

IF OBJECT_ID(N'dbo.IssueReports', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.IssueReports
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_IssueReports PRIMARY KEY,
        DipendenteId int NOT NULL,
        Categoria nvarchar(100) NOT NULL,
        Oggetto nvarchar(200) NOT NULL,
        Luogo nvarchar(200) NULL,
        Descrizione nvarchar(1000) NOT NULL,
        Priorita nvarchar(30) NOT NULL,
        Note nvarchar(1000) NULL,
        Stato nvarchar(30) NOT NULL CONSTRAINT DF_IssueReports_Stato DEFAULT (N'IN_ATTESA'),
        NotaGestione nvarchar(1000) NULL,
        GestitoDaUtenteId int NULL,
        GestitoAt datetime2 NULL,
        CreatedAt datetime2 NOT NULL CONSTRAINT DF_IssueReports_CreatedAt DEFAULT (sysutcdatetime()),
        UpdatedAt datetime2 NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_IssueReports_Dipendenti' AND parent_object_id = OBJECT_ID(N'dbo.IssueReports'))
BEGIN
    ALTER TABLE dbo.IssueReports WITH CHECK ADD CONSTRAINT FK_IssueReports_Dipendenti
        FOREIGN KEY (DipendenteId) REFERENCES dbo.Dipendenti(Id);
END;

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_IssueReports_Utenti_GestitoDa' AND parent_object_id = OBJECT_ID(N'dbo.IssueReports'))
BEGIN
    ALTER TABLE dbo.IssueReports WITH CHECK ADD CONSTRAINT FK_IssueReports_Utenti_GestitoDa
        FOREIGN KEY (GestitoDaUtenteId) REFERENCES dbo.Utenti(Id);
END;

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_IssueReports_Stato' AND parent_object_id = OBJECT_ID(N'dbo.IssueReports'))
BEGIN
    ALTER TABLE dbo.IssueReports WITH CHECK ADD CONSTRAINT CK_IssueReports_Stato
        CHECK (Stato IN (N'IN_ATTESA', N'APPROVATA', N'RIFIUTATA', N'IN_REVISIONE'));
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_IssueReports_DipendenteId' AND object_id = OBJECT_ID(N'dbo.IssueReports'))
    CREATE INDEX IX_IssueReports_DipendenteId ON dbo.IssueReports(DipendenteId);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_IssueReports_Stato' AND object_id = OBJECT_ID(N'dbo.IssueReports'))
    CREATE INDEX IX_IssueReports_Stato ON dbo.IssueReports(Stato);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_IssueReports_CreatedAt' AND object_id = OBJECT_ID(N'dbo.IssueReports'))
    CREATE INDEX IX_IssueReports_CreatedAt ON dbo.IssueReports(CreatedAt);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_IssueReports_GestitoDaUtenteId' AND object_id = OBJECT_ID(N'dbo.IssueReports'))
    CREATE INDEX IX_IssueReports_GestitoDaUtenteId ON dbo.IssueReports(GestitoDaUtenteId);

COMMIT TRANSACTION;

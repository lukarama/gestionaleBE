SET XACT_ABORT ON;
GO

BEGIN TRANSACTION;
GO

IF OBJECT_ID(N'dbo.CartelleDocumentiDipendenti', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.CartelleDocumentiDipendenti
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_CartelleDocumentiDipendenti PRIMARY KEY,
        DipendenteId INT NOT NULL,
        ParentCartellaId INT NULL,
        Nome NVARCHAR(150) NOT NULL,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_CartelleDocumentiDipendenti_CreatedAt DEFAULT (SYSDATETIME()),
        UpdatedAt DATETIME2 NULL,
        CreatedByUtenteId INT NULL
    );
END;
GO

IF COL_LENGTH(N'dbo.CartelleDocumentiDipendenti', N'DipendenteId') IS NULL ALTER TABLE dbo.CartelleDocumentiDipendenti ADD DipendenteId INT NOT NULL;
IF COL_LENGTH(N'dbo.CartelleDocumentiDipendenti', N'ParentCartellaId') IS NULL ALTER TABLE dbo.CartelleDocumentiDipendenti ADD ParentCartellaId INT NULL;
IF COL_LENGTH(N'dbo.CartelleDocumentiDipendenti', N'Nome') IS NULL ALTER TABLE dbo.CartelleDocumentiDipendenti ADD Nome NVARCHAR(150) NOT NULL CONSTRAINT DF_CartelleDocumentiDipendenti_Nome DEFAULT (N'Altro') WITH VALUES;
IF COL_LENGTH(N'dbo.CartelleDocumentiDipendenti', N'CreatedAt') IS NULL ALTER TABLE dbo.CartelleDocumentiDipendenti ADD CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_CartelleDocumentiDipendenti_CreatedAt DEFAULT (SYSDATETIME()) WITH VALUES;
IF COL_LENGTH(N'dbo.CartelleDocumentiDipendenti', N'UpdatedAt') IS NULL ALTER TABLE dbo.CartelleDocumentiDipendenti ADD UpdatedAt DATETIME2 NULL;
IF COL_LENGTH(N'dbo.CartelleDocumentiDipendenti', N'CreatedByUtenteId') IS NULL ALTER TABLE dbo.CartelleDocumentiDipendenti ADD CreatedByUtenteId INT NULL;
GO

IF COL_LENGTH(N'dbo.DocumentiDipendenti', N'CartellaId') IS NULL ALTER TABLE dbo.DocumentiDipendenti ADD CartellaId INT NULL;
IF COL_LENGTH(N'dbo.DocumentiDipendenti', N'NomeFileSalvato') IS NULL ALTER TABLE dbo.DocumentiDipendenti ADD NomeFileSalvato NVARCHAR(255) NULL;
IF COL_LENGTH(N'dbo.DocumentiDipendenti', N'DimensioneBytes') IS NULL ALTER TABLE dbo.DocumentiDipendenti ADD DimensioneBytes BIGINT NOT NULL CONSTRAINT DF_DocumentiDipendenti_DimensioneBytes DEFAULT (0) WITH VALUES;
IF COL_LENGTH(N'dbo.DocumentiDipendenti', N'UploadedByUtenteId') IS NULL ALTER TABLE dbo.DocumentiDipendenti ADD UploadedByUtenteId INT NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.CartelleDocumentiDipendenti') AND name = N'IX_CartelleDocumentiDipendenti_DipendenteId')
    CREATE INDEX IX_CartelleDocumentiDipendenti_DipendenteId ON dbo.CartelleDocumentiDipendenti (DipendenteId);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.CartelleDocumentiDipendenti') AND name = N'IX_CartelleDocumentiDipendenti_ParentCartellaId')
    CREATE INDEX IX_CartelleDocumentiDipendenti_ParentCartellaId ON dbo.CartelleDocumentiDipendenti (ParentCartellaId);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.CartelleDocumentiDipendenti') AND name = N'IX_CartelleDocumentiDipendenti_CreatedByUtenteId')
    CREATE INDEX IX_CartelleDocumentiDipendenti_CreatedByUtenteId ON dbo.CartelleDocumentiDipendenti (CreatedByUtenteId);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.CartelleDocumentiDipendenti') AND name = N'UX_CartelleDocumentiDipendenti_Dipendente_Parent_Nome')
    CREATE UNIQUE INDEX UX_CartelleDocumentiDipendenti_Dipendente_Parent_Nome ON dbo.CartelleDocumentiDipendenti (DipendenteId, ParentCartellaId, Nome) WHERE ParentCartellaId IS NOT NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.CartelleDocumentiDipendenti') AND name = N'UX_CartelleDocumentiDipendenti_Dipendente_Nome_Root')
    CREATE UNIQUE INDEX UX_CartelleDocumentiDipendenti_Dipendente_Nome_Root ON dbo.CartelleDocumentiDipendenti (DipendenteId, Nome) WHERE ParentCartellaId IS NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.DocumentiDipendenti') AND name = N'IX_DocumentiDipendenti_CartellaId')
    CREATE INDEX IX_DocumentiDipendenti_CartellaId ON dbo.DocumentiDipendenti (CartellaId);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.DocumentiDipendenti') AND name = N'IX_DocumentiDipendenti_UploadedByUtenteId')
    CREATE INDEX IX_DocumentiDipendenti_UploadedByUtenteId ON dbo.DocumentiDipendenti (UploadedByUtenteId);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_CartelleDocumentiDipendenti_Dipendenti')
    ALTER TABLE dbo.CartelleDocumentiDipendenti ADD CONSTRAINT FK_CartelleDocumentiDipendenti_Dipendenti FOREIGN KEY (DipendenteId) REFERENCES dbo.Dipendenti(Id);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_CartelleDocumentiDipendenti_Parent')
    ALTER TABLE dbo.CartelleDocumentiDipendenti ADD CONSTRAINT FK_CartelleDocumentiDipendenti_Parent FOREIGN KEY (ParentCartellaId) REFERENCES dbo.CartelleDocumentiDipendenti(Id);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_CartelleDocumentiDipendenti_Utenti_CreatedBy')
    ALTER TABLE dbo.CartelleDocumentiDipendenti ADD CONSTRAINT FK_CartelleDocumentiDipendenti_Utenti_CreatedBy FOREIGN KEY (CreatedByUtenteId) REFERENCES dbo.Utenti(Id);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_DocumentiDipendenti_Cartelle')
    ALTER TABLE dbo.DocumentiDipendenti ADD CONSTRAINT FK_DocumentiDipendenti_Cartelle FOREIGN KEY (CartellaId) REFERENCES dbo.CartelleDocumentiDipendenti(Id);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_DocumentiDipendenti_Utenti_UploadedBy')
    ALTER TABLE dbo.DocumentiDipendenti ADD CONSTRAINT FK_DocumentiDipendenti_Utenti_UploadedBy FOREIGN KEY (UploadedByUtenteId) REFERENCES dbo.Utenti(Id);
GO

DECLARE @CartelleStandard TABLE (Nome NVARCHAR(150) NOT NULL);
INSERT INTO @CartelleStandard (Nome)
VALUES (N'Cedolini'), (N'Contratti'), (N'CU'), (N'Documenti personali'), (N'Altro');

INSERT INTO dbo.CartelleDocumentiDipendenti (DipendenteId, Nome)
SELECT d.Id, c.Nome
FROM dbo.Dipendenti d
CROSS JOIN @CartelleStandard c
WHERE d.Attivo = 1
  AND NOT EXISTS
  (
      SELECT 1
      FROM dbo.CartelleDocumentiDipendenti existing
      WHERE existing.DipendenteId = d.Id
        AND existing.ParentCartellaId IS NULL
        AND existing.Nome = c.Nome
  );
GO

MERGE dbo.Permessi AS target
USING
(
    VALUES
        (N'documenti_dipendenti.manage.all', N'documenti_dipendenti', N'manage_all', N'Gestione documenti di tutti i dipendenti')
) AS source (Codice, Risorsa, Azione, Descrizione)
ON target.Codice = source.Codice
WHEN MATCHED THEN
    UPDATE SET target.Attivo = 1, target.Descrizione = source.Descrizione
WHEN NOT MATCHED THEN
    INSERT (Codice, Risorsa, Azione, Descrizione, Attivo)
    VALUES (source.Codice, source.Risorsa, source.Azione, source.Descrizione, 1);
GO

INSERT INTO dbo.RuoliPermessi (RuoloId, PermessoId)
SELECT r.Id, p.Id
FROM dbo.Ruoli r
JOIN dbo.Permessi p ON p.Codice = N'documenti_dipendenti.manage.all'
WHERE r.Nome IN (N'ADMIN', N'SEGRETERIA')
  AND NOT EXISTS
  (
      SELECT 1
      FROM dbo.RuoliPermessi rp
      WHERE rp.RuoloId = r.Id AND rp.PermessoId = p.Id
  );
GO

COMMIT TRANSACTION;
GO

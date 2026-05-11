SET XACT_ABORT ON;
GO

BEGIN TRANSACTION;
GO

IF OBJECT_ID(N'dbo.Ruoli', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Ruoli
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Ruoli PRIMARY KEY,
        Nome NVARCHAR(50) NOT NULL,
        Descrizione NVARCHAR(255) NULL,
        Attivo BIT NOT NULL CONSTRAINT DF_Ruoli_Attivo DEFAULT (1),
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Ruoli_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedAt DATETIME2 NULL
    );
END;
GO

IF OBJECT_ID(N'dbo.Utenti', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Utenti
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Utenti PRIMARY KEY,
        DipendenteId INT NULL,
        Username NVARCHAR(100) NOT NULL,
        Email NVARCHAR(255) NULL,
        PasswordHash NVARCHAR(500) NOT NULL,
        Nome NVARCHAR(100) NULL,
        Cognome NVARCHAR(100) NULL,
        Attivo BIT NOT NULL CONSTRAINT DF_Utenti_Attivo DEFAULT (1),
        MustChangePassword BIT NOT NULL CONSTRAINT DF_Utenti_MustChangePassword DEFAULT (1),
        UltimoAccessoAt DATETIME2 NULL,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Utenti_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedAt DATETIME2 NULL
    );
END;
GO

IF OBJECT_ID(N'dbo.UtentiRuoli', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.UtentiRuoli
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_UtentiRuoli PRIMARY KEY,
        UtenteId INT NOT NULL,
        RuoloId INT NOT NULL,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_UtentiRuoli_CreatedAt DEFAULT (SYSUTCDATETIME())
    );
END;
GO

IF OBJECT_ID(N'dbo.UtentiVisibilita', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.UtentiVisibilita
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_UtentiVisibilita PRIMARY KEY,
        UtenteId INT NOT NULL,
        Chiave NVARCHAR(50) NOT NULL,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_UtentiVisibilita_CreatedAt DEFAULT (SYSUTCDATETIME())
    );
END;
GO

IF COL_LENGTH(N'dbo.Ruoli', N'Descrizione') IS NULL ALTER TABLE dbo.Ruoli ADD Descrizione NVARCHAR(255) NULL;
IF COL_LENGTH(N'dbo.Ruoli', N'Attivo') IS NULL ALTER TABLE dbo.Ruoli ADD Attivo BIT NOT NULL CONSTRAINT DF_Ruoli_Attivo DEFAULT (1) WITH VALUES;
IF COL_LENGTH(N'dbo.Ruoli', N'CreatedAt') IS NULL ALTER TABLE dbo.Ruoli ADD CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Ruoli_CreatedAt DEFAULT (SYSUTCDATETIME()) WITH VALUES;
IF COL_LENGTH(N'dbo.Ruoli', N'UpdatedAt') IS NULL ALTER TABLE dbo.Ruoli ADD UpdatedAt DATETIME2 NULL;
GO

IF COL_LENGTH(N'dbo.Utenti', N'DipendenteId') IS NULL ALTER TABLE dbo.Utenti ADD DipendenteId INT NULL;
IF COL_LENGTH(N'dbo.Utenti', N'Email') IS NULL ALTER TABLE dbo.Utenti ADD Email NVARCHAR(255) NULL;
IF COL_LENGTH(N'dbo.Utenti', N'Nome') IS NULL ALTER TABLE dbo.Utenti ADD Nome NVARCHAR(100) NULL;
IF COL_LENGTH(N'dbo.Utenti', N'Cognome') IS NULL ALTER TABLE dbo.Utenti ADD Cognome NVARCHAR(100) NULL;
IF COL_LENGTH(N'dbo.Utenti', N'MustChangePassword') IS NULL ALTER TABLE dbo.Utenti ADD MustChangePassword BIT NOT NULL CONSTRAINT DF_Utenti_MustChangePassword DEFAULT (1) WITH VALUES;
IF COL_LENGTH(N'dbo.Utenti', N'UltimoAccessoAt') IS NULL ALTER TABLE dbo.Utenti ADD UltimoAccessoAt DATETIME2 NULL;
IF COL_LENGTH(N'dbo.Utenti', N'CreatedAt') IS NULL ALTER TABLE dbo.Utenti ADD CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Utenti_CreatedAt DEFAULT (SYSUTCDATETIME()) WITH VALUES;
IF COL_LENGTH(N'dbo.Utenti', N'UpdatedAt') IS NULL ALTER TABLE dbo.Utenti ADD UpdatedAt DATETIME2 NULL;
GO

IF EXISTS
(
    SELECT 1
    FROM sys.columns c
    JOIN sys.index_columns ic ON ic.object_id = c.object_id AND ic.column_id = c.column_id
    JOIN sys.indexes i ON i.object_id = ic.object_id AND i.index_id = ic.index_id
    WHERE c.object_id = OBJECT_ID(N'dbo.Utenti')
      AND c.name = N'Email'
      AND c.is_nullable = 0
      AND i.name = N'UX_Utenti_Email'
)
BEGIN
    DROP INDEX UX_Utenti_Email ON dbo.Utenti;
END;
GO

IF EXISTS
(
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.Utenti')
      AND name IN (N'Email', N'Nome', N'Cognome')
      AND is_nullable = 0
)
BEGIN
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Utenti') AND name = N'Email' AND is_nullable = 0)
        ALTER TABLE dbo.Utenti ALTER COLUMN Email NVARCHAR(255) NULL;

    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Utenti') AND name = N'Nome' AND is_nullable = 0)
        ALTER TABLE dbo.Utenti ALTER COLUMN Nome NVARCHAR(100) NULL;

    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Utenti') AND name = N'Cognome' AND is_nullable = 0)
        ALTER TABLE dbo.Utenti ALTER COLUMN Cognome NVARCHAR(100) NULL;
END;
GO

IF COL_LENGTH(N'dbo.UtentiRuoli', N'Id') IS NULL
BEGIN
    ALTER TABLE dbo.UtentiRuoli ADD Id INT IDENTITY(1,1) NOT NULL;
END;
GO

DECLARE @UtentiRuoliPkName SYSNAME;
DECLARE @UtentiRuoliPkUsesId BIT;

SELECT @UtentiRuoliPkName = kc.name,
       @UtentiRuoliPkUsesId = CASE WHEN SUM(CASE WHEN c.name = N'Id' THEN 1 ELSE 0 END) = 1 AND COUNT(*) = 1 THEN 1 ELSE 0 END
FROM sys.key_constraints kc
JOIN sys.index_columns ic ON ic.object_id = kc.parent_object_id AND ic.index_id = kc.unique_index_id
JOIN sys.columns c ON c.object_id = ic.object_id AND c.column_id = ic.column_id
WHERE kc.parent_object_id = OBJECT_ID(N'dbo.UtentiRuoli')
  AND kc.type = N'PK'
GROUP BY kc.name;

IF @UtentiRuoliPkName IS NOT NULL AND ISNULL(@UtentiRuoliPkUsesId, 0) = 0
BEGIN
    EXEC(N'ALTER TABLE dbo.UtentiRuoli DROP CONSTRAINT ' + QUOTENAME(@UtentiRuoliPkName));
END;

IF NOT EXISTS
(
    SELECT 1
    FROM sys.key_constraints
    WHERE parent_object_id = OBJECT_ID(N'dbo.UtentiRuoli')
      AND type = N'PK'
)
BEGIN
    ALTER TABLE dbo.UtentiRuoli ADD CONSTRAINT PK_UtentiRuoli PRIMARY KEY (Id);
END;
GO

IF COL_LENGTH(N'dbo.UtentiVisibilita', N'Id') IS NULL ALTER TABLE dbo.UtentiVisibilita ADD Id INT IDENTITY(1,1) NOT NULL;
IF COL_LENGTH(N'dbo.UtentiVisibilita', N'UtenteId') IS NULL ALTER TABLE dbo.UtentiVisibilita ADD UtenteId INT NOT NULL;
IF COL_LENGTH(N'dbo.UtentiVisibilita', N'Chiave') IS NULL ALTER TABLE dbo.UtentiVisibilita ADD Chiave NVARCHAR(50) NOT NULL;
IF COL_LENGTH(N'dbo.UtentiVisibilita', N'CreatedAt') IS NULL ALTER TABLE dbo.UtentiVisibilita ADD CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_UtentiVisibilita_CreatedAt DEFAULT (SYSUTCDATETIME()) WITH VALUES;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.Ruoli') AND name = N'UX_Ruoli_Nome')
    CREATE UNIQUE INDEX UX_Ruoli_Nome ON dbo.Ruoli (Nome);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.Utenti') AND name = N'UX_Utenti_Username')
    CREATE UNIQUE INDEX UX_Utenti_Username ON dbo.Utenti (Username);
GO

IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.Utenti') AND name = N'UX_Utenti_Email' AND filter_definition IS NULL)
    DROP INDEX UX_Utenti_Email ON dbo.Utenti;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.Utenti') AND name = N'UX_Utenti_Email')
    CREATE UNIQUE INDEX UX_Utenti_Email ON dbo.Utenti (Email) WHERE Email IS NOT NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.Utenti') AND name = N'UX_Utenti_DipendenteId')
    CREATE UNIQUE INDEX UX_Utenti_DipendenteId ON dbo.Utenti (DipendenteId) WHERE DipendenteId IS NOT NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.UtentiRuoli') AND name = N'UX_UtentiRuoli_UtenteId_RuoloId')
    CREATE UNIQUE INDEX UX_UtentiRuoli_UtenteId_RuoloId ON dbo.UtentiRuoli (UtenteId, RuoloId);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.UtentiVisibilita') AND name = N'UX_UtentiVisibilita_UtenteId_Chiave')
    CREATE UNIQUE INDEX UX_UtentiVisibilita_UtenteId_Chiave ON dbo.UtentiVisibilita (UtenteId, Chiave);
GO

IF OBJECT_ID(N'dbo.Dipendenti', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Utenti_Dipendenti')
BEGIN
    ALTER TABLE dbo.Utenti
    ADD CONSTRAINT FK_Utenti_Dipendenti FOREIGN KEY (DipendenteId) REFERENCES dbo.Dipendenti(Id);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_UtentiRuoli_Utenti')
BEGIN
    ALTER TABLE dbo.UtentiRuoli
    ADD CONSTRAINT FK_UtentiRuoli_Utenti FOREIGN KEY (UtenteId) REFERENCES dbo.Utenti(Id);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_UtentiRuoli_Ruoli')
BEGIN
    ALTER TABLE dbo.UtentiRuoli
    ADD CONSTRAINT FK_UtentiRuoli_Ruoli FOREIGN KEY (RuoloId) REFERENCES dbo.Ruoli(Id);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_UtentiVisibilita_Utenti')
BEGIN
    ALTER TABLE dbo.UtentiVisibilita
    ADD CONSTRAINT FK_UtentiVisibilita_Utenti FOREIGN KEY (UtenteId) REFERENCES dbo.Utenti(Id);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_UtentiVisibilita_Chiave')
BEGIN
    ALTER TABLE dbo.UtentiVisibilita
    ADD CONSTRAINT CK_UtentiVisibilita_Chiave
    CHECK (Chiave IN (N'dashboard', N'dipendenti', N'magazzino', N'attrezzature', N'dpi', N'mezzi', N'cantieri', N'segreteria', N'ferie-permessi', N'richieste-aziendali'));
END;
GO

MERGE dbo.Ruoli AS target
USING
(
    VALUES
        (N'ADMIN', N'Accesso completo al gestionale'),
        (N'RESPONSABILE', N'Può approvare, rifiutare o mettere in revisione le richieste aziendali'),
        (N'MAGAZZINIERE', N'Accesso all''area magazzino'),
        (N'DIPENDENTE', N'Accesso dipendente'),
        (N'SEGRETERIA', N'Accesso segreteria')
) AS source (Nome, Descrizione)
ON UPPER(target.Nome) = source.Nome
WHEN MATCHED THEN
    UPDATE SET
        target.Nome = source.Nome,
        target.Attivo = 1,
        target.UpdatedAt = SYSUTCDATETIME()
WHEN NOT MATCHED THEN
    INSERT (Nome, Descrizione, Attivo)
    VALUES (source.Nome, source.Descrizione, 1);
GO

COMMIT TRANSACTION;
GO

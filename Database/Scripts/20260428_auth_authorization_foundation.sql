ALTER TABLE Utenti
ADD DipendenteId INT NULL,
    MustChangePassword BIT NOT NULL CONSTRAINT DF_Utenti_MustChangePassword DEFAULT (1);
GO

ALTER TABLE Utenti
ADD CONSTRAINT FK_Utenti_Dipendenti
FOREIGN KEY (DipendenteId) REFERENCES Dipendenti(Id);
GO

CREATE UNIQUE INDEX UX_Utenti_DipendenteId
ON Utenti (DipendenteId)
WHERE DipendenteId IS NOT NULL;
GO

CREATE TABLE Permessi
(
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Codice NVARCHAR(150) NOT NULL,
    Risorsa NVARCHAR(100) NOT NULL,
    Azione NVARCHAR(100) NOT NULL,
    Descrizione NVARCHAR(255) NULL,
    Attivo BIT NOT NULL CONSTRAINT DF_Permessi_Attivo DEFAULT (1),
    CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Permessi_CreatedAt DEFAULT (SYSDATETIME()),
    UpdatedAt DATETIME2 NULL
);
GO

CREATE UNIQUE INDEX UX_Permessi_Codice ON Permessi (Codice);
GO

CREATE TABLE RuoliPermessi
(
    RuoloId INT NOT NULL,
    PermessoId INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_RuoliPermessi_CreatedAt DEFAULT (SYSDATETIME()),
    CONSTRAINT PK_RuoliPermessi PRIMARY KEY (RuoloId, PermessoId),
    CONSTRAINT FK_RuoliPermessi_Ruoli FOREIGN KEY (RuoloId) REFERENCES Ruoli(Id),
    CONSTRAINT FK_RuoliPermessi_Permessi FOREIGN KEY (PermessoId) REFERENCES Permessi(Id)
);
GO

CREATE TABLE RefreshTokens
(
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    UtenteId INT NOT NULL,
    TokenHash NVARCHAR(500) NOT NULL,
    ExpiresAt DATETIME2 NOT NULL,
    CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_RefreshTokens_CreatedAt DEFAULT (SYSDATETIME()),
    RevokedAt DATETIME2 NULL,
    CreatedByIp NVARCHAR(100) NULL,
    UserAgent NVARCHAR(500) NULL,
    CONSTRAINT FK_RefreshTokens_Utenti FOREIGN KEY (UtenteId) REFERENCES Utenti(Id)
);
GO

CREATE UNIQUE INDEX UX_RefreshTokens_TokenHash ON RefreshTokens (TokenHash);
GO

CREATE INDEX IX_RefreshTokens_UtenteId_ExpiresAt ON RefreshTokens (UtenteId, ExpiresAt);
GO

IF NOT EXISTS (SELECT 1 FROM Ruoli WHERE Nome = 'ADMIN')
BEGIN
    INSERT INTO Ruoli (Nome, Descrizione, Attivo)
    VALUES ('ADMIN', 'Accesso completo al gestionale', 1);
END
GO

IF NOT EXISTS (SELECT 1 FROM Ruoli WHERE Nome = 'MAGAZZINIERE')
BEGIN
    INSERT INTO Ruoli (Nome, Descrizione, Attivo)
    VALUES ('MAGAZZINIERE', 'Accesso all''area magazzino e movimenti materiali', 1);
END
GO

IF NOT EXISTS (SELECT 1 FROM Ruoli WHERE Nome = 'DIPENDENTE')
BEGIN
    INSERT INTO Ruoli (Nome, Descrizione, Attivo)
    VALUES ('DIPENDENTE', 'Accesso limitato ai propri dati personali', 1);
END
GO

IF NOT EXISTS (SELECT 1 FROM Ruoli WHERE Nome = 'SEGRETERIA')
BEGIN
    INSERT INTO Ruoli (Nome, Descrizione, Attivo)
    VALUES ('SEGRETERIA', 'Accesso alle aree anagrafiche e amministrative', 1);
END
GO

MERGE Permessi AS target
USING
(
    VALUES
        ('dipendenti.read.all', 'dipendenti', 'read_all', 'Lettura di tutti i dipendenti'),
        ('dipendenti.read.self', 'dipendenti', 'read_self', 'Lettura del proprio profilo dipendente'),
        ('dipendenti.create', 'dipendenti', 'create', 'Creazione dipendenti'),
        ('dipendenti.update.all', 'dipendenti', 'update_all', 'Modifica di tutti i dipendenti'),
        ('dipendenti.delete', 'dipendenti', 'delete', 'Disattivazione o eliminazione dipendenti'),
        ('movimenti_materiale.read', 'movimenti_materiale', 'read', 'Lettura movimenti materiali'),
        ('movimenti_materiale.create', 'movimenti_materiale', 'create', 'Creazione movimenti materiali'),
        ('movimenti_materiale.update', 'movimenti_materiale', 'update', 'Modifica movimenti materiali'),
        ('movimenti_materiale.delete', 'movimenti_materiale', 'delete', 'Eliminazione movimenti materiali'),
        ('magazzino.read', 'magazzino', 'read', 'Accesso dashboard magazzino'),
        ('documenti_dipendenti.read.all', 'documenti_dipendenti', 'read_all', 'Lettura documenti di tutti i dipendenti'),
        ('documenti_dipendenti.read.self', 'documenti_dipendenti', 'read_self', 'Lettura dei propri documenti')
) AS source (Codice, Risorsa, Azione, Descrizione)
ON target.Codice = source.Codice
WHEN NOT MATCHED THEN
    INSERT (Codice, Risorsa, Azione, Descrizione, Attivo)
    VALUES (source.Codice, source.Risorsa, source.Azione, source.Descrizione, 1);
GO

INSERT INTO RuoliPermessi (RuoloId, PermessoId)
SELECT r.Id, p.Id
FROM Ruoli r
JOIN Permessi p ON
    (r.Nome = 'ADMIN')
    OR (r.Nome = 'MAGAZZINIERE' AND p.Codice IN ('movimenti_materiale.read', 'movimenti_materiale.create', 'movimenti_materiale.update', 'magazzino.read'))
    OR (r.Nome = 'DIPENDENTE' AND p.Codice IN ('dipendenti.read.self', 'documenti_dipendenti.read.self'))
    OR (r.Nome = 'SEGRETERIA' AND p.Codice IN ('dipendenti.read.all', 'dipendenti.create', 'dipendenti.update.all', 'documenti_dipendenti.read.all'))
WHERE NOT EXISTS
(
    SELECT 1
    FROM RuoliPermessi rp
    WHERE rp.RuoloId = r.Id
      AND rp.PermessoId = p.Id
);
GO

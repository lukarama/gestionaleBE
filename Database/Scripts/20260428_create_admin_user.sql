/*
    Script per creare un utente amministratore con accesso completo.

    Credenziali iniziali:
    - Username: admin
    - Email: admin@azienda.local
    - Password temporanea: impostarla fuori dal repository

    Dopo il primo accesso conviene:
    1. cambiare la password
    2. impostare una mail reale
    3. se necessario collegare il record a un DipendenteId
*/

SET NOCOUNT ON;
GO

DECLARE @Username NVARCHAR(100) = N'admin';
DECLARE @Email NVARCHAR(255) = N'admin@azienda.local';
DECLARE @PasswordHash NVARCHAR(500) = N'pbkdf2-sha256.100000.zB+K6Qks+aIctlm7SVY4Qw==.nFmqG5smlbHgMJHg41GDWSk99XT2sAXPLEainPxTn/U=';
DECLARE @Nome NVARCHAR(100) = N'Admin';
DECLARE @Cognome NVARCHAR(100) = N'Sistema';
DECLARE @DipendenteId INT = NULL;

DECLARE @RuoloAdminId INT;
DECLARE @UtenteId INT;

SELECT @RuoloAdminId = Id
FROM Ruoli
WHERE Nome = N'ADMIN'
  AND Attivo = 1;

IF @RuoloAdminId IS NULL
BEGIN
    THROW 50001, 'Ruolo ADMIN non trovato. Eseguire prima lo script di foundation auth.', 1;
END;

SELECT @UtenteId = Id
FROM Utenti
WHERE Username = @Username
   OR Email = @Email;

IF @UtenteId IS NULL
BEGIN
    INSERT INTO Utenti
    (
        Username,
        Email,
        PasswordHash,
        Nome,
        Cognome,
        DipendenteId,
        Attivo,
        MustChangePassword,
        UltimoAccessoAt,
        CreatedAt,
        UpdatedAt
    )
    VALUES
    (
        @Username,
        @Email,
        @PasswordHash,
        @Nome,
        @Cognome,
        @DipendenteId,
        1,
        1,
        NULL,
        SYSDATETIME(),
        NULL
    );

    SET @UtenteId = SCOPE_IDENTITY();
END;

IF NOT EXISTS
(
    SELECT 1
    FROM UtentiRuoli
    WHERE UtenteId = @UtenteId
      AND RuoloId = @RuoloAdminId
)
BEGIN
    INSERT INTO UtentiRuoli
    (
        UtenteId,
        RuoloId,
        CreatedAt
    )
    VALUES
    (
        @UtenteId,
        @RuoloAdminId,
        SYSDATETIME()
    );
END;

SELECT
    u.Id,
    u.Username,
    u.Email,
    u.Nome,
    u.Cognome,
    u.Attivo,
    u.MustChangePassword,
    r.Nome AS Ruolo
FROM Utenti u
JOIN UtentiRuoli ur ON ur.UtenteId = u.Id
JOIN Ruoli r ON r.Id = ur.RuoloId
WHERE u.Id = @UtenteId;
GO

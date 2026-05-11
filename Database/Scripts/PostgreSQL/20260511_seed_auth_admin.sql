INSERT INTO "Ruoli" ("Nome", "Descrizione", "Attivo")
VALUES
    ('ADMIN', 'Accesso completo al gestionale', TRUE),
    ('RESPONSABILE', 'Accesso alle aree operative assegnate', TRUE),
    ('MAGAZZINIERE', 'Accesso all''area magazzino e movimenti materiali', TRUE),
    ('DIPENDENTE', 'Accesso limitato ai propri dati personali', TRUE),
    ('SEGRETERIA', 'Accesso alle aree anagrafiche e amministrative', TRUE)
ON CONFLICT ("Nome") DO UPDATE
SET
    "Descrizione" = EXCLUDED."Descrizione",
    "Attivo" = TRUE,
    "UpdatedAt" = CURRENT_TIMESTAMP;

INSERT INTO "Permessi" ("Codice", "Risorsa", "Azione", "Descrizione", "Attivo")
VALUES
    ('dipendenti.read.all', 'dipendenti', 'read_all', 'Lettura di tutti i dipendenti', TRUE),
    ('dipendenti.read.self', 'dipendenti', 'read_self', 'Lettura del proprio profilo dipendente', TRUE),
    ('dipendenti.create', 'dipendenti', 'create', 'Creazione dipendenti', TRUE),
    ('dipendenti.update.all', 'dipendenti', 'update_all', 'Modifica di tutti i dipendenti', TRUE),
    ('dipendenti.delete', 'dipendenti', 'delete', 'Disattivazione o eliminazione dipendenti', TRUE),
    ('movimenti_materiale.read', 'movimenti_materiale', 'read', 'Lettura movimenti materiali', TRUE),
    ('movimenti_materiale.create', 'movimenti_materiale', 'create', 'Creazione movimenti materiali', TRUE),
    ('movimenti_materiale.update', 'movimenti_materiale', 'update', 'Modifica movimenti materiali', TRUE),
    ('movimenti_materiale.delete', 'movimenti_materiale', 'delete', 'Eliminazione movimenti materiali', TRUE),
    ('magazzino.read', 'magazzino', 'read', 'Accesso dashboard magazzino', TRUE),
    ('documenti_dipendenti.read.all', 'documenti_dipendenti', 'read_all', 'Lettura documenti di tutti i dipendenti', TRUE),
    ('documenti_dipendenti.read.self', 'documenti_dipendenti', 'read_self', 'Lettura dei propri documenti', TRUE),
    ('documenti_dipendenti.manage.all', 'documenti_dipendenti', 'manage_all', 'Gestione documenti di tutti i dipendenti', TRUE)
ON CONFLICT ("Codice") DO UPDATE
SET
    "Risorsa" = EXCLUDED."Risorsa",
    "Azione" = EXCLUDED."Azione",
    "Descrizione" = EXCLUDED."Descrizione",
    "Attivo" = TRUE,
    "UpdatedAt" = CURRENT_TIMESTAMP;

INSERT INTO "RuoliPermessi" ("RuoloId", "PermessoId")
SELECT r."Id", p."Id"
FROM "Ruoli" r
JOIN "Permessi" p ON
    r."Nome" = 'ADMIN'
    OR (r."Nome" = 'MAGAZZINIERE' AND p."Codice" IN ('movimenti_materiale.read', 'movimenti_materiale.create', 'movimenti_materiale.update', 'magazzino.read'))
    OR (r."Nome" = 'DIPENDENTE' AND p."Codice" IN ('dipendenti.read.self', 'documenti_dipendenti.read.self'))
    OR (r."Nome" = 'SEGRETERIA' AND p."Codice" IN ('dipendenti.read.all', 'dipendenti.create', 'dipendenti.update.all', 'documenti_dipendenti.read.all', 'documenti_dipendenti.manage.all'))
    OR (r."Nome" = 'RESPONSABILE' AND p."Codice" IN ('dipendenti.read.all', 'movimenti_materiale.read', 'magazzino.read', 'documenti_dipendenti.read.all'))
ON CONFLICT ("RuoloId", "PermessoId") DO NOTHING;

WITH upsert_admin AS (
    INSERT INTO "Utenti"
    (
        "Username",
        "Email",
        "PasswordHash",
        "Nome",
        "Cognome",
        "DipendenteId",
        "Attivo",
        "MustChangePassword",
        "UltimoAccessoAt",
        "CreatedAt",
        "UpdatedAt"
    )
    VALUES
    (
        'admin',
        'admin@azienda.local',
        'pbkdf2-sha256.100000.zB+K6Qks+aIctlm7SVY4Qw==.nFmqG5smlbHgMJHg41GDWSk99XT2sAXPLEainPxTn/U=',
        'Admin',
        'Sistema',
        NULL,
        TRUE,
        TRUE,
        NULL,
        CURRENT_TIMESTAMP,
        NULL
    )
    ON CONFLICT ("Username") DO UPDATE
    SET
        "Email" = EXCLUDED."Email",
        "Attivo" = TRUE,
        "MustChangePassword" = TRUE,
        "UpdatedAt" = CURRENT_TIMESTAMP
    RETURNING "Id"
)
INSERT INTO "UtentiRuoli" ("UtenteId", "RuoloId")
SELECT ua."Id", r."Id"
FROM upsert_admin ua
JOIN "Ruoli" r ON r."Nome" = 'ADMIN'
ON CONFLICT ("UtenteId", "RuoloId") DO NOTHING;

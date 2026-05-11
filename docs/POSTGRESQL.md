# PostgreSQL

Il backend usa PostgreSQL tramite `Npgsql.EntityFrameworkCore.PostgreSQL`.

## Configurazione locale

Impostare la connection string reale fuori dal repository, per esempio con user-secrets:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=gestionale_aziendale;Username=postgres;Password=LA_PASSWORD_LOCALE"
```

`appsettings.Development.json` contiene solo un placeholder e non va usato con password reali.

## Creazione schema

Con PostgreSQL avviato e database creato:

```powershell
dotnet ef database update
```

In alternativa si puo applicare lo script generato:

```text
Database/Scripts/PostgreSQL/20260511_initial_schema.sql
```

## Produzione

Sulla VPS impostare la connection string tramite variabile d'ambiente o configurazione server, non dentro Git:

```powershell
$env:ConnectionStrings__DefaultConnection = "Host=localhost;Port=5432;Database=gestionale_aziendale;Username=gestionale_app;Password=..."
```

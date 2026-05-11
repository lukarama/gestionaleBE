namespace Gestionale.Api.Common.Auth;

public static class PermissionCodes
{
    public const string DipendentiReadAll = "dipendenti.read.all";
    public const string DipendentiReadSelf = "dipendenti.read.self";
    public const string DipendentiCreate = "dipendenti.create";
    public const string DipendentiUpdateAll = "dipendenti.update.all";
    public const string DipendentiDelete = "dipendenti.delete";

    public const string MovimentiMaterialeRead = "movimenti_materiale.read";
    public const string MovimentiMaterialeCreate = "movimenti_materiale.create";
    public const string MovimentiMaterialeUpdate = "movimenti_materiale.update";
    public const string MovimentiMaterialeDelete = "movimenti_materiale.delete";

    public const string MagazzinoRead = "magazzino.read";
    public const string DocumentiDipendentiReadAll = "documenti_dipendenti.read.all";
    public const string DocumentiDipendentiReadSelf = "documenti_dipendenti.read.self";
    public const string DocumentiDipendentiManageAll = "documenti_dipendenti.manage.all";
}

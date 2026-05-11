namespace Gestionale.Api.Services;

public static class RequestStatusCodes
{
    public const string InAttesa = "IN_ATTESA";
    public const string Approvata = "APPROVATA";
    public const string Rifiutata = "RIFIUTATA";
    public const string InRevisione = "IN_REVISIONE";

    public static readonly HashSet<string> StatiGestione = new(StringComparer.OrdinalIgnoreCase)
    {
        Approvata,
        Rifiutata,
        InRevisione
    };

    public static bool RichiedeNota(string stato)
    {
        return string.Equals(stato, Rifiutata, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(stato, InRevisione, StringComparison.OrdinalIgnoreCase);
    }
}

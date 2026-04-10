namespace Gestionale.Api.Common
{
    public static class ServiceMessages
    {
        public const string NotFound = "Elemento non trovato.";
        public const string DeleteBlocked = "Non è possibile eliminare l'elemento perché è collegato ad altri record.";
        public const string ValidationError = "Dati non validi.";
    }
}
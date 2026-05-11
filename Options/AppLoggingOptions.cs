namespace Gestionale.Api.Options;

public class AppLoggingOptions
{
    public const string SectionName = "AppLogging";

    public string Directory { get; set; } = "Logs";

    public int RetentionDays { get; set; } = 30;

    public bool LogSuccessfulRequests { get; set; } = true;

    public int SlowRequestThresholdMs { get; set; } = 1000;
}

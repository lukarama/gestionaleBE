using System.Collections.Concurrent;
using System.Text.Json;
using Gestionale.Api.Options;
using Microsoft.Extensions.Options;

namespace Gestionale.Api.Logging;

public sealed class GdprSafeFileLoggerProvider : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, GdprSafeFileLogger> _loggers = new();
    private readonly IOptionsMonitor<AppLoggingOptions> _options;

    public GdprSafeFileLoggerProvider(IOptionsMonitor<AppLoggingOptions> options)
    {
        _options = options;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return _loggers.GetOrAdd(categoryName, name => new GdprSafeFileLogger(name, _options));
    }

    public void Dispose()
    {
        _loggers.Clear();
    }
}

public sealed class GdprSafeFileLogger : ILogger
{
    private static readonly object WriteLock = new();
    private readonly string _categoryName;
    private readonly IOptionsMonitor<AppLoggingOptions> _options;
    private DateOnly _lastCleanupDate;

    public GdprSafeFileLogger(string categoryName, IOptionsMonitor<AppLoggingOptions> options)
    {
        _categoryName = categoryName;
        _options = options;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var options = _options.CurrentValue;
        var logDirectory = Path.GetFullPath(options.Directory);
        Directory.CreateDirectory(logDirectory);
        CleanupOldLogs(logDirectory, options.RetentionDays);

        var filePath = Path.Combine(logDirectory, $"gestionale-{DateTime.UtcNow:yyyyMMdd}.jsonl");
        var entry = new
        {
            timestampUtc = DateTime.UtcNow,
            level = logLevel.ToString(),
            category = _categoryName,
            eventId = eventId.Id,
            message = formatter(state, exception),
            exception = exception == null
                ? null
                : new
                {
                    type = exception.GetType().FullName,
                    message = exception.Message
                }
        };

        var line = JsonSerializer.Serialize(entry);
        lock (WriteLock)
        {
            File.AppendAllText(filePath, line + Environment.NewLine);
        }
    }

    private void CleanupOldLogs(string logDirectory, int retentionDays)
    {
        if (retentionDays <= 0)
        {
            return;
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (_lastCleanupDate == today)
        {
            return;
        }

        _lastCleanupDate = today;
        var cutoff = DateTime.UtcNow.AddDays(-retentionDays);
        foreach (var file in Directory.EnumerateFiles(logDirectory, "gestionale-*.jsonl"))
        {
            try
            {
                if (File.GetLastWriteTimeUtc(file) < cutoff)
                {
                    File.Delete(file);
                }
            }
            catch
            {
                // Logging cleanup must never break application startup or requests.
            }
        }
    }
}

using System.Diagnostics;
using System.Security.Claims;
using Gestionale.Api.Options;
using Microsoft.Extensions.Options;

namespace Gestionale.Api.Common;

public class RequestLoggingMiddleware
{
    private static readonly HashSet<string> StaticExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".css", ".js", ".map", ".png", ".jpg", ".jpeg", ".gif", ".svg", ".ico", ".woff", ".woff2"
    };

    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    private readonly AppLoggingOptions _options;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger,
        IOptions<AppLoggingOptions> options)
    {
        _next = next;
        _logger = logger;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            LogRequest(context, stopwatch.ElapsedMilliseconds);
        }
    }

    private void LogRequest(HttpContext context, long elapsedMs)
    {
        if (IsStaticAsset(context.Request.Path))
        {
            return;
        }

        var statusCode = context.Response.StatusCode;
        var shouldLog =
            _options.LogSuccessfulRequests ||
            statusCode >= StatusCodes.Status400BadRequest ||
            elapsedMs >= _options.SlowRequestThresholdMs;

        if (!shouldLog)
        {
            return;
        }

        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var routePattern = context.GetEndpoint()?.DisplayName ?? context.Request.Path.Value ?? "-";
        var clientIp = MaskIp(context.Connection.RemoteIpAddress?.ToString());

        using (_logger.BeginScope(new Dictionary<string, object?>
        {
            ["TraceId"] = context.TraceIdentifier,
            ["UserId"] = userId,
            ["ClientIpMasked"] = clientIp
        }))
        {
            _logger.LogInformation(
                "HTTP {Method} {Route} responded {StatusCode} in {ElapsedMs} ms. TraceId={TraceId}; UserId={UserId}; ClientIpMasked={ClientIpMasked}",
                context.Request.Method,
                routePattern,
                statusCode,
                elapsedMs,
                context.TraceIdentifier,
                userId ?? "anonymous",
                clientIp ?? "unknown");
        }
    }

    private static bool IsStaticAsset(PathString path)
    {
        var extension = Path.GetExtension(path.Value);
        return !string.IsNullOrWhiteSpace(extension) && StaticExtensions.Contains(extension);
    }

    private static string? MaskIp(string? ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
        {
            return null;
        }

        if (System.Net.IPAddress.TryParse(ipAddress, out var parsed))
        {
            var bytes = parsed.GetAddressBytes();
            if (bytes.Length == 4)
            {
                bytes[3] = 0;
                return new System.Net.IPAddress(bytes).ToString();
            }

            if (bytes.Length == 16)
            {
                for (var i = 8; i < bytes.Length; i++)
                {
                    bytes[i] = 0;
                }

                return new System.Net.IPAddress(bytes).ToString();
            }
        }

        return "masked";
    }
}

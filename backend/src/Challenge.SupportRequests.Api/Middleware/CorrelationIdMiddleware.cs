using System.Diagnostics;

namespace Challenge.SupportRequests.Api.Middleware;

public sealed class CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
{
    public const string HeaderName = "X-Correlation-ID";
    public async Task InvokeAsync(HttpContext context)
    {
        var supplied = context.Request.Headers[HeaderName].ToString();
        context.TraceIdentifier = supplied.Length is > 0 and <= 100 && supplied.All(c => char.IsAsciiLetterOrDigit(c) || c is '-' or '_' or '.')
            ? supplied : Guid.NewGuid().ToString("N");
        context.Response.OnStarting(() => { context.Response.Headers[HeaderName] = context.TraceIdentifier; return Task.CompletedTask; });
        using var scope = logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = context.TraceIdentifier });
        var started = Stopwatch.GetTimestamp();
        try { await next(context); }
        finally
        {
            logger.LogInformation("HTTP {Method} {Path} returned {StatusCode} in {ElapsedMs} ms",
                context.Request.Method, context.Request.Path, context.Response.StatusCode, Stopwatch.GetElapsedTime(started).TotalMilliseconds);
        }
    }
}

namespace Cms.API.Middleware;

/// <summary>
/// After any successful content mutation (POST/PUT/PATCH/DELETE on an /api/
/// content endpoint), pings the public site's on-demand revalidation webhook so
/// the change shows up immediately instead of waiting out the ISR window.
///
/// Fire-and-forget: never blocks or fails the original request. No-ops unless
/// both Revalidate:Url and Revalidate:Secret are configured (env:
/// SITE_REVALIDATE_URL / SITE_REVALIDATE_SECRET).
/// </summary>
public class RevalidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHttpClientFactory _httpFactory;
    private readonly ILogger<RevalidationMiddleware> _logger;
    private readonly string? _url;
    private readonly string? _secret;

    // Mutations on these paths don't change public site content, so skip them.
    private static readonly string[] Ignored =
    {
        "/api/auth", "/api/messages", "/api/job-applications", "/api/analytics",
    };

    public RevalidationMiddleware(
        RequestDelegate next,
        IHttpClientFactory httpFactory,
        IConfiguration config,
        ILogger<RevalidationMiddleware> logger)
    {
        _next = next;
        _httpFactory = httpFactory;
        _logger = logger;
        _url = config["Revalidate:Url"] ?? Environment.GetEnvironmentVariable("SITE_REVALIDATE_URL");
        _secret = config["Revalidate:Secret"] ?? Environment.GetEnvironmentVariable("SITE_REVALIDATE_SECRET");
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);

        if (string.IsNullOrWhiteSpace(_url)) return;

        var method = context.Request.Method;
        if (method != HttpMethods.Post && method != HttpMethods.Put &&
            method != HttpMethods.Patch && method != HttpMethods.Delete) return;

        if (context.Response.StatusCode is < 200 or >= 300) return;

        var path = context.Request.Path.Value ?? string.Empty;
        if (!path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase)) return;
        if (Ignored.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase))) return;

        // Fire-and-forget — don't make the admin wait on the site.
        _ = TriggerAsync();
    }

    private async Task TriggerAsync()
    {
        try
        {
            var client = _httpFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(5);
            var sep = _url!.Contains('?') ? "&" : "?";
            var url = $"{_url}{sep}secret={Uri.EscapeDataString(_secret ?? string.Empty)}";
            await client.PostAsync(url, null);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "On-demand revalidation ping failed.");
        }
    }
}

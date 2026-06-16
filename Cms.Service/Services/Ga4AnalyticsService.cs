using System.Globalization;
using Cms.Service.DTOs;
using Cms.Service.Interfaces;
using Cms.Service.Settings;
using Google.Analytics.Data.V1Beta;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cms.Service.Services;

/// <summary>
/// Reads daily traffic from the GA4 Data API. Credentials resolve in this order:
/// explicit JSON in config → service-account key file → Application Default
/// Credentials (the GOOGLE_APPLICATION_CREDENTIALS env var). If GA4 is not
/// configured or the call fails, it returns sample data so the chart never breaks.
/// </summary>
public class Ga4AnalyticsService : IAnalyticsService
{
    private readonly GoogleAnalyticsSettings _settings;
    private readonly ILogger<Ga4AnalyticsService> _logger;

    public Ga4AnalyticsService(
        IOptions<GoogleAnalyticsSettings> options,
        ILogger<Ga4AnalyticsService> logger)
    {
        _settings = options.Value;
        _logger = logger;
    }

    public async Task<AnalyticsOverviewResponse> GetOverviewAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_settings.PropertyId))
        {
            _logger.LogInformation("GA4 PropertyId not configured; returning sample analytics data.");
            return SampleData();
        }

        try
        {
            GoogleCredential? credential = null;
            // FromJson/FromFile are flagged obsolete in this beta package in favour of a
            // not-yet-stable CredentialFactory API; the methods still work, so suppress here.
#pragma warning disable CS0618
            if (!string.IsNullOrWhiteSpace(_settings.CredentialsJson))
                credential = GoogleCredential.FromJson(_settings.CredentialsJson);
            else if (!string.IsNullOrWhiteSpace(_settings.CredentialsPath) && File.Exists(_settings.CredentialsPath))
                credential = GoogleCredential.FromFile(_settings.CredentialsPath);
#pragma warning restore CS0618
            // else: Application Default Credentials (the GOOGLE_APPLICATION_CREDENTIALS env var)

            var builder = new BetaAnalyticsDataClientBuilder();
            if (credential != null)
                builder.GoogleCredential = credential.CreateScoped(
                    "https://www.googleapis.com/auth/analytics.readonly");

            var client = await builder.BuildAsync(cancellationToken);

            var days = _settings.Days > 0 ? _settings.Days : 7;
            var request = new RunReportRequest
            {
                Property = $"properties/{_settings.PropertyId}",
                DateRanges = { new DateRange { StartDate = $"{days - 1}daysAgo", EndDate = "today" } },
                Dimensions = { new Dimension { Name = "date" } },
                Metrics =
                {
                    new Metric { Name = "screenPageViews" },
                    new Metric { Name = "totalUsers" },
                },
                OrderBys =
                {
                    new OrderBy { Dimension = new OrderBy.Types.DimensionOrderBy { DimensionName = "date" } },
                },
            };

            var response = await client.RunReportAsync(request, cancellationToken);

            var categories = new List<string>();
            var pageViews = new List<int>();
            var visitors = new List<int>();

            foreach (var row in response.Rows)
            {
                var raw = row.DimensionValues[0].Value; // GA4 returns dates as yyyyMMdd
                var label = DateTime.TryParseExact(
                    raw, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
                    ? date.ToString("ddd", CultureInfo.InvariantCulture)
                    : raw;

                categories.Add(label);
                pageViews.Add(ParseInt(row.MetricValues[0].Value));
                visitors.Add(ParseInt(row.MetricValues[1].Value));
            }

            return new AnalyticsOverviewResponse(
                Categories: categories.ToArray(),
                PageViews: pageViews.ToArray(),
                Visitors: visitors.ToArray(),
                TotalPageViews: pageViews.Sum(),
                TotalVisitors: visitors.Sum());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch GA4 analytics; falling back to sample data.");
            return SampleData();
        }
    }

    private static int ParseInt(string value) =>
        int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n) ? n : 0;

    private static AnalyticsOverviewResponse SampleData()
    {
        var categories = new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
        var pageViews = new[] { 320, 410, 380, 520, 610, 450, 390 };
        var visitors = new[] { 180, 230, 210, 290, 340, 260, 220 };

        return new AnalyticsOverviewResponse(
            Categories: categories,
            PageViews: pageViews,
            Visitors: visitors,
            TotalPageViews: pageViews.Sum(),
            TotalVisitors: visitors.Sum());
    }
}

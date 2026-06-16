namespace Cms.Service.Settings;

/// <summary>
/// Binds the "GoogleAnalytics" config section. When <see cref="PropertyId"/> is
/// empty the analytics service falls back to sample data, so the app still runs
/// without GA4 credentials.
/// </summary>
public class GoogleAnalyticsSettings
{
    /// <summary>Numeric GA4 property id (Admin → Property Settings), e.g. "123456789".</summary>
    public string? PropertyId { get; set; }

    /// <summary>Path to a Google service-account JSON key file.</summary>
    public string? CredentialsPath { get; set; }

    /// <summary>Raw service-account JSON (alternative to <see cref="CredentialsPath"/>).</summary>
    public string? CredentialsJson { get; set; }

    /// <summary>How many trailing days to report (default 7).</summary>
    public int Days { get; set; } = 7;
}

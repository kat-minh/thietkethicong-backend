using Cms.Service.DTOs;

namespace Cms.Service.Interfaces;

public interface IAnalyticsService
{
    /// <summary>
    /// Traffic overview for the admin dashboard charts. Pulls real data from the
    /// GA4 Data API when configured, otherwise returns sample data.
    /// </summary>
    Task<AnalyticsOverviewResponse> GetOverviewAsync(CancellationToken cancellationToken = default);
}

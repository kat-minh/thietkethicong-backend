using Cms.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cms.API.Controllers;

[ApiController]
[Route("api/analytics")]
[Authorize]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analytics;

    public AnalyticsController(IAnalyticsService analytics)
    {
        _analytics = analytics;
    }

    /// <summary>
    /// Traffic overview for the admin dashboard charts. Pulls live data from the
    /// GA4 Data API when configured (see the "GoogleAnalytics" config section),
    /// otherwise falls back to sample data.
    /// </summary>
    [HttpGet("overview")]
    public async Task<IActionResult> Overview(CancellationToken cancellationToken)
    {
        var response = await _analytics.GetOverviewAsync(cancellationToken);
        return Ok(response);
    }
}

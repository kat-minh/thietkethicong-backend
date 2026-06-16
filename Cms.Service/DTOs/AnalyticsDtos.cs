namespace Cms.Service.DTOs;

public record AnalyticsOverviewResponse(
    string[] Categories,
    int[] PageViews,
    int[] Visitors,
    int TotalPageViews,
    int TotalVisitors);

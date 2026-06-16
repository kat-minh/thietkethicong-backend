using Cms.Service.Interfaces;
using Cms.Service.Services;
using Cms.Service.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cms.Service.Extensions;

public static class DependencyInjection
{
    /// <summary>Registers business services and binds JWT settings.</summary>
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.Configure<GoogleAnalyticsSettings>(configuration.GetSection("GoogleAnalytics"));

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<IServiceItemService, ServiceItemService>();
        services.AddScoped<ISiteSettingService, SiteSettingService>();
        services.AddScoped<IContactMessageService, ContactMessageService>();
        services.AddScoped<IUserAdminService, UserAdminService>();
        services.AddScoped<IAnalyticsService, Ga4AnalyticsService>();

        return services;
    }
}

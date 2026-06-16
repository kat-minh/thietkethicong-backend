using Cms.Repository.Interfaces;
using Cms.Repository.Persistence;
using Cms.Repository.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cms.Repository.Extensions;

public static class DependencyInjection
{
    /// <summary>Registers the DbContext (PostgreSQL) and data-access services.</summary>
    public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<IServiceItemRepository, ServiceItemRepository>();
        services.AddScoped<ISiteSettingRepository, SiteSettingRepository>();
        services.AddScoped<IContactMessageRepository, ContactMessageRepository>();

        // Generic CRUD for flat content entities (testimonials, team, faq, …).
        services.AddScoped(typeof(ISimpleRepository<>), typeof(SimpleRepository<>));

        return services;
    }
}

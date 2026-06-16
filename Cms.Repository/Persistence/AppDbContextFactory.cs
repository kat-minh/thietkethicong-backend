using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Cms.Repository.Persistence;

/// <summary>
/// Design-time factory so `dotnet ef migrations add` can build the model without
/// booting the API (which would try to connect to Postgres on startup). The
/// connection string here is only used to pick the provider — no DB connection
/// is opened when scaffolding a migration.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=cms_db;Username=cms_user;Password=cms_password")
            .Options;
        return new AppDbContext(options);
    }
}

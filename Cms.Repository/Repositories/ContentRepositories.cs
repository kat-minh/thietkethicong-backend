using Cms.Repository.Entities;
using Cms.Repository.Interfaces;
using Cms.Repository.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cms.Repository.Repositories;

public class PostRepository : IPostRepository
{
    private readonly AppDbContext _db;
    public PostRepository(AppDbContext db) => _db = db;

    public Task<List<Post>> GetAllAsync() =>
        _db.Posts.AsNoTracking().OrderByDescending(p => p.UpdatedAt).ToListAsync();

    public Task<List<Post>> GetPublishedAsync() =>
        _db.Posts.AsNoTracking()
            .Where(p => p.Status == "published")
            .OrderByDescending(p => p.PublishedAt)
            .ToListAsync();

    public Task<Post?> GetBySlugAsync(string slug) =>
        _db.Posts.FirstOrDefaultAsync(p => p.Slug == slug);

    public Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null) =>
        _db.Posts.AnyAsync(p => p.Slug == slug && (excludeId == null || p.Id != excludeId));

    public Task<int> CountAsync() => _db.Posts.CountAsync();

    public async Task AddAsync(Post post) { await _db.Posts.AddAsync(post); await _db.SaveChangesAsync(); }
    public async Task UpdateAsync(Post post) { _db.Posts.Update(post); await _db.SaveChangesAsync(); }
    public async Task DeleteAsync(Post post) { _db.Posts.Remove(post); await _db.SaveChangesAsync(); }
}

public class ServiceItemRepository : IServiceItemRepository
{
    private readonly AppDbContext _db;
    public ServiceItemRepository(AppDbContext db) => _db = db;

    public Task<List<ServiceItem>> GetAllAsync() =>
        _db.Services.AsNoTracking().OrderBy(s => s.SortOrder).ThenBy(s => s.Title).ToListAsync();

    public Task<ServiceItem?> GetBySlugAsync(string slug) =>
        _db.Services.FirstOrDefaultAsync(s => s.Slug == slug);

    public Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null) =>
        _db.Services.AnyAsync(s => s.Slug == slug && (excludeId == null || s.Id != excludeId));

    public Task<int> CountAsync() => _db.Services.CountAsync();

    public async Task AddAsync(ServiceItem item) { await _db.Services.AddAsync(item); await _db.SaveChangesAsync(); }
    public async Task UpdateAsync(ServiceItem item) { _db.Services.Update(item); await _db.SaveChangesAsync(); }
    public async Task DeleteAsync(ServiceItem item) { _db.Services.Remove(item); await _db.SaveChangesAsync(); }
}

public class SiteSettingRepository : ISiteSettingRepository
{
    private readonly AppDbContext _db;
    public SiteSettingRepository(AppDbContext db) => _db = db;

    public Task<SiteSetting?> GetAsync() => _db.SiteSettings.FirstOrDefaultAsync();

    public async Task UpsertAsync(SiteSetting setting)
    {
        var existing = await _db.SiteSettings.FirstOrDefaultAsync();
        if (existing is null)
            await _db.SiteSettings.AddAsync(setting);
        else
            _db.Entry(existing).CurrentValues.SetValues(setting);

        await _db.SaveChangesAsync();
    }
}

public class ContactMessageRepository : IContactMessageRepository
{
    private readonly AppDbContext _db;
    public ContactMessageRepository(AppDbContext db) => _db = db;

    public Task<List<ContactMessage>> GetAllAsync() =>
        _db.ContactMessages.AsNoTracking().OrderByDescending(m => m.CreatedAt).ToListAsync();

    public Task<ContactMessage?> GetByIdAsync(Guid id) =>
        _db.ContactMessages.FirstOrDefaultAsync(m => m.Id == id);

    public Task<int> CountUnreadAsync() => _db.ContactMessages.CountAsync(m => !m.IsRead);

    public async Task AddAsync(ContactMessage message) { await _db.ContactMessages.AddAsync(message); await _db.SaveChangesAsync(); }
    public async Task UpdateAsync(ContactMessage message) { _db.ContactMessages.Update(message); await _db.SaveChangesAsync(); }
    public async Task DeleteAsync(ContactMessage message) { _db.ContactMessages.Remove(message); await _db.SaveChangesAsync(); }
}

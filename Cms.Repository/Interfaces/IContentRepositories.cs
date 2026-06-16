using Cms.Repository.Entities;

namespace Cms.Repository.Interfaces;

public interface IPostRepository
{
    Task<List<Post>> GetAllAsync();
    Task<List<Post>> GetPublishedAsync();
    Task<Post?> GetBySlugAsync(string slug);
    Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null);
    Task<int> CountAsync();
    Task AddAsync(Post post);
    Task UpdateAsync(Post post);
    Task DeleteAsync(Post post);
}

public interface IServiceItemRepository
{
    Task<List<ServiceItem>> GetAllAsync();
    Task<ServiceItem?> GetBySlugAsync(string slug);
    Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null);
    Task<int> CountAsync();
    Task AddAsync(ServiceItem item);
    Task UpdateAsync(ServiceItem item);
    Task DeleteAsync(ServiceItem item);
}

public interface ISiteSettingRepository
{
    Task<SiteSetting?> GetAsync();
    Task UpsertAsync(SiteSetting setting);
}

public interface IPageContentRepository
{
    Task<List<PageContent>> GetAllAsync();
    Task<int> CountAsync();
    Task AddRangeAsync(IEnumerable<PageContent> items);
    /// <summary>Update only the Value of existing keys (admin can't add/remove keys).</summary>
    Task UpdateValuesAsync(IReadOnlyDictionary<string, string> values);
}

public interface IContactMessageRepository
{
    Task<List<ContactMessage>> GetAllAsync();
    Task<ContactMessage?> GetByIdAsync(Guid id);
    Task<int> CountUnreadAsync();
    Task AddAsync(ContactMessage message);
    Task UpdateAsync(ContactMessage message);
    Task DeleteAsync(ContactMessage message);
}

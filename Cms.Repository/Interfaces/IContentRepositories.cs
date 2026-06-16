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

public interface IContactMessageRepository
{
    Task<List<ContactMessage>> GetAllAsync();
    Task<ContactMessage?> GetByIdAsync(Guid id);
    Task<int> CountUnreadAsync();
    Task AddAsync(ContactMessage message);
    Task UpdateAsync(ContactMessage message);
    Task DeleteAsync(ContactMessage message);
}

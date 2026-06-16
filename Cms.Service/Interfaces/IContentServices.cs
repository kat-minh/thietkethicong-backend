using Cms.Service.DTOs;

namespace Cms.Service.Interfaces;

public interface IPostService
{
    Task<List<PostListItem>> GetAllAsync();
    Task<List<PostListItem>> GetPublishedAsync();
    Task<PostResponse?> GetBySlugAsync(string slug);
    Task<PostResponse?> CreateAsync(PostMutationRequest request);
    Task<PostResponse?> UpdateAsync(string slug, PostMutationRequest request);
    Task<bool> DeleteAsync(string slug);
}

public interface IServiceItemService
{
    Task<List<ServiceResponse>> GetAllAsync();
    Task<ServiceResponse?> GetBySlugAsync(string slug);
    Task<ServiceResponse?> CreateAsync(ServiceMutationRequest request);
    Task<ServiceResponse?> UpdateAsync(string slug, ServiceMutationRequest request);
    Task<bool> DeleteAsync(string slug);
}

public interface ISiteSettingService
{
    Task<SiteSettingDto> GetAsync();
    Task<SiteSettingDto> UpdateAsync(SiteSettingDto dto);
}

public interface IContactMessageService
{
    Task<List<ContactMessageResponse>> GetAllAsync();
    Task<int> CountUnreadAsync();
    Task<ContactMessageResponse> CreateAsync(CreateContactMessageRequest request);
    Task<ContactMessageResponse?> MarkReadAsync(Guid id);
    Task<bool> DeleteAsync(Guid id);
}

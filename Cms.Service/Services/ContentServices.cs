using System.Text.Json;
using Cms.Repository.Entities;
using Cms.Repository.Interfaces;
using Cms.Service.Common;
using Cms.Service.DTOs;
using Cms.Service.Interfaces;

namespace Cms.Service.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _repo;
    public PostService(IPostRepository repo) => _repo = repo;

    public async Task<List<PostListItem>> GetAllAsync() =>
        (await _repo.GetAllAsync()).Select(ToListItem).ToList();

    public async Task<List<PostListItem>> GetPublishedAsync() =>
        (await _repo.GetPublishedAsync()).Select(ToListItem).ToList();

    public async Task<PostResponse?> GetBySlugAsync(string slug)
    {
        var p = await _repo.GetBySlugAsync(slug);
        return p is null ? null : ToResponse(p);
    }

    public async Task<PostResponse?> CreateAsync(PostMutationRequest r)
    {
        var slug = SlugHelper.Slugify(r.Slug, r.Title);
        if (await _repo.SlugExistsAsync(slug)) return null;

        var now = DateTime.UtcNow;
        var post = new Post { Id = Guid.NewGuid(), CreatedAt = now, UpdatedAt = now };
        Apply(post, r, slug);
        await _repo.AddAsync(post);
        return ToResponse(post);
    }

    public async Task<PostResponse?> UpdateAsync(string slug, PostMutationRequest r)
    {
        var post = await _repo.GetBySlugAsync(slug);
        if (post is null) return null;

        var newSlug = SlugHelper.Slugify(r.Slug, r.Title);
        if (await _repo.SlugExistsAsync(newSlug, post.Id))
            throw new InvalidOperationException($"Slug '{newSlug}' is already in use.");

        Apply(post, r, newSlug);
        post.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(post);
        return ToResponse(post);
    }

    public async Task<bool> DeleteAsync(string slug)
    {
        var post = await _repo.GetBySlugAsync(slug);
        if (post is null) return false;
        await _repo.DeleteAsync(post);
        return true;
    }

    private static void Apply(Post post, PostMutationRequest r, string slug)
    {
        post.Slug = slug;
        post.Title = r.Title.Trim();
        post.Excerpt = r.Excerpt;
        post.CoverImage = r.CoverImage.Trim();
        post.BodyHtml = r.BodyHtml;
        post.Category = r.Category.Trim();
        post.Author = r.Author.Trim();
        post.ReadingTime = r.ReadingTime.Trim();
        post.Featured = r.Featured;
        post.Status = r.Status == "published" ? "published" : "draft";
        if (post.Status == "published" && post.PublishedAt is null)
            post.PublishedAt = DateTime.UtcNow;
        if (post.Status == "draft")
            post.PublishedAt = null;
    }

    private static PostListItem ToListItem(Post p) =>
        new(p.Id, p.Slug, p.Title, p.Excerpt, p.Category, p.Status, p.PublishedAt, p.CoverImage, p.Featured, p.UpdatedAt);

    private static PostResponse ToResponse(Post p) =>
        new(p.Id, p.Slug, p.Title, p.Excerpt, p.CoverImage, p.BodyHtml,
            p.Category, p.Author, p.ReadingTime, p.Status, p.PublishedAt, p.Featured, p.CreatedAt, p.UpdatedAt);
}

public class ServiceItemService : IServiceItemService
{
    private readonly IServiceItemRepository _repo;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    public ServiceItemService(IServiceItemRepository repo) => _repo = repo;

    public async Task<List<ServiceResponse>> GetAllAsync() =>
        (await _repo.GetAllAsync()).Select(ToResponse).ToList();

    public async Task<ServiceResponse?> GetBySlugAsync(string slug)
    {
        var s = await _repo.GetBySlugAsync(slug);
        return s is null ? null : ToResponse(s);
    }

    public async Task<ServiceResponse?> CreateAsync(ServiceMutationRequest r)
    {
        var slug = SlugHelper.Slugify(r.Slug, r.Title);
        if (await _repo.SlugExistsAsync(slug)) return null;

        var now = DateTime.UtcNow;
        var item = new ServiceItem { Id = Guid.NewGuid(), CreatedAt = now, UpdatedAt = now };
        Apply(item, r, slug);
        await _repo.AddAsync(item);
        return ToResponse(item);
    }

    public async Task<ServiceResponse?> UpdateAsync(string slug, ServiceMutationRequest r)
    {
        var item = await _repo.GetBySlugAsync(slug);
        if (item is null) return null;

        var newSlug = SlugHelper.Slugify(r.Slug, r.Title);
        if (await _repo.SlugExistsAsync(newSlug, item.Id))
            throw new InvalidOperationException($"Slug '{newSlug}' is already in use.");

        Apply(item, r, newSlug);
        item.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(item);
        return ToResponse(item);
    }

    public async Task<bool> DeleteAsync(string slug)
    {
        var item = await _repo.GetBySlugAsync(slug);
        if (item is null) return false;
        await _repo.DeleteAsync(item);
        return true;
    }

    private static void Apply(ServiceItem item, ServiceMutationRequest r, string slug)
    {
        item.Slug = slug;
        item.Title = r.Title.Trim();
        item.Short = r.Short;
        item.Summary = r.Summary;
        item.Image = r.Image.Trim();
        item.ProjectCategory = r.ProjectCategory;
        item.SortOrder = r.SortOrder;

        var detail = new ServiceDetail
        {
            Capabilities = r.Capabilities.Where(c => !string.IsNullOrWhiteSpace(c)).ToList(),
            Process = r.Process,
            Styles = r.Styles,
            Materials = r.Materials,
            Packages = r.Packages,
            Faqs = r.Faqs,
        };
        item.DetailJson = JsonSerializer.Serialize(detail, JsonOpts);
    }

    private static ServiceResponse ToResponse(ServiceItem s)
    {
        ServiceDetail detail;
        try { detail = JsonSerializer.Deserialize<ServiceDetail>(s.DetailJson, JsonOpts) ?? new(); }
        catch { detail = new(); }

        return new ServiceResponse(
            s.Id, s.Slug, s.Title, s.Short, s.Summary, s.Image, s.ProjectCategory, s.SortOrder,
            detail.Capabilities, detail.Process, detail.Styles, detail.Materials, detail.Packages, detail.Faqs,
            s.UpdatedAt);
    }
}

public class SiteSettingService : ISiteSettingService
{
    private readonly ISiteSettingRepository _repo;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public SiteSettingService(ISiteSettingRepository repo) => _repo = repo;

    public async Task<SiteSettingDto> GetAsync()
    {
        var s = await _repo.GetAsync();
        return s is null ? new SiteSettingDto() : ToDto(s);
    }

    public async Task<SiteSettingDto> UpdateAsync(SiteSettingDto dto)
    {
        var existing = await _repo.GetAsync();
        var entity = existing ?? new SiteSetting { Id = Guid.NewGuid() };

        entity.Name = dto.Name;
        entity.LegalName = dto.LegalName;
        entity.Tagline = dto.Tagline;
        entity.Description = dto.Description;
        entity.Phone = dto.Phone;
        entity.Email = dto.Email;
        entity.TaxId = dto.TaxId;
        entity.Facebook = dto.Facebook;
        entity.Zalo = dto.Zalo;
        entity.OfficesJson = JsonSerializer.Serialize(dto.Offices ?? new(), JsonOpts);
        entity.UpdatedAt = DateTime.UtcNow;

        await _repo.UpsertAsync(entity);
        return ToDto(entity);
    }

    private static SiteSettingDto ToDto(SiteSetting s)
    {
        List<Office> offices;
        try { offices = JsonSerializer.Deserialize<List<Office>>(s.OfficesJson, JsonOpts) ?? new(); }
        catch { offices = new(); }

        return new SiteSettingDto
        {
            Name = s.Name,
            LegalName = s.LegalName,
            Tagline = s.Tagline,
            Description = s.Description,
            Phone = s.Phone,
            Email = s.Email,
            TaxId = s.TaxId,
            Facebook = s.Facebook,
            Zalo = s.Zalo,
            Offices = offices,
        };
    }
}

public class ContactMessageService : IContactMessageService
{
    private readonly IContactMessageRepository _repo;
    public ContactMessageService(IContactMessageRepository repo) => _repo = repo;

    public async Task<List<ContactMessageResponse>> GetAllAsync() =>
        (await _repo.GetAllAsync()).Select(ToResponse).ToList();

    public Task<int> CountUnreadAsync() => _repo.CountUnreadAsync();

    public async Task<ContactMessageResponse> CreateAsync(CreateContactMessageRequest r)
    {
        var msg = new ContactMessage
        {
            Id = Guid.NewGuid(),
            Name = r.Name.Trim(),
            Email = r.Email.Trim(),
            Phone = r.Phone,
            Subject = r.Subject,
            Message = r.Message,
            IsRead = false,
            CreatedAt = DateTime.UtcNow,
        };
        await _repo.AddAsync(msg);
        return ToResponse(msg);
    }

    public async Task<ContactMessageResponse?> MarkReadAsync(Guid id)
    {
        var msg = await _repo.GetByIdAsync(id);
        if (msg is null) return null;
        msg.IsRead = true;
        await _repo.UpdateAsync(msg);
        return ToResponse(msg);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var msg = await _repo.GetByIdAsync(id);
        if (msg is null) return false;
        await _repo.DeleteAsync(msg);
        return true;
    }

    private static ContactMessageResponse ToResponse(ContactMessage m) =>
        new(m.Id, m.Name, m.Email, m.Phone, m.Subject, m.Message, m.IsRead, m.CreatedAt);
}

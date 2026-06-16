using System.Text.Json;
using Cms.Repository.Entities;
using Cms.Repository.Interfaces;
using Cms.Service.DTOs;
using Cms.Service.Interfaces;

namespace Cms.Service.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _repo;

    private static readonly JsonSerializerOptions JsonOpts =
        new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public ProjectService(IProjectRepository repo) => _repo = repo;

    public async Task<List<ProjectListItem>> GetAllAsync()
    {
        var projects = await _repo.GetAllAsync();
        return projects.Select(p => new ProjectListItem(
            p.Id, p.Slug, p.Title, p.Category, p.Location, p.Year, p.Cover, p.Gallery, p.Featured, p.UpdatedAt)).ToList();
    }

    public async Task<ProjectResponse?> GetBySlugAsync(string slug)
    {
        var p = await _repo.GetBySlugAsync(slug);
        return p is null ? null : ToResponse(p);
    }

    public async Task<ProjectResponse?> CreateAsync(ProjectMutationRequest request)
    {
        var slug = Slugify(request.Slug, request.Title);
        if (await _repo.SlugExistsAsync(slug))
            return null;

        var now = DateTime.UtcNow;
        var entity = new Project
        {
            Id = Guid.NewGuid(),
            CreatedAt = now,
            UpdatedAt = now,
        };
        Apply(entity, request, slug);

        await _repo.AddAsync(entity);
        return ToResponse(entity);
    }

    public async Task<ProjectResponse?> UpdateAsync(string slug, ProjectMutationRequest request)
    {
        var entity = await _repo.GetBySlugAsync(slug);
        if (entity is null) return null;

        var newSlug = Slugify(request.Slug, request.Title);
        if (await _repo.SlugExistsAsync(newSlug, entity.Id))
            throw new InvalidOperationException($"Slug '{newSlug}' is already in use.");

        Apply(entity, request, newSlug);
        entity.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(entity);
        return ToResponse(entity);
    }

    public async Task<bool> DeleteAsync(string slug)
    {
        var entity = await _repo.GetBySlugAsync(slug);
        if (entity is null) return false;

        await _repo.DeleteAsync(entity);
        return true;
    }

    // --- helpers -------------------------------------------------------------

    private static void Apply(Project entity, ProjectMutationRequest r, string slug)
    {
        entity.Slug = slug;
        entity.Title = r.Title.Trim();
        entity.Category = r.Category.Trim();
        entity.Location = r.Location.Trim();
        entity.Year = r.Year;
        entity.Excerpt = r.Excerpt;
        entity.Cover = r.Cover.Trim();
        entity.Hero = string.IsNullOrWhiteSpace(r.Hero) ? r.Cover.Trim() : r.Hero.Trim();
        entity.Subtitle = r.Subtitle;
        entity.Area = r.Area;
        entity.Client = r.Client;
        entity.WorkType = string.IsNullOrWhiteSpace(r.WorkType) ? "Thiết kế & Thi công" : r.WorkType.Trim();
        entity.BeforeImage = r.BeforeImage?.Trim() ?? string.Empty;
        entity.AfterImage = r.AfterImage?.Trim() ?? string.Empty;
        entity.Featured = r.Featured;
        entity.Gallery = r.Gallery.Where(g => !string.IsNullOrWhiteSpace(g)).ToList();
        entity.ContentJson = JsonSerializer.Serialize(r.Content ?? new(), JsonOpts);
    }

    private static ProjectResponse ToResponse(Project p)
    {
        List<ContentBlock> content;
        try
        {
            content = JsonSerializer.Deserialize<List<ContentBlock>>(p.ContentJson, JsonOpts) ?? new();
        }
        catch
        {
            content = new();
        }

        return new ProjectResponse(
            p.Id, p.Slug, p.Title, p.Category, p.Location, p.Year, p.Excerpt,
            p.Cover, p.Hero, p.Subtitle, p.Area, p.Client, p.WorkType, p.BeforeImage, p.AfterImage, p.Featured,
            p.Gallery, content, p.CreatedAt, p.UpdatedAt);
    }

    /// <summary>Use the provided slug, else derive one from the title.</summary>
    private static string Slugify(string? slug, string title)
    {
        var src = string.IsNullOrWhiteSpace(slug) ? title : slug;
        src = src.Trim().ToLowerInvariant();

        var normalized = src
            .Replace("đ", "d")
            .Replace("à", "a").Replace("á", "a").Replace("ả", "a").Replace("ã", "a").Replace("ạ", "a")
            .Replace("â", "a").Replace("ầ", "a").Replace("ấ", "a").Replace("ậ", "a").Replace("ẩ", "a").Replace("ẫ", "a")
            .Replace("ă", "a").Replace("ằ", "a").Replace("ắ", "a").Replace("ặ", "a").Replace("ẳ", "a").Replace("ẵ", "a")
            .Replace("è", "e").Replace("é", "e").Replace("ẻ", "e").Replace("ẽ", "e").Replace("ẹ", "e")
            .Replace("ê", "e").Replace("ề", "e").Replace("ế", "e").Replace("ệ", "e").Replace("ể", "e").Replace("ễ", "e")
            .Replace("ì", "i").Replace("í", "i").Replace("ỉ", "i").Replace("ĩ", "i").Replace("ị", "i")
            .Replace("ò", "o").Replace("ó", "o").Replace("ỏ", "o").Replace("õ", "o").Replace("ọ", "o")
            .Replace("ô", "o").Replace("ồ", "o").Replace("ố", "o").Replace("ộ", "o").Replace("ổ", "o").Replace("ỗ", "o")
            .Replace("ơ", "o").Replace("ờ", "o").Replace("ớ", "o").Replace("ợ", "o").Replace("ở", "o").Replace("ỡ", "o")
            .Replace("ù", "u").Replace("ú", "u").Replace("ủ", "u").Replace("ũ", "u").Replace("ụ", "u")
            .Replace("ư", "u").Replace("ừ", "u").Replace("ứ", "u").Replace("ự", "u").Replace("ử", "u").Replace("ữ", "u")
            .Replace("ỳ", "y").Replace("ý", "y").Replace("ỷ", "y").Replace("ỹ", "y").Replace("ỵ", "y");

        var chars = normalized
            .Select(c => char.IsLetterOrDigit(c) ? c : '-')
            .ToArray();
        var slugified = new string(chars);

        while (slugified.Contains("--"))
            slugified = slugified.Replace("--", "-");

        return slugified.Trim('-');
    }
}

using Cms.Service.DTOs;

namespace Cms.Service.Interfaces;

public interface IProjectService
{
    Task<List<ProjectListItem>> GetAllAsync();
    Task<ProjectResponse?> GetBySlugAsync(string slug);

    /// <summary>Returns the created project, or null if the slug is taken.</summary>
    Task<ProjectResponse?> CreateAsync(ProjectMutationRequest request);

    /// <summary>Returns the updated project, null if not found, throws if slug clashes.</summary>
    Task<ProjectResponse?> UpdateAsync(string slug, ProjectMutationRequest request);

    Task<bool> DeleteAsync(string slug);
}

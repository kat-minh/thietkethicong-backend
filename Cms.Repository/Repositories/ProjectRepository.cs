using Cms.Repository.Entities;
using Cms.Repository.Interfaces;
using Cms.Repository.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cms.Repository.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _db;

    public ProjectRepository(AppDbContext db) => _db = db;

    public Task<List<Project>> GetAllAsync() =>
        _db.Projects.AsNoTracking().OrderByDescending(p => p.UpdatedAt).ToListAsync();

    public Task<Project?> GetBySlugAsync(string slug) =>
        _db.Projects.FirstOrDefaultAsync(p => p.Slug == slug);

    public Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null) =>
        _db.Projects.AnyAsync(p => p.Slug == slug && (excludeId == null || p.Id != excludeId));

    public Task<int> CountAsync() => _db.Projects.CountAsync();

    public async Task AddAsync(Project project)
    {
        await _db.Projects.AddAsync(project);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Project project)
    {
        _db.Projects.Update(project);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Project project)
    {
        _db.Projects.Remove(project);
        await _db.SaveChangesAsync();
    }
}

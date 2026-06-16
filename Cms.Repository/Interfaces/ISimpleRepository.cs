using Cms.Repository.Entities;

namespace Cms.Repository.Interfaces;

/// <summary>Generic CRUD for flat, orderable content entities.</summary>
public interface ISimpleRepository<T> where T : class, IContentEntity
{
    Task<List<T>> GetAllAsync();
    Task<T?> GetByIdAsync(Guid id);
    Task AddAsync(T entity);
    Task<bool> UpdateAsync(T entity);
    Task<bool> DeleteAsync(Guid id);
}

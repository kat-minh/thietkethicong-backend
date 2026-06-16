using Cms.Repository.Entities;
using Cms.Repository.Interfaces;
using Cms.Repository.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cms.Repository.Repositories;

public class SimpleRepository<T> : ISimpleRepository<T> where T : class, IContentEntity
{
    private readonly AppDbContext _db;
    public SimpleRepository(AppDbContext db) => _db = db;

    public Task<List<T>> GetAllAsync() =>
        _db.Set<T>().AsNoTracking().OrderBy(e => e.SortOrder).ToListAsync();

    public async Task<T?> GetByIdAsync(Guid id) => await _db.Set<T>().FindAsync(id);

    public async Task AddAsync(T entity)
    {
        await _db.Set<T>().AddAsync(entity);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(T entity)
    {
        var exists = await _db.Set<T>().AnyAsync(e => e.Id == entity.Id);
        if (!exists) return false;
        _db.Set<T>().Update(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _db.Set<T>().FindAsync(id);
        if (entity is null) return false;
        _db.Set<T>().Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }
}

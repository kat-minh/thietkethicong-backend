using Cms.Repository.Entities;
using Cms.Repository.Interfaces;
using Cms.Repository.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cms.Repository.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db) => _db = db;

    public Task<User?> GetByUsernameAsync(string username) =>
        _db.Users.FirstOrDefaultAsync(u => u.Username == username);

    public async Task<User?> GetByIdAsync(Guid id) =>
        await _db.Users.FindAsync(id);

    public Task<List<User>> GetAllAsync() =>
        _db.Users.AsNoTracking().OrderBy(u => u.Username).ToListAsync();

    public Task<bool> AnyAsync() => _db.Users.AnyAsync();

    public Task<bool> ExistsAsync(string username, string email) =>
        _db.Users.AnyAsync(u => u.Username == username || u.Email == email);

    public async Task AddAsync(User user)
    {
        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(User user)
    {
        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
    }
}

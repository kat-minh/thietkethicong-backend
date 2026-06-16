using Cms.Repository.Entities;
using Cms.Repository.Interfaces;
using Cms.Service.DTOs;
using Cms.Service.Interfaces;

namespace Cms.Service.Services;

public class UserAdminService : IUserAdminService
{
    private readonly IUserRepository _users;
    public UserAdminService(IUserRepository users) => _users = users;

    public async Task<List<UserResponse>> GetAllAsync() =>
        (await _users.GetAllAsync())
            .Select(u => new UserResponse(u.Id, u.Username, u.Email, u.Role)).ToList();

    public async Task<UserResponse?> CreateAsync(CreateUserRequest request)
    {
        if (await _users.ExistsAsync(request.Username.Trim(), request.Email.Trim()))
            return null;

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username.Trim(),
            Email = request.Email.Trim(),
            Role = string.IsNullOrWhiteSpace(request.Role) ? "Admin" : request.Role.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
        };
        await _users.AddAsync(user);
        return new UserResponse(user.Id, user.Username, user.Email, user.Role);
    }

    public async Task<UserResponse?> UpdateAsync(Guid id, UpdateUserRequest request)
    {
        var user = await _users.GetByIdAsync(id);
        if (user is null) return null;

        var username = request.Username.Trim();
        var email = request.Email.Trim();

        var others = await _users.GetAllAsync();
        if (others.Any(u => u.Id != id && (u.Username == username || u.Email == email)))
            throw new InvalidOperationException("Username or email already exists.");

        user.Username = username;
        user.Email = email;
        user.Role = string.IsNullOrWhiteSpace(request.Role) ? user.Role : request.Role.Trim();
        if (!string.IsNullOrWhiteSpace(request.Password))
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        await _users.UpdateAsync(user);
        return new UserResponse(user.Id, user.Username, user.Email, user.Role);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var user = await _users.GetByIdAsync(id);
        if (user is null) return false;
        await _users.DeleteAsync(user);
        return true;
    }
}

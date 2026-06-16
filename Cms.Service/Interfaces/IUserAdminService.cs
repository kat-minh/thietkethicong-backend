using Cms.Service.DTOs;

namespace Cms.Service.Interfaces;

public interface IUserAdminService
{
    Task<List<UserResponse>> GetAllAsync();
    Task<UserResponse?> CreateAsync(CreateUserRequest request);
    /// <summary>Returns null if not found; throws InvalidOperationException on a username/email clash.</summary>
    Task<UserResponse?> UpdateAsync(Guid id, UpdateUserRequest request);
    Task<bool> DeleteAsync(Guid id);
}

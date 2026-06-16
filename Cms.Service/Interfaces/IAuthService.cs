using Cms.Service.DTOs;

namespace Cms.Service.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<UserProfileResponse?> GetProfileAsync(Guid userId);
}

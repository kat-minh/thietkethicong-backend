using Cms.Repository.Interfaces;
using Cms.Service.DTOs;
using Cms.Service.Interfaces;
using Cms.Service.Settings;
using Microsoft.Extensions.Options;

namespace Cms.Service.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly ITokenService _tokenService;
    private readonly JwtSettings _jwt;

    public AuthService(IUserRepository users, ITokenService tokenService, IOptions<JwtSettings> jwt)
    {
        _users = users;
        _tokenService = tokenService;
        _jwt = jwt.Value;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return null;
        }

        var user = await _users.GetByUsernameAsync(request.Username);

        // Same null result for "no such user" and "wrong password" to avoid user enumeration.
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return null;
        }

        var token = _tokenService.CreateToken(user);
        return new LoginResponse(token, "Bearer", _jwt.ExpiryMinutes);
    }

    public async Task<UserProfileResponse?> GetProfileAsync(Guid userId)
    {
        var user = await _users.GetByIdAsync(userId);
        return user is null
            ? null
            : new UserProfileResponse(user.Id, user.Username, user.Email, user.Role);
    }
}

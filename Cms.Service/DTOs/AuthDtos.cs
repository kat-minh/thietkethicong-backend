namespace Cms.Service.DTOs;

public record LoginRequest(string Username, string Password);

public record LoginResponse(string Token, string TokenType, int ExpiresInMinutes);

public record UserProfileResponse(Guid Id, string Username, string Email, string Role);

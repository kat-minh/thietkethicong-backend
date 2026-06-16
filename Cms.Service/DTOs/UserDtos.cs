namespace Cms.Service.DTOs;

public record UserResponse(Guid Id, string Username, string Email, string Role);

public class CreateUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "Admin";
}

public class UpdateUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "Admin";
    /// <summary>Optional — only changes the password when non-empty.</summary>
    public string? Password { get; set; }
}

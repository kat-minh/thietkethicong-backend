namespace Cms.Repository.Entities;

/// <summary>
/// Application user. Code-First entity mapped to the "Users" table in PostgreSQL.
/// </summary>
public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Username { get; set; } = string.Empty;

    /// <summary>BCrypt hash. Never store or return the plain-text password.</summary>
    public string PasswordHash { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = "Admin";
}

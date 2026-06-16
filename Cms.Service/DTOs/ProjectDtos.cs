namespace Cms.Service.DTOs;

/// <summary>One block of a project's rich body.</summary>
public class ContentBlock
{
    public string Type { get; set; } = "p"; // p | h1..h6 | img
    public string? Text { get; set; }
    public string? Src { get; set; }
}

/// <summary>Slim row for the admin list / public grid.</summary>
public record ProjectListItem(
    Guid Id,
    string Slug,
    string Title,
    string Category,
    string Location,
    int Year,
    string Cover,
    List<string> Gallery,
    bool Featured,
    DateTime UpdatedAt);

/// <summary>Full project payload.</summary>
public record ProjectResponse(
    Guid Id,
    string Slug,
    string Title,
    string Category,
    string Location,
    int Year,
    string Excerpt,
    string Cover,
    string Hero,
    string? Subtitle,
    string? Area,
    string? Client,
    bool Featured,
    List<string> Gallery,
    List<ContentBlock> Content,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public class ProjectMutationRequest
{
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Excerpt { get; set; } = string.Empty;
    public string Cover { get; set; } = string.Empty;
    public string Hero { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string? Area { get; set; }
    public string? Client { get; set; }
    public bool Featured { get; set; }
    public List<string> Gallery { get; set; } = new();
    public List<ContentBlock> Content { get; set; } = new();
}

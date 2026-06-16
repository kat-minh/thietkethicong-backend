namespace Cms.Repository.Entities;

/// <summary>
/// Portfolio project — the primary CMS content type for the BMT studio site.
/// Mirrors the shape used by the public site (src/lib/data/projects.ts).
/// </summary>
public class Project
{
    public Guid Id { get; set; } = Guid.NewGuid();

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

    /// <summary>Gallery image URLs — stored as a Postgres text[] array.</summary>
    public List<string> Gallery { get; set; } = new();

    /// <summary>
    /// Rich body as a JSON array of content blocks
    /// ({ "type": "p|h2|h3|img", "text"?, "src"? }). Stored as jsonb.
    /// </summary>
    public string ContentJson { get; set; } = "[]";

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

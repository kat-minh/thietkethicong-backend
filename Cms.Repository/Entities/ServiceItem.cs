namespace Cms.Repository.Entities;

/// <summary>A service offered by the studio (shown on /services).</summary>
public class ServiceItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    /// <summary>One-line tagline shown in lists.</summary>
    public string Short { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public string? Icon { get; set; }

    /// <summary>Links to a Project.Category for "related projects" on the detail page.</summary>
    public string? ProjectCategory { get; set; }

    /// <summary>
    /// Rich detail as JSON: { capabilities[], process[], styles[], materials[],
    /// packages[], faqs[] }. Stored as jsonb.
    /// </summary>
    public string DetailJson { get; set; } = "{}";

    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

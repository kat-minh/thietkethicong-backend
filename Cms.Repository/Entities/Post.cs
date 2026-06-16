namespace Cms.Repository.Entities;

/// <summary>Blog / news article.</summary>
public class Post
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
    public string CoverImage { get; set; } = string.Empty;

    /// <summary>Editorial category, e.g. "Tin tức", "Kiến thức", "Tuyển dụng".</summary>
    public string Category { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    /// <summary>Display string, e.g. "3 phút đọc".</summary>
    public string ReadingTime { get; set; } = string.Empty;

    /// <summary>Article body as HTML.</summary>
    public string BodyHtml { get; set; } = string.Empty;

    public List<string> Tags { get; set; } = new();

    /// <summary>"draft" or "published".</summary>
    public string Status { get; set; } = "draft";
    public DateTime? PublishedAt { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

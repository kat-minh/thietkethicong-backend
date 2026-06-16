namespace Cms.Repository.Entities;

/// <summary>Marker for simple, orderable, flat content entities.</summary>
public interface IContentEntity
{
    Guid Id { get; set; }
    int SortOrder { get; set; }
}

public class Testimonial : IContentEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Quote { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}

public class TeamMember : IContentEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Photo { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}

public class ProcessStep : IContentEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Step { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}

public class StatItem : IContentEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Value { get; set; } = string.Empty;
    public string Suffix { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}

public class Faq : IContentEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}

public class Partner : IContentEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Logo { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}

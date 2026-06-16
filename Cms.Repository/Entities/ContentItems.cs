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
    public int SortOrder { get; set; }
}

/// <summary>Core value shown on /about ("Giá trị cốt lõi").</summary>
public class Philosophy : IContentEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    /// <summary>Display number, e.g. "01".</summary>
    public string No { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}

/// <summary>Achievement / impressive number shown on /about ("Năng lực").</summary>
public class Award : IContentEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    /// <summary>Headline figure, e.g. "2000+", "10+".</summary>
    public string Year { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}

/// <summary>Commitment / certification line shown on /about ("Cam kết").</summary>
public class Certification : IContentEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Text { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}

/// <summary>
/// "Hồ sơ năng lực" album — managed independently of Projects (its own photo set).
/// Shown on /ho-so-nang-luc, grouped by Category into chapters.
/// </summary>
public class Album : IContentEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Cover { get; set; } = string.Empty;
    /// <summary>Album photo set (Postgres text[]).</summary>
    public List<string> Images { get; set; } = new();
    public int SortOrder { get; set; }
}

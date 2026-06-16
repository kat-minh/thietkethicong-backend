namespace Cms.Service.DTOs;

// ---------------- Posts ----------------
public record PostListItem(
    Guid Id, string Slug, string Title, string Excerpt, string Category, string Status, DateTime? PublishedAt, string CoverImage, bool Featured, DateTime UpdatedAt);

public record PostResponse(
    Guid Id, string Slug, string Title, string Excerpt, string CoverImage, string BodyHtml,
    string Category, string Author, string ReadingTime,
    string Status, DateTime? PublishedAt, bool Featured, DateTime CreatedAt, DateTime UpdatedAt);

public class PostMutationRequest
{
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
    public string CoverImage { get; set; } = string.Empty;
    public string BodyHtml { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string ReadingTime { get; set; } = string.Empty;
    public string Status { get; set; } = "draft";
    public bool Featured { get; set; }
}

// ---------------- Services ----------------
public record ServiceProcessStepDto(string Title, string Body);
public record ServiceNamedNoteDto(string Name, string Note);
public record ServicePackageDto(string Name, string Price, List<string> Features);
public record ServiceFaqDto(string Q, string A);

public record ServiceResponse(
    Guid Id, string Slug, string Title, string Short, string Summary, string Image, string? Icon,
    string? ProjectCategory, int SortOrder,
    List<string> Capabilities, List<ServiceProcessStepDto> Process,
    List<ServiceNamedNoteDto> Styles, List<ServiceNamedNoteDto> Materials,
    List<ServicePackageDto> Packages, List<ServiceFaqDto> Faqs, DateTime UpdatedAt);

public class ServiceMutationRequest
{
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Short { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? ProjectCategory { get; set; }
    public int SortOrder { get; set; }
    public List<string> Capabilities { get; set; } = new();
    public List<ServiceProcessStepDto> Process { get; set; } = new();
    public List<ServiceNamedNoteDto> Styles { get; set; } = new();
    public List<ServiceNamedNoteDto> Materials { get; set; } = new();
    public List<ServicePackageDto> Packages { get; set; } = new();
    public List<ServiceFaqDto> Faqs { get; set; } = new();
}

/// <summary>Internal shape (de)serialized to <c>ServiceItem.DetailJson</c>.</summary>
public class ServiceDetail
{
    public List<string> Capabilities { get; set; } = new();
    public List<ServiceProcessStepDto> Process { get; set; } = new();
    public List<ServiceNamedNoteDto> Styles { get; set; } = new();
    public List<ServiceNamedNoteDto> Materials { get; set; } = new();
    public List<ServicePackageDto> Packages { get; set; } = new();
    public List<ServiceFaqDto> Faqs { get; set; } = new();
}

// ---------------- Site settings ----------------
public class Office
{
    public string Label { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}

public class SiteSettingDto
{
    public string Name { get; set; } = string.Empty;
    public string LegalName { get; set; } = string.Empty;
    public string Tagline { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty;
    public string Facebook { get; set; } = string.Empty;
    public string Zalo { get; set; } = string.Empty;
    public List<Office> Offices { get; set; } = new();
}

// ---------------- Contact messages ----------------
public record ContactMessageResponse(
    Guid Id, string Name, string Email, string? Phone, string? Subject, string Message, bool IsRead, DateTime CreatedAt);

public class CreateContactMessageRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Subject { get; set; }
    public string Message { get; set; } = string.Empty;
}

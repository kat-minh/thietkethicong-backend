namespace Cms.Repository.Entities;

/// <summary>A job opening shown on the public careers page (/tuyen-dung).</summary>
public class JobPosting : IContentEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;   // e.g. Thiết kế, Thi công
    public string Location { get; set; } = string.Empty;     // e.g. TP.HCM
    public string EmploymentType { get; set; } = string.Empty; // e.g. Toàn thời gian
    public string Salary { get; set; } = string.Empty;       // e.g. Thoả thuận
    public string Excerpt { get; set; } = string.Empty;
    public string DescriptionHtml { get; set; } = string.Empty; // rich text
    public bool IsOpen { get; set; } = true;
    public int SortOrder { get; set; }
}

/// <summary>A submission from the shared "Ứng tuyển" application form.</summary>
public class JobApplication : IContentEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;     // vị trí ứng tuyển
    public string CoverLetter { get; set; } = string.Empty;
    public string CvUrl { get; set; } = string.Empty;        // link to the CV (URL for now)
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }

    /// <summary>Unused for applications, present only to satisfy IContentEntity.</summary>
    public int SortOrder { get; set; }
}

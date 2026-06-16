namespace Cms.Repository.Entities;

/// <summary>
/// Singleton site configuration (one row). Mirrors src/lib/site.ts so the
/// public website can be driven from the CMS.
/// </summary>
public class SiteSetting
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;
    public string LegalName { get; set; } = string.Empty;
    public string Tagline { get; set; } = string.Empty;
    /// <summary>Short brand statement shown in the footer.</summary>
    public string Manifesto { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty;

    /// <summary>JSON array of { label, address }.</summary>
    public string OfficesJson { get; set; } = "[]";

    /// <summary>JSON array of { label, href } social links.</summary>
    public string SocialJson { get; set; } = "[]";

    public DateTime UpdatedAt { get; set; }
}

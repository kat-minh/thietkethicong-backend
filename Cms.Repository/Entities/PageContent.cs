namespace Cms.Repository.Entities;

/// <summary>
/// One editable piece of page copy or image, addressed by a stable Key
/// (e.g. "about.hero.intro"). The set of keys is fixed by the site/seeder;
/// the admin only edits Value. Self-describing (Page/Section/Kind) so the
/// admin can render an editor without hardcoding the key list.
/// </summary>
public class PageContent
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Stable address, unique. e.g. "home.hero.intro".</summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>The editable value (text, rich text, or image URL/id).</summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>Grouping for the admin UI, e.g. "home", "about".</summary>
    public string Page { get; set; } = string.Empty;

    /// <summary>Human label shown in the admin editor.</summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>"text" | "textarea" | "image" — drives the admin input.</summary>
    public string Kind { get; set; } = "text";

    public int SortOrder { get; set; }
}

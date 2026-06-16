using System.Text.Json;
using System.Text.Json.Serialization;
using Cms.Repository.Entities;
using Cms.Repository.Interfaces;
using Cms.Service.DTOs;
using Cms.Service.Interfaces;

namespace Cms.Service.Services;

/// <summary>
/// Idempotent seeding. The bulk content (projects, posts, services, studio
/// blocks) is loaded from the reduced JSON snapshots under SeedData/, exported
/// from the public site's fallback data. Each block only runs when its table is
/// empty, so it is safe on every startup.
/// </summary>
public class DatabaseSeeder : IDatabaseSeeder
{
    private readonly IUserRepository _users;
    private readonly IProjectRepository _projects;
    private readonly IPostRepository _posts;
    private readonly IServiceItemRepository _services;
    private readonly ISiteSettingRepository _settings;
    private readonly ISimpleRepository<Testimonial> _testimonials;
    private readonly ISimpleRepository<TeamMember> _team;
    private readonly ISimpleRepository<ProcessStep> _process;
    private readonly ISimpleRepository<StatItem> _stats;
    private readonly ISimpleRepository<Faq> _faqs;
    private readonly ISimpleRepository<Partner> _partners;
    private readonly ISimpleRepository<Philosophy> _philosophy;
    private readonly ISimpleRepository<Award> _awards;
    private readonly ISimpleRepository<Certification> _certifications;
    private readonly IPageContentRepository _pageContent;

    public DatabaseSeeder(
        IUserRepository users,
        IProjectRepository projects,
        IPostRepository posts,
        IServiceItemRepository services,
        ISiteSettingRepository settings,
        ISimpleRepository<Testimonial> testimonials,
        ISimpleRepository<TeamMember> team,
        ISimpleRepository<ProcessStep> process,
        ISimpleRepository<StatItem> stats,
        ISimpleRepository<Faq> faqs,
        ISimpleRepository<Partner> partners,
        ISimpleRepository<Philosophy> philosophy,
        ISimpleRepository<Award> awards,
        ISimpleRepository<Certification> certifications,
        IPageContentRepository pageContent)
    {
        _users = users;
        _projects = projects;
        _posts = posts;
        _services = services;
        _settings = settings;
        _testimonials = testimonials;
        _team = team;
        _process = process;
        _stats = stats;
        _faqs = faqs;
        _partners = partners;
        _philosophy = philosophy;
        _awards = awards;
        _certifications = certifications;
        _pageContent = pageContent;
    }

    // Matches how ProjectService/ServiceItemService persist their JSON columns.
    private static readonly JsonSerializerOptions StoreOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };
    private static readonly JsonSerializerOptions ReadOpts = new() { PropertyNameCaseInsensitive = true };

    private static T? ReadSeed<T>(string file)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "SeedData", file);
        if (!File.Exists(path)) return default;
        try { return JsonSerializer.Deserialize<T>(File.ReadAllText(path), ReadOpts); }
        catch { return default; }
    }

    public async Task SeedAsync()
    {
        await SeedAdminAsync();
        await SeedProjectsAsync();
        await SeedPostsAsync();
        await SeedServicesAsync();
        await SeedSettingsAsync();
        await SeedStudioAsync();
        await SeedPageContentAsync();
    }

    private async Task SeedPageContentAsync()
    {
        // Additive: only insert keys that don't exist yet, so new keys ship to
        // already-seeded databases without wiping admin edits to existing ones.
        var existing = (await _pageContent.GetAllAsync()).Select(p => p.Key).ToHashSet();
        var items = new List<PageContent>
        {
            // —— Trang chủ ——
            new() { Key = "home.hero.intro", Page = "Trang chủ", Label = "Hero · Đoạn giới thiệu", Kind = "textarea", SortOrder = 1,
                Value = "Đơn vị thiết kế & thi công trọn gói tại TP.HCM, kiến tạo những không gian sống và thương mại vừa cuốn hút, vừa đáng nhớ." },
            new() { Key = "home.hero.image", Page = "Trang chủ", Label = "Hero · Ảnh nền", Kind = "image", SortOrder = 2,
                Value = "1600585154340-be6161a56a0c" },

            // —— Giới thiệu ——
            new() { Key = "about.hero.intro", Page = "Giới thiệu", Label = "Hero · Đoạn mở đầu", Kind = "textarea", SortOrder = 1,
                Value = "BMT Decor là một trong những đơn vị thi công được đánh giá cao tại TP.HCM bởi sự sáng tạo và những công trình chất lượng, quy mô. Chúng tôi giữ thiết kế và thi công dưới một mái nhà để ý tưởng được hiện thực hóa trọn vẹn." },
            new() { Key = "about.story.eyebrow", Page = "Giới thiệu", Label = "Câu chuyện · Nhãn", Kind = "text", SortOrder = 2,
                Value = "Câu chuyện của chúng tôi" },
            new() { Key = "about.story.lead", Page = "Giới thiệu", Label = "Câu chuyện · Câu dẫn", Kind = "textarea", SortOrder = 3,
                Value = "BMT Decor không chỉ thiết kế và thi công, chúng tôi còn đồng hành cùng thành công của bạn." },
            new() { Key = "about.story.p1", Page = "Giới thiệu", Label = "Câu chuyện · Đoạn 1", Kind = "textarea", SortOrder = 4,
                Value = "BMT Decor chủ động đầu tư xưởng sản xuất hơn 3.500m², chuyên sản xuất – gia công nội thất bởi đội ngũ thợ lành nghề và duyệt trực tiếp từ thiết kế. Quy trình khép kín ấy là thế mạnh thầm lặng của chúng tôi: thiết kế ra sao, thi công đúng như vậy." },
            new() { Key = "about.story.p2", Page = "Giới thiệu", Label = "Câu chuyện · Đoạn 2", Kind = "textarea", SortOrder = 5,
                Value = "Là đối tác tin cậy của các thương hiệu lớn — VinCom, Central Mall, Hyundai và nhiều doanh nghiệp khác. Mỗi công trình nhà ở, văn phòng, showroom, nhà hàng hay thẩm mỹ viện đều là một minh chứng cho thiết kế tinh tế và thi công chất lượng." },
            new() { Key = "about.vision.eyebrow", Page = "Giới thiệu", Label = "Tầm nhìn · Nhãn", Kind = "text", SortOrder = 6,
                Value = "Tầm nhìn" },
            new() { Key = "about.vision.heading", Page = "Giới thiệu", Label = "Tầm nhìn · Tiêu đề", Kind = "textarea", SortOrder = 7,
                Value = "Kiến tạo không gian nâng tầm trải nghiệm sống." },
            new() { Key = "about.vision.text", Page = "Giới thiệu", Label = "Tầm nhìn · Nội dung", Kind = "textarea", SortOrder = 8,
                Value = "Chúng tôi đo lường thành công không bằng mét vuông hay giải thưởng, mà bằng những khoảnh khắc thường ngày mà không gian tạo ra — ly cà phê sáng trong ánh sáng đẹp, bữa tối kéo dài, và căn nhà vẫn vẹn nguyên cảm xúc sau nhiều năm." },
            new() { Key = "about.vision.image", Page = "Giới thiệu", Label = "Tầm nhìn · Ảnh", Kind = "image", SortOrder = 9,
                Value = "1600585152220-90363fe7e115" },

            // —— Liên hệ ——
            new() { Key = "contact.hero.intro", Page = "Liên hệ", Label = "Hero · Đoạn mở đầu", Kind = "textarea", SortOrder = 1,
                Value = "Hãy chia sẻ về không gian và mong muốn của bạn. Buổi tư vấn đầu tiên luôn miễn phí — và thường là khởi đầu của một công trình đáng nhớ." },

            // —— Báo giá ——
            new() { Key = "estimator.hero.intro", Page = "Báo giá", Label = "Hero · Đoạn mở đầu", Kind = "textarea", SortOrder = 1,
                Value = "Bốn câu hỏi nhanh để có khoảng giá thực tế — sau đó để lại thông tin để nhận báo giá chi tiết theo từng hạng mục cho dự án của bạn." },
        };

        var missing = items.Where(i => !existing.Contains(i.Key)).ToList();
        if (missing.Count > 0) await _pageContent.AddRangeAsync(missing);
    }

    private async Task SeedProjectsAsync()
    {
        if (await _projects.CountAsync() > 0) return;
        var data = ReadSeed<List<SeedProject>>("projects.json");
        if (data is null) return;
        var now = DateTime.UtcNow;
        foreach (var p in data)
        {
            await _projects.AddAsync(new Project
            {
                Id = Guid.NewGuid(),
                Slug = p.Slug,
                Title = p.Title,
                Category = p.Category,
                Location = p.Location,
                Year = p.Year,
                Excerpt = p.Excerpt,
                Cover = p.Cover,
                Hero = string.IsNullOrWhiteSpace(p.Hero) ? p.Cover : p.Hero,
                Subtitle = p.Subtitle,
                Area = p.Area,
                Client = p.Client,
                Featured = p.Featured,
                Gallery = (p.Gallery ?? new()).Where(g => !string.IsNullOrWhiteSpace(g)).ToList(),
                ContentJson = JsonSerializer.Serialize(p.Content ?? new(), StoreOpts),
                CreatedAt = now,
                UpdatedAt = now,
            });
        }
    }

    private async Task SeedPostsAsync()
    {
        if (await _posts.CountAsync() > 0) return;
        var data = ReadSeed<List<SeedPost>>("posts.json");
        if (data is null) return;
        var now = DateTime.UtcNow;
        foreach (var p in data)
        {
            await _posts.AddAsync(new Post
            {
                Id = Guid.NewGuid(),
                Slug = p.Slug,
                Title = p.Title,
                Excerpt = p.Excerpt,
                CoverImage = p.CoverImage,
                Category = p.Category,
                Author = p.Author,
                ReadingTime = p.ReadingTime,
                BodyHtml = p.BodyHtml,
                Featured = p.Featured,
                Status = string.IsNullOrWhiteSpace(p.Status) ? "published" : p.Status,
                PublishedAt = now,
                CreatedAt = now,
                UpdatedAt = now,
            });
        }
    }

    private async Task SeedServicesAsync()
    {
        if (await _services.CountAsync() > 0) return;
        var data = ReadSeed<List<SeedService>>("services.json");
        if (data is null) return;
        var now = DateTime.UtcNow;
        foreach (var s in data)
        {
            await _services.AddAsync(new ServiceItem
            {
                Id = Guid.NewGuid(),
                Slug = s.Slug,
                Title = s.Title,
                Short = s.Short,
                Summary = s.Summary,
                Image = s.Image,
                ProjectCategory = string.IsNullOrWhiteSpace(s.ProjectCategory) ? null : s.ProjectCategory,
                SortOrder = s.SortOrder,
                DetailJson = JsonSerializer.Serialize(s.Detail ?? new(), StoreOpts),
                CreatedAt = now,
                UpdatedAt = now,
            });
        }
    }

    private async Task SeedStudioAsync()
    {
        var studio = ReadSeed<StudioSeed>("studio.json") ?? new();

        if ((await _stats.GetAllAsync()).Count == 0)
            foreach (var s in studio.Stats) await _stats.AddAsync(s);

        if ((await _process.GetAllAsync()).Count == 0)
            foreach (var p in studio.Process) await _process.AddAsync(p);

        if ((await _testimonials.GetAllAsync()).Count == 0)
            foreach (var t in studio.Testimonials) await _testimonials.AddAsync(t);

        if ((await _team.GetAllAsync()).Count == 0)
            foreach (var m in studio.Team) await _team.AddAsync(m);

        if ((await _partners.GetAllAsync()).Count == 0)
            foreach (var p in studio.Partners) await _partners.AddAsync(p);

        if ((await _faqs.GetAllAsync()).Count == 0)
            foreach (var f in studio.Faqs) await _faqs.AddAsync(f);

        if ((await _philosophy.GetAllAsync()).Count == 0)
            foreach (var p in studio.Philosophy) await _philosophy.AddAsync(p);

        if ((await _awards.GetAllAsync()).Count == 0)
            foreach (var a in studio.Awards) await _awards.AddAsync(a);

        if ((await _certifications.GetAllAsync()).Count == 0)
            foreach (var c in studio.Certifications) await _certifications.AddAsync(c);
    }

    private async Task SeedAdminAsync()
    {
        if (await _users.AnyAsync()) return;
        await _users.AddAsync(new User
        {
            Id = Guid.NewGuid(),
            Username = "admin",
            Email = "admin@bmt.local",
            Role = "Admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
        });
    }

    private async Task SeedSettingsAsync()
    {
        if (await _settings.GetAsync() is not null) return;

        await _settings.UpsertAsync(new SiteSetting
        {
            Id = Guid.NewGuid(),
            Name = "BMT DECOR",
            LegalName = "CÔNG TY TNHH TMDV BMT DECOR",
            Tagline = "Thiết kế · Thi công · Nội thất trọn gói",
            Manifesto = "Chúng tôi không chỉ xây dựng không gian — chúng tôi kiến tạo trải nghiệm.",
            Description = "BMT Decor là đơn vị thiết kế và thi công nội thất tại TP.HCM, mang đến giải pháp trọn gói cho nhà ở và công trình thương mại với hơn 2000+ dự án đã hoàn thiện.",
            Phone = "0934888881",
            Email = "bmt.decor@thietkethicong.vn",
            TaxId = "0317552987",
            OfficesJson = "[{\"label\":\"Cơ sở 1\",\"address\":\"380 Vũ Huy Tấn, Phường 15, Quận Bình Thạnh, TP.HCM\"},{\"label\":\"Cơ sở 2\",\"address\":\"58 Thành Thái, Phường 12, Quận 10, TP.HCM\"},{\"label\":\"Xưởng sản xuất\",\"address\":\"Nguyễn Thị Tú, P. Bình Hưng Hòa B, Bình Tân, TP.HCM\"}]",
            SocialJson = "[{\"label\":\"Facebook\",\"href\":\"https://facebook.com\"},{\"label\":\"Zalo\",\"href\":\"https://zalo.me/1255459439490998198\"}]",
            UpdatedAt = DateTime.UtcNow,
        });
    }

    // ---- JSON seed DTOs (camelCase files under SeedData/) -------------------
    private sealed record SeedBlock(string Type, string? Text, string? Src);

    private sealed class SeedProject
    {
        public string Slug { get; set; } = "";
        public string Title { get; set; } = "";
        public string Category { get; set; } = "";
        public string Location { get; set; } = "";
        public int Year { get; set; }
        public string Excerpt { get; set; } = "";
        public string Cover { get; set; } = "";
        public string Hero { get; set; } = "";
        public string Subtitle { get; set; } = "";
        public string Area { get; set; } = "";
        public string Client { get; set; } = "";
        public bool Featured { get; set; }
        public List<string> Gallery { get; set; } = new();
        public List<SeedBlock> Content { get; set; } = new();
    }

    private sealed class SeedPost
    {
        public string Slug { get; set; } = "";
        public string Title { get; set; } = "";
        public string Excerpt { get; set; } = "";
        public string CoverImage { get; set; } = "";
        public string Category { get; set; } = "";
        public string Author { get; set; } = "";
        public string ReadingTime { get; set; } = "";
        public string BodyHtml { get; set; } = "";
        public bool Featured { get; set; }
        public string Status { get; set; } = "published";
    }

    private sealed class SeedService
    {
        public string Slug { get; set; } = "";
        public string Title { get; set; } = "";
        public string Short { get; set; } = "";
        public string Summary { get; set; } = "";
        public string Image { get; set; } = "";
        public string ProjectCategory { get; set; } = "";
        public int SortOrder { get; set; }
        public ServiceDetail Detail { get; set; } = new();
    }

    private sealed class StudioSeed
    {
        public List<StatItem> Stats { get; set; } = new();
        public List<ProcessStep> Process { get; set; } = new();
        public List<Testimonial> Testimonials { get; set; } = new();
        public List<TeamMember> Team { get; set; } = new();
        public List<Partner> Partners { get; set; } = new();
        public List<Faq> Faqs { get; set; } = new();
        public List<Philosophy> Philosophy { get; set; } = new();
        public List<Award> Awards { get; set; } = new();
        public List<Certification> Certifications { get; set; } = new();
    }
}

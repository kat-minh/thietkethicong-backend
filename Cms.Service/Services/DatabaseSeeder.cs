using Cms.Repository.Entities;
using Cms.Repository.Interfaces;
using Cms.Service.Interfaces;

namespace Cms.Service.Services;

/// <summary>
/// Idempotent seeding: admin + sample projects/posts/services + site settings.
/// Each block only runs when its table is empty, so it is safe on every startup.
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
        ISimpleRepository<Faq> faqs)
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
    }

    public async Task SeedAsync()
    {
        await SeedAdminAsync();
        await SeedProjectsAsync();
        await SeedPostsAsync();
        await SeedServicesAsync();
        await SeedSettingsAsync();
        await SeedStudioAsync();
    }

    private async Task SeedStudioAsync()
    {
        if ((await _stats.GetAllAsync()).Count == 0)
        {
            var stats = new[]
            {
                new StatItem { Value = "10", Suffix = "+", Label = "Năm kinh nghiệm", SortOrder = 1 },
                new StatItem { Value = "2000", Suffix = "+", Label = "Dự án hoàn thiện", SortOrder = 2 },
                new StatItem { Value = "96", Suffix = "%", Label = "Khách hàng quay lại", SortOrder = 3 },
                new StatItem { Value = "3500", Suffix = "m²", Label = "Xưởng sản xuất", SortOrder = 4 },
            };
            foreach (var s in stats) await _stats.AddAsync(s);
        }

        if ((await _process.GetAllAsync()).Count == 0)
        {
            var steps = new[]
            {
                new ProcessStep { Step = "01", Title = "Tư vấn & khảo sát", Body = "Lắng nghe nhu cầu, khảo sát mặt bằng.", Duration = "Tuần 1", SortOrder = 1 },
                new ProcessStep { Step = "02", Title = "Thiết kế & 3D", Body = "Concept, mặt bằng, vật liệu và phối cảnh 3D.", Duration = "Tuần 2–6", SortOrder = 2 },
                new ProcessStep { Step = "03", Title = "Thi công & giám sát", Body = "Đội thi công triển khai, giám sát chất lượng.", Duration = "Tuần 7–20", SortOrder = 3 },
                new ProcessStep { Step = "04", Title = "Nghiệm thu & bàn giao", Body = "Hoàn thiện, nghiệm thu và bàn giao.", Duration = "Tuần cuối", SortOrder = 4 },
            };
            foreach (var p in steps) await _process.AddAsync(p);
        }

        if ((await _testimonials.GetAllAsync()).Count == 0)
        {
            var items = new[]
            {
                new Testimonial { Quote = "Thiết kế tinh tế, thi công chất lượng. Rất hài lòng!", Name = "Chủ căn hộ", Role = "Khách hàng TP.HCM", SortOrder = 1 },
                new Testimonial { Quote = "BMT Decor chú trọng từng chi tiết, không gian hoàn hảo.", Name = "Chủ quán café", Role = "Khách hàng TP.HCM", SortOrder = 2 },
                new Testimonial { Quote = "Không gian spa sang trọng và thư giãn hơn nhờ BMT Decor.", Name = "Chủ spa", Role = "Khách hàng TP.HCM", SortOrder = 3 },
            };
            foreach (var t in items) await _testimonials.AddAsync(t);
        }

        if ((await _team.GetAllAsync()).Count == 0)
        {
            var members = new[]
            {
                new TeamMember { Name = "Đội ngũ Kiến trúc sư", Role = "Thiết kế kiến trúc & nội thất", Photo = "", SortOrder = 1 },
                new TeamMember { Name = "Đội Thi công", Role = "Giám sát & thi công công trình", Photo = "", SortOrder = 2 },
            };
            foreach (var m in members) await _team.AddAsync(m);
        }

        if ((await _faqs.GetAllAsync()).Count == 0)
        {
            var faqs = new[]
            {
                new Faq { Question = "BMT Decor nhận thi công ở đâu?", Answer = "Trụ sở tại TP.HCM, nhận dự án trên toàn quốc.", SortOrder = 1 },
                new Faq { Question = "Có tư vấn thiết kế miễn phí không?", Answer = "Có — buổi tư vấn đầu tiên hoàn toàn miễn phí.", SortOrder = 2 },
                new Faq { Question = "Có làm theo ngân sách cố định không?", Answer = "Luôn luôn. Báo giá minh bạch theo ngân sách của bạn.", SortOrder = 3 },
            };
            foreach (var f in faqs) await _faqs.AddAsync(f);
        }
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

    private async Task SeedProjectsAsync()
    {
        if (await _projects.CountAsync() > 0) return;
        var now = DateTime.UtcNow;

        var samples = new List<Project>
        {
            new() { Id = Guid.NewGuid(), Slug = "tham-my-vien-klain", Title = "Thẩm mỹ viện KLAIN", Category = "Thẩm mỹ viện - Nha khoa", Location = "TP.HCM", Year = 2024, Excerpt = "Không gian thẩm mỹ viện sang trọng, hiện đại.", Cover = "https://thietkethicong.vn/wp-content/uploads/2024/08/IMG_4051.jpg", Hero = "https://thietkethicong.vn/wp-content/uploads/2024/08/IMG_4051.jpg", Featured = true, Gallery = new() { "https://thietkethicong.vn/wp-content/uploads/2024/08/IMG_4051.jpg" }, ContentJson = "[{\"type\":\"p\",\"text\":\"Thiết kế và thi công trọn gói.\"}]", CreatedAt = now, UpdatedAt = now },
            new() { Id = Guid.NewGuid(), Slug = "showroom-noi-that-demo", Title = "Showroom nội thất (demo)", Category = "Showroom", Location = "Hà Nội", Year = 2023, Excerpt = "Showroom trưng bày nội thất cao cấp.", Cover = "https://thietkethicong.vn/wp-content/uploads/2024/08/IMG_4022.jpg", Hero = "https://thietkethicong.vn/wp-content/uploads/2024/08/IMG_4022.jpg", Featured = false, Gallery = new() { "https://thietkethicong.vn/wp-content/uploads/2024/08/IMG_4022.jpg" }, ContentJson = "[{\"type\":\"p\",\"text\":\"Dự án mẫu.\"}]", CreatedAt = now, UpdatedAt = now },
            new() { Id = Guid.NewGuid(), Slug = "van-phong-lam-viec-demo", Title = "Văn phòng làm việc (demo)", Category = "Văn phòng", Location = "Đà Nẵng", Year = 2023, Excerpt = "Văn phòng hiện đại, tối ưu công năng.", Cover = "https://thietkethicong.vn/wp-content/uploads/2024/08/IMG_4024.jpg", Hero = "https://thietkethicong.vn/wp-content/uploads/2024/08/IMG_4024.jpg", Featured = true, Gallery = new() { "https://thietkethicong.vn/wp-content/uploads/2024/08/IMG_4024.jpg" }, ContentJson = "[{\"type\":\"p\",\"text\":\"Dự án mẫu.\"}]", CreatedAt = now, UpdatedAt = now },
        };
        foreach (var p in samples) await _projects.AddAsync(p);
    }

    private async Task SeedPostsAsync()
    {
        if (await _posts.CountAsync() > 0) return;
        var now = DateTime.UtcNow;

        await _posts.AddAsync(new Post
        {
            Id = Guid.NewGuid(),
            Slug = "xu-huong-noi-that-2024",
            Title = "Xu hướng nội thất 2024",
            Excerpt = "Những phong cách nội thất dẫn đầu năm 2024.",
            CoverImage = "https://thietkethicong.vn/wp-content/uploads/2024/08/IMG_4015.jpg",
            BodyHtml = "<p>Nội dung bài viết mẫu cho CMS. Bạn có thể chỉnh sửa trong trang quản trị.</p>",
            Status = "published",
            PublishedAt = now,
            CreatedAt = now,
            UpdatedAt = now,
        });
    }

    private async Task SeedServicesAsync()
    {
        if (await _services.CountAsync() > 0) return;
        var now = DateTime.UtcNow;

        var items = new (string slug, string title, int order)[]
        {
            ("thiet-ke-noi-that", "Thiết kế nội thất", 1),
            ("xay-dung-sua-chua", "Xây dựng - Sửa chữa", 2),
            ("thiet-ke-van-phong", "Thiết kế văn phòng", 3),
            ("showroom-shop", "Showroom - Shop", 4),
        };

        foreach (var (slug, title, order) in items)
        {
            await _services.AddAsync(new ServiceItem
            {
                Id = Guid.NewGuid(),
                Slug = slug,
                Title = title,
                Summary = $"Dịch vụ {title.ToLower()} trọn gói của BMT Decor.",
                Image = "https://thietkethicong.vn/wp-content/uploads/2024/08/IMG_4024.jpg",
                SortOrder = order,
                CreatedAt = now,
                UpdatedAt = now,
            });
        }
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
            Description = "BMT Decor là đơn vị thiết kế và thi công nội thất tại TP.HCM.",
            Phone = "0934888881",
            Email = "bmt.decor@thietkethicong.vn",
            TaxId = "0317552987",
            Facebook = "https://facebook.com",
            Zalo = "https://zalo.me/1255459439490998198",
            OfficesJson = "[{\"label\":\"Cơ sở 1\",\"address\":\"380 Vũ Huy Tấn, P.15, Bình Thạnh, TP.HCM\"},{\"label\":\"Cơ sở 2\",\"address\":\"58 Thành Thái, P.12, Quận 10, TP.HCM\"}]",
            UpdatedAt = DateTime.UtcNow,
        });
    }
}

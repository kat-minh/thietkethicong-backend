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
    private readonly ISimpleRepository<JobPosting> _jobs;
    private readonly ISimpleRepository<Album> _albums;
    private readonly ISimpleRepository<JobApplication> _applications;
    private readonly IContactMessageRepository _messages;
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
        ISimpleRepository<JobPosting> jobs,
        ISimpleRepository<Album> albums,
        ISimpleRepository<JobApplication> applications,
        IContactMessageRepository messages,
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
        _jobs = jobs;
        _albums = albums;
        _applications = applications;
        _messages = messages;
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
        await SeedJobsAsync();
        await SeedAlbumsAsync();
        await SeedMessagesAsync();
        await SeedApplicationsAsync();
        await SeedPageContentAsync();
    }

    private async Task SeedMessagesAsync()
    {
        if ((await _messages.GetAllAsync()).Count > 0) return;
        var now = DateTime.UtcNow;
        var items = new[]
        {
            new ContactMessage { Id = Guid.NewGuid(), Name = "Nguyễn Văn An", Email = "an.nguyen@example.com", Phone = "0901234567", Subject = "Tư vấn thiết kế căn hộ 80m²", Message = "Chào BMT Decor, mình muốn tư vấn thiết kế nội thất căn hộ 80m² ở Quận 7, phong cách hiện đại. Mong được báo giá.", IsRead = false, CreatedAt = now.AddHours(-2) },
            new ContactMessage { Id = Guid.NewGuid(), Name = "Trần Thị Bình", Email = "binh.tran@example.com", Phone = "0938111222", Subject = "Thi công showroom", Message = "Bên mình cần thi công showroom 150m² tại Bình Thạnh, mong được khảo sát trong tuần này.", IsRead = false, CreatedAt = now.AddDays(-1) },
            new ContactMessage { Id = Guid.NewGuid(), Name = "Lê Hoàng Cường", Email = "cuong.le@example.com", Phone = "0977333444", Subject = "Cải tạo văn phòng", Message = "Văn phòng công ty cần cải tạo lại, diện tích khoảng 200m². Vui lòng liên hệ lại giúp mình.", IsRead = true, CreatedAt = now.AddDays(-3) },
        };
        foreach (var m in items) await _messages.AddAsync(m);
    }

    private async Task SeedApplicationsAsync()
    {
        if ((await _applications.GetAllAsync()).Count > 0) return;
        var now = DateTime.UtcNow;
        // Placeholder CV link (temporary, like the sample images).
        const string cv = "https://thietkethicong.vn/wp-content/uploads/2024/08/IMG_4015.jpg";
        var items = new[]
        {
            new JobApplication { Id = Guid.NewGuid(), Name = "Phạm Minh Đức", Phone = "0901555666", Email = "duc.pham@example.com", Position = "Thực Tập Sinh Marketing", CoverLetter = "Em là sinh viên năm 3 ngành Marketing, đam mê social media và đã làm cộng tác viên content. Mong được thực tập tại BMT Decor.", CvUrl = cv, IsRead = false, CreatedAt = now.AddHours(-5) },
            new JobApplication { Id = Guid.NewGuid(), Name = "Võ Thị Em", Phone = "0912777888", Email = "em.vo@example.com", Position = "Thực Tập Sinh Thiết kế Nội/Ngoại thất", CoverLetter = "Em sử dụng tốt SketchUp, 3ds Max, AutoCAD. Gửi kèm portfolio trong CV ạ.", CvUrl = cv, IsRead = false, CreatedAt = now.AddDays(-2) },
            new JobApplication { Id = Guid.NewGuid(), Name = "Đỗ Quốc Huy", Phone = "0933999000", Email = "huy.do@example.com", Position = "Nhân viên Kinh doanh dự án", CoverLetter = "Em có 2 năm kinh nghiệm sales nội thất, mong gia nhập đội ngũ kinh doanh của BMT.", CvUrl = cv, IsRead = true, CreatedAt = now.AddDays(-4) },
        };
        foreach (var a in items) await _applications.AddAsync(a);
    }

    private async Task SeedAlbumsAsync()
    {
        if ((await _albums.GetAllAsync()).Count > 0) return;
        var data = ReadSeed<List<Album>>("albums.json");
        if (data is null) return;
        var seen = new HashSet<string>();
        foreach (var a in data)
        {
            if (string.IsNullOrWhiteSpace(a.Slug) || !seen.Add(a.Slug)) continue;
            a.Id = Guid.NewGuid();
            await _albums.AddAsync(a);
        }
    }

    private async Task SeedJobsAsync()
    {
        if ((await _jobs.GetAllAsync()).Count > 0) return;
        var data = ReadSeed<List<JobPosting>>("jobs.json");
        if (data is null) return;
        foreach (var j in data)
        {
            j.Id = Guid.NewGuid();
            await _jobs.AddAsync(j);
        }
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
            new() { Key = "home.studio.eyebrow", Page = "Trang chủ", Label = "Giới thiệu studio · Nhãn", Kind = "text", SortOrder = 3, Value = "Về BMT Decor" },
            new() { Key = "home.studio.statement", Page = "Trang chủ", Label = "Giới thiệu studio · Câu tuyên ngôn", Kind = "textarea", SortOrder = 4,
                Value = "Chúng tôi không chỉ xây dựng không gian. Chúng tôi kiến tạo trải nghiệm sống — nơi ánh sáng, vật liệu và tỷ lệ hòa quyện để biến điều thường nhật trở nên phi thường." },
            new() { Key = "home.studio.link", Page = "Trang chủ", Label = "Giới thiệu studio · Nhãn liên kết", Kind = "text", SortOrder = 5, Value = "Câu chuyện của chúng tôi" },
            new() { Key = "home.services.eyebrow", Page = "Trang chủ", Label = "Dịch vụ · Nhãn", Kind = "text", SortOrder = 6, Value = "Dịch vụ của chúng tôi" },
            new() { Key = "home.services.heading", Page = "Trang chủ", Label = "Dịch vụ · Tiêu đề", Kind = "textarea", SortOrder = 7, Value = "Một đơn vị, trọn vẹn hành trình." },
            new() { Key = "home.philosophy.eyebrow", Page = "Trang chủ", Label = "Triết lý · Nhãn", Kind = "text", SortOrder = 8, Value = "Giá trị cốt lõi" },
            new() { Key = "home.philosophy.heading", Page = "Trang chủ", Label = "Triết lý · Tiêu đề", Kind = "textarea", SortOrder = 9, Value = "Bốn cam kết trong mỗi không gian." },
            new() { Key = "home.philosophy.image", Page = "Trang chủ", Label = "Triết lý · Ảnh", Kind = "image", SortOrder = 10, Value = "1615873968403-89e068629265" },
            new() { Key = "home.beforeafter.eyebrow", Page = "Trang chủ", Label = "Before/After · Nhãn", Kind = "text", SortOrder = 11, Value = "Sự thay đổi" },
            new() { Key = "home.beforeafter.heading", Page = "Trang chủ", Label = "Before/After · Tiêu đề", Kind = "textarea", SortOrder = 12, Value = "Trước, sau, ngỡ ngàng." },
            new() { Key = "home.beforeafter.body", Page = "Trang chủ", Label = "Before/After · Mô tả", Kind = "textarea", SortOrder = 13,
                Value = "Kéo để thấy cách chúng tôi đổi mới một không gian — vẫn bốn bức tường ấy, một diện mạo hoàn toàn mới. Khoảnh khắc khách hàng không bao giờ quên." },
            new() { Key = "home.beforeafter.link", Page = "Trang chủ", Label = "Before/After · Nhãn liên kết", Kind = "text", SortOrder = 14, Value = "Tìm hiểu cải tạo" },
            new() { Key = "home.beforeafter.before", Page = "Trang chủ", Label = "Before/After · Ảnh trước", Kind = "image", SortOrder = 15, Value = "1497366216548-37526070297c" },
            new() { Key = "home.beforeafter.after", Page = "Trang chủ", Label = "Before/After · Ảnh sau", Kind = "image", SortOrder = 16, Value = "1600585154340-be6161a56a0c" },
            new() { Key = "home.story.eyebrow", Page = "Trang chủ", Label = "Câu chuyện cuộn · Nhãn", Kind = "text", SortOrder = 17, Value = "Giải phẫu một không gian" },
            new() { Key = "home.story.1.word", Page = "Trang chủ", Label = "Chương 1 · Tiêu đề", Kind = "text", SortOrder = 18, Value = "Ánh sáng" },
            new() { Key = "home.story.1.line", Page = "Trang chủ", Label = "Chương 1 · Mô tả", Kind = "textarea", SortOrder = 19, Value = "Chúng tôi bắt đầu từ ánh sáng. Nơi nắng lên, nơi nắng đọng lại — mọi thứ còn lại là chi tiết." },
            new() { Key = "home.story.1.img", Page = "Trang chủ", Label = "Chương 1 · Ảnh", Kind = "image", SortOrder = 20, Value = "1600585154340-be6161a56a0c" },
            new() { Key = "home.story.2.word", Page = "Trang chủ", Label = "Chương 2 · Tiêu đề", Kind = "text", SortOrder = 21, Value = "Vật liệu" },
            new() { Key = "home.story.2.line", Page = "Trang chủ", Label = "Chương 2 · Mô tả", Kind = "textarea", SortOrder = 22, Value = "Gỗ, đá, vữa, đồng. Những bề mặt được chọn không để phô trương, mà để đẹp dần theo thời gian." },
            new() { Key = "home.story.2.img", Page = "Trang chủ", Label = "Chương 2 · Ảnh", Kind = "image", SortOrder = 23, Value = "1615873968403-89e068629265" },
            new() { Key = "home.story.3.word", Page = "Trang chủ", Label = "Chương 3 · Tiêu đề", Kind = "text", SortOrder = 24, Value = "Không gian" },
            new() { Key = "home.story.3.line", Page = "Trang chủ", Label = "Chương 3 · Mô tả", Kind = "textarea", SortOrder = 25, Value = "Những căn phòng biết thở. Ngưỡng cửa biết dừng. Một tầm nhìn dẫn dắt bạn đi sâu vào trong." },
            new() { Key = "home.story.3.img", Page = "Trang chủ", Label = "Chương 3 · Ảnh", Kind = "image", SortOrder = 26, Value = "1600607687939-ce8a6c25118c" },
            new() { Key = "home.story.4.word", Page = "Trang chủ", Label = "Chương 4 · Tiêu đề", Kind = "text", SortOrder = 27, Value = "Tay nghề" },
            new() { Key = "home.story.4.line", Page = "Trang chủ", Label = "Chương 4 · Mô tả", Kind = "textarea", SortOrder = 28, Value = "Vẽ bằng tay và thi công bởi chính đội ngũ của chúng tôi — hoàn thiện tại công trình, tới từng milimét." },
            new() { Key = "home.story.4.img", Page = "Trang chủ", Label = "Chương 4 · Ảnh", Kind = "image", SortOrder = 29, Value = "1567767292278-a4f21aa2d36e" },
            new() { Key = "home.video.eyebrow", Page = "Trang chủ", Label = "Video · Nhãn", Kind = "text", SortOrder = 30, Value = "Bên trong xưởng" },
            new() { Key = "home.video.heading", Page = "Trang chủ", Label = "Video · Tiêu đề", Kind = "textarea", SortOrder = 31, Value = "Thước phim về cách một không gian thành hình." },
            new() { Key = "home.video.image", Page = "Trang chủ", Label = "Video · Ảnh nền", Kind = "image", SortOrder = 32, Value = "1600607687939-ce8a6c25118c" },
            new() { Key = "home.process.eyebrow", Page = "Trang chủ", Label = "Quy trình (tiêu đề) · Nhãn", Kind = "text", SortOrder = 33, Value = "Quy trình làm việc" },
            new() { Key = "home.process.heading", Page = "Trang chủ", Label = "Quy trình (tiêu đề) · Tiêu đề", Kind = "text", SortOrder = 34, Value = "Từ nét vẽ đầu tiên" },
            new() { Key = "home.process.headingAccent", Page = "Trang chủ", Label = "Quy trình (tiêu đề) · Tiêu đề (chữ vàng)", Kind = "text", SortOrder = 35, Value = "đến ngày trao chìa khóa." },

            // —— Giới thiệu ——
            new() { Key = "about.hero.eyebrow", Page = "Giới thiệu", Label = "Hero · Nhãn", Kind = "text", SortOrder = 10, Value = "Giới thiệu" },
            new() { Key = "about.hero.title", Page = "Giới thiệu", Label = "Hero · Tiêu đề (mỗi dòng một hàng)", Kind = "textarea", SortOrder = 11, Value = "Đơn vị thiết kế\nthi công nội thất\nchuyên nghiệp." },
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
            new() { Key = "about.commit.eyebrow", Page = "Giới thiệu", Label = "Cam kết (chi tiết) · Nhãn", Kind = "text", SortOrder = 20, Value = "Cam kết" },
            new() { Key = "about.commit.1.title", Page = "Giới thiệu", Label = "Cam kết (chi tiết) · Mục 1 — Tiêu đề", Kind = "text", SortOrder = 21, Value = "Thiết kế chuẩn xác" },
            new() { Key = "about.commit.1.body", Page = "Giới thiệu", Label = "Cam kết (chi tiết) · Mục 1 — Mô tả", Kind = "textarea", SortOrder = 22,
                Value = "Mỗi phương án được nghiên cứu kỹ lưỡng, tối ưu công năng và bám sát nhu cầu thực tế của khách hàng." },
            new() { Key = "about.commit.2.title", Page = "Giới thiệu", Label = "Cam kết (chi tiết) · Mục 2 — Tiêu đề", Kind = "text", SortOrder = 23, Value = "Báo giá minh bạch" },
            new() { Key = "about.commit.2.body", Page = "Giới thiệu", Label = "Cam kết (chi tiết) · Mục 2 — Mô tả", Kind = "textarea", SortOrder = 24,
                Value = "Chi phí được liệt kê rõ ràng theo từng hạng mục, hạn chế tối đa các khoản phát sinh ngoài thỏa thuận." },
            new() { Key = "about.commit.3.title", Page = "Giới thiệu", Label = "Cam kết (chi tiết) · Mục 3 — Tiêu đề", Kind = "text", SortOrder = 25, Value = "Thi công đúng tiến độ" },
            new() { Key = "about.commit.3.body", Page = "Giới thiệu", Label = "Cam kết (chi tiết) · Mục 3 — Mô tả", Kind = "textarea", SortOrder = 26,
                Value = "Lập kế hoạch thi công khoa học, đảm bảo hoàn thành và bàn giao đúng thời gian đã cam kết." },
            new() { Key = "about.commit.4.title", Page = "Giới thiệu", Label = "Cam kết (chi tiết) · Mục 4 — Tiêu đề", Kind = "text", SortOrder = 27, Value = "Bảo hành rõ ràng" },
            new() { Key = "about.commit.4.body", Page = "Giới thiệu", Label = "Cam kết (chi tiết) · Mục 4 — Mô tả", Kind = "textarea", SortOrder = 28,
                Value = "Chính sách bảo hành minh bạch, đồng hành cùng khách hàng sau bàn giao. Hỗ trợ bảo trì và xử lý các vấn đề kỹ thuật nhanh chóng, kịp thời." },
            new() { Key = "about.cta.eyebrow", Page = "Giới thiệu", Label = "Khối kêu gọi (CTA) · Nhãn", Kind = "text", SortOrder = 30, Value = "Hợp tác cùng chúng tôi" },
            new() { Key = "about.cta.title", Page = "Giới thiệu", Label = "Khối kêu gọi (CTA) · Tiêu đề (mỗi dòng một hàng)", Kind = "textarea", SortOrder = 31, Value = "Kể cho chúng tôi\ndự án của bạn." },
            new() { Key = "about.cta.image", Page = "Giới thiệu", Label = "Khối kêu gọi (CTA) · Ảnh nền", Kind = "image", SortOrder = 32, Value = "1600585152220-90363fe7e115" },

            // —— Liên hệ ——
            new() { Key = "contact.hero.intro", Page = "Liên hệ", Label = "Hero · Đoạn mở đầu", Kind = "textarea", SortOrder = 1,
                Value = "Hãy chia sẻ về không gian và mong muốn của bạn. Buổi tư vấn đầu tiên luôn miễn phí — và thường là khởi đầu của một công trình đáng nhớ." },

            // —— Báo giá ——
            new() { Key = "estimator.hero.eyebrow", Page = "Báo giá", Label = "Hero · Nhãn", Kind = "text", SortOrder = 10, Value = "Báo giá" },
            new() { Key = "estimator.hero.title", Page = "Báo giá", Label = "Hero · Tiêu đề (mỗi dòng một hàng)", Kind = "textarea", SortOrder = 11, Value = "Ngân sách,\nchỉ trong\nmột phút." },
            new() { Key = "estimator.hero.intro", Page = "Báo giá", Label = "Hero · Đoạn mở đầu", Kind = "textarea", SortOrder = 1,
                Value = "Bốn câu hỏi nhanh để có khoảng giá thực tế — sau đó để lại thông tin để nhận báo giá chi tiết theo từng hạng mục cho dự án của bạn." },
            // Estimator rate tables — edited via a dedicated admin page (kind "json"
            // is hidden from the generic page-content editor).
            new() { Key = "estimator.config", Page = "Báo giá", Label = "Bảng đơn giá ước tính", Kind = "json", SortOrder = 2,
                Value = @"{""propertyTypes"":[{""id"":""apartment"",""label"":""Căn hộ"",""mult"":1.0},{""id"":""townhouse"",""label"":""Nhà phố"",""mult"":1.1},{""id"":""villa"",""label"":""Biệt thự"",""mult"":1.25},{""id"":""office"",""label"":""Văn phòng"",""mult"":1.05},{""id"":""fnb"",""label"":""Café / Nhà hàng"",""mult"":1.35},{""id"":""retail"",""label"":""Showroom / Cửa hàng"",""mult"":1.2}],""packages"":[{""id"":""design"",""label"":""Chỉ thiết kế"",""rate"":350000,""blurb"":""Từ concept đến bản vẽ thi công.""},{""id"":""refresh"",""label"":""Cải tạo — Làm mới"",""rate"":3500000,""blurb"":""Nâng cấp & trang trí cơ bản.""},{""id"":""essential"",""label"":""Trọn gói — Tiêu chuẩn"",""rate"":8000000,""blurb"":""Hoàn thiện tiêu chuẩn, nội thất cơ bản.""},{""id"":""signature"",""label"":""Trọn gói — Cao cấp"",""rate"":13000000,""blurb"":""Thiết kế riêng, vật liệu cao cấp.""},{""id"":""atelier"",""label"":""Trọn gói — Đặc tuyển"",""rate"":20000000,""blurb"":""Sang trọng, may đo hoàn toàn.""}]}" },
            // —— Dịch vụ (trang list) ——
            new() { Key = "services.hero.eyebrow", Page = "Dịch vụ", Label = "Hero · Nhãn", Kind = "text", SortOrder = 1, Value = "Dịch vụ của chúng tôi" },
            new() { Key = "services.hero.title", Page = "Dịch vụ", Label = "Hero · Tiêu đề (mỗi dòng một hàng)", Kind = "textarea", SortOrder = 2, Value = "Một đơn vị,\ntrọn vẹn\nhành trình." },
            new() { Key = "services.hero.intro", Page = "Dịch vụ", Label = "Hero · Đoạn mở đầu", Kind = "textarea", SortOrder = 3, Value = "Bạn có thể chọn riêng một hạng mục hoặc trọn gói thiết kế – thi công. Phần lớn khách hàng chọn trọn gói: thiết kế và thi công cùng một đội ngũ, một ngân sách, một đầu mối chịu trách nhiệm." },

            // —— Dự án (trang list) ——
            new() { Key = "projects.hero.eyebrow", Page = "Dự án", Label = "Hero · Nhãn", Kind = "text", SortOrder = 1, Value = "Dự án tiêu biểu" },
            new() { Key = "projects.hero.title", Page = "Dự án", Label = "Hero · Tiêu đề (mỗi dòng một hàng)", Kind = "textarea", SortOrder = 2, Value = "Mỗi dự án\nlà một không gian\nđược kiến tạo." },

            // —— Tin tức (trang list) ——
            new() { Key = "blog.hero.eyebrow", Page = "Tin tức", Label = "Hero · Nhãn", Kind = "text", SortOrder = 1, Value = "Tin tức" },
            new() { Key = "blog.hero.title", Page = "Tin tức", Label = "Hero · Tiêu đề (mỗi dòng một hàng)", Kind = "textarea", SortOrder = 2, Value = "Tin tức\n& tuyển dụng." },

            // —— Tuyển dụng (trang list) ——
            new() { Key = "careers.hero.eyebrow", Page = "Tuyển dụng", Label = "Hero · Nhãn", Kind = "text", SortOrder = 1, Value = "Tuyển dụng" },
            new() { Key = "careers.hero.title", Page = "Tuyển dụng", Label = "Hero · Tiêu đề (mỗi dòng một hàng)", Kind = "textarea", SortOrder = 2, Value = "Gia nhập\nđội ngũ BMT Decor" },
            new() { Key = "careers.hero.intro", Page = "Tuyển dụng", Label = "Hero · Đoạn mở đầu", Kind = "textarea", SortOrder = 3, Value = "Chúng tôi luôn tìm kiếm những con người tài năng, tận tâm để cùng kiến tạo những không gian đáng nhớ. Khám phá các vị trí đang mở bên dưới." },
            new() { Key = "careers.hero.image", Page = "Tuyển dụng", Label = "Hero · Ảnh", Kind = "image", SortOrder = 4, Value = "1497215728101-856f4ea42174" },

            // —— Khối kêu gọi (CTA) dùng chung, hiện trong hub Trang chủ ——
            new() { Key = "cta.eyebrow", Page = "Trang chủ", Label = "Khối kêu gọi (CTA) · Nhãn", Kind = "text", SortOrder = 40, Value = "Bắt đầu dự án" },
            new() { Key = "cta.title", Page = "Trang chủ", Label = "Khối kêu gọi (CTA) · Tiêu đề (mỗi dòng một hàng)", Kind = "textarea", SortOrder = 41, Value = "Cùng BMT Decor kiến tạo\nkhông gian sống\nđáng nhớ" },
            new() { Key = "cta.image", Page = "Trang chủ", Label = "Khối kêu gọi (CTA) · Ảnh nền", Kind = "image", SortOrder = 42, Value = "1600585152220-90363fe7e115" },

            // —— Hồ sơ năng lực (trang portfolio) ——
            new() { Key = "portfolio.cover.brand", Page = "Hồ sơ năng lực", Label = "Bìa · Thương hiệu", Kind = "text", SortOrder = 1, Value = "BMT Studio" },
            new() { Key = "portfolio.cover.eyebrow", Page = "Hồ sơ năng lực", Label = "Bìa · Nhãn", Kind = "text", SortOrder = 2, Value = "Hồ sơ năng lực" },
            new() { Key = "portfolio.cover.title", Page = "Hồ sơ năng lực", Label = "Bìa · Tiêu đề lớn", Kind = "text", SortOrder = 3, Value = "Portfolio" },
            new() { Key = "portfolio.cover.desc", Page = "Hồ sơ năng lực", Label = "Bìa · Mô tả", Kind = "textarea", SortOrder = 4, Value = "Tuyển tập các công trình thiết kế & thi công đã hoàn thiện." },
            new() { Key = "portfolio.contents.eyebrow", Page = "Hồ sơ năng lực", Label = "Mục lục · Nhãn", Kind = "text", SortOrder = 5, Value = "Mục lục" },
            new() { Key = "portfolio.contents.heading", Page = "Hồ sơ năng lực", Label = "Mục lục · Tiêu đề", Kind = "text", SortOrder = 6, Value = "Các chương trong hồ sơ" },
            new() { Key = "portfolio.contents.hint", Page = "Hồ sơ năng lực", Label = "Mục lục · Ghi chú", Kind = "text", SortOrder = 7, Value = "Chọn một chương để lật xem." },
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
                WorkType = string.IsNullOrWhiteSpace(p.WorkType) ? "Thiết kế & Thi công" : p.WorkType,
                BeforeImage = p.BeforeImage,
                AfterImage = p.AfterImage,
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
        public string WorkType { get; set; } = "Thiết kế & Thi công";
        public string BeforeImage { get; set; } = "";
        public string AfterImage { get; set; } = "";
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

using Cms.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cms.Repository.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<ServiceItem> Services => Set<ServiceItem>();
    public DbSet<SiteSetting> SiteSettings => Set<SiteSetting>();
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
    public DbSet<Testimonial> Testimonials => Set<Testimonial>();
    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();
    public DbSet<ProcessStep> ProcessSteps => Set<ProcessStep>();
    public DbSet<StatItem> Stats => Set<StatItem>();
    public DbSet<Faq> Faqs => Set<Faq>();
    public DbSet<Partner> Partners => Set<Partner>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
            entity.Property(u => u.Role).IsRequired().HasMaxLength(20);
            entity.HasIndex(u => u.Username).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Slug).IsRequired().HasMaxLength(200);
            entity.HasIndex(p => p.Slug).IsUnique();
            entity.Property(p => p.Title).IsRequired().HasMaxLength(300);
            entity.Property(p => p.Category).IsRequired().HasMaxLength(100);
            entity.Property(p => p.Location).HasMaxLength(150);
            entity.Property(p => p.Subtitle).HasMaxLength(300);
            entity.Property(p => p.Area).HasMaxLength(100);
            entity.Property(p => p.Client).HasMaxLength(200);
            entity.Property(p => p.ContentJson).HasColumnType("jsonb");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Slug).IsRequired().HasMaxLength(200);
            entity.HasIndex(p => p.Slug).IsUnique();
            entity.Property(p => p.Title).IsRequired().HasMaxLength(300);
            entity.Property(p => p.Status).IsRequired().HasMaxLength(20);
        });

        modelBuilder.Entity<ServiceItem>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Slug).IsRequired().HasMaxLength(200);
            entity.HasIndex(s => s.Slug).IsUnique();
            entity.Property(s => s.Title).IsRequired().HasMaxLength(200);
            entity.Property(s => s.Icon).HasMaxLength(100);
            entity.Property(s => s.DetailJson).HasColumnType("jsonb");
        });

        modelBuilder.Entity<SiteSetting>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Name).HasMaxLength(200);
            entity.Property(s => s.LegalName).HasMaxLength(300);
            entity.Property(s => s.Phone).HasMaxLength(50);
            entity.Property(s => s.Email).HasMaxLength(256);
            entity.Property(s => s.TaxId).HasMaxLength(50);
            entity.Property(s => s.OfficesJson).HasColumnType("jsonb");
        });

        modelBuilder.Entity<ContactMessage>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Name).IsRequired().HasMaxLength(150);
            entity.Property(m => m.Email).IsRequired().HasMaxLength(256);
            entity.Property(m => m.Phone).HasMaxLength(50);
            entity.Property(m => m.Subject).HasMaxLength(200);
        });
    }
}

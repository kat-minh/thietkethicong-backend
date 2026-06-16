using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cms.Repository.Migrations
{
    /// <inheritdoc />
    public partial class FixSettingsAndReseedContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // A prior migration renamed Zalo→Manifesto, so existing rows have a URL
            // stuck in Manifesto and an empty social list. Repair the singleton.
            migrationBuilder.Sql(
                "UPDATE \"SiteSettings\" SET \"Manifesto\" = 'Chúng tôi không chỉ xây dựng không gian — chúng tôi kiến tạo trải nghiệm.' " +
                "WHERE \"Manifesto\" IS NULL OR \"Manifesto\" = '' OR \"Manifesto\" LIKE 'http%';");
            migrationBuilder.Sql(
                "UPDATE \"SiteSettings\" SET \"SocialJson\" = '[{\"label\":\"Facebook\",\"href\":\"https://facebook.com\"},{\"label\":\"Zalo\",\"href\":\"https://zalo.me/1255459439490998198\"}]' " +
                "WHERE \"SocialJson\" IS NULL OR \"SocialJson\"::text NOT LIKE '%href%';");

            // Clear the old thin demo content so the seeder repopulates Projects/Posts/
            // Services from the reduced fallback snapshots (SeedData/*.json) on this boot.
            migrationBuilder.Sql("DELETE FROM \"Projects\";");
            migrationBuilder.Sql("DELETE FROM \"Posts\";");
            migrationBuilder.Sql("DELETE FROM \"Services\";");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

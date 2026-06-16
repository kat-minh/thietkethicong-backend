using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cms.Repository.Migrations
{
    /// <inheritdoc />
    public partial class ReseedServicesFullData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Reseed so prod picks up the enriched sample data: services gain
            // styles/materials/packages/faqs; posts gain reading time; projects
            // gain before/after. Seeder repopulates from SeedData/*.json on boot.
            migrationBuilder.Sql("DELETE FROM \"Services\";");
            migrationBuilder.Sql("DELETE FROM \"Posts\";");
            migrationBuilder.Sql("DELETE FROM \"Projects\";");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

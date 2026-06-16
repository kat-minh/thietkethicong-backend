using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cms.Repository.Migrations
{
    /// <inheritdoc />
    public partial class ReseedProjectsFullData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Reseed projects so prod picks up the enriched sample data
            // (subtitle/area/client/before-after) from SeedData/projects.json.
            migrationBuilder.Sql("DELETE FROM \"Projects\";");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

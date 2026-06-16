using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cms.Repository.Migrations
{
    /// <inheritdoc />
    public partial class ReseedJobs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Clear ad-hoc/old job postings so the seeder populates the sample
            // [Mẫu] positions from SeedData/jobs.json on this boot.
            migrationBuilder.Sql("DELETE FROM \"JobPostings\";");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

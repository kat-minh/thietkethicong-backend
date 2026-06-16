using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cms.Repository.Migrations
{
    /// <inheritdoc />
    public partial class ProjectWorkTypeAndBeforeAfter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AfterImage",
                table: "Projects",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BeforeImage",
                table: "Projects",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WorkType",
                table: "Projects",
                type: "text",
                nullable: false,
                defaultValue: "Thiết kế & Thi công");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AfterImage",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "BeforeImage",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "WorkType",
                table: "Projects");
        }
    }
}

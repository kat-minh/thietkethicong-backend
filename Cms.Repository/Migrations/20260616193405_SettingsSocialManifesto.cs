using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cms.Repository.Migrations
{
    /// <inheritdoc />
    public partial class SettingsSocialManifesto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Facebook",
                table: "SiteSettings");

            migrationBuilder.RenameColumn(
                name: "Zalo",
                table: "SiteSettings",
                newName: "Manifesto");

            migrationBuilder.AddColumn<string>(
                name: "SocialJson",
                table: "SiteSettings",
                type: "jsonb",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SocialJson",
                table: "SiteSettings");

            migrationBuilder.RenameColumn(
                name: "Manifesto",
                table: "SiteSettings",
                newName: "Zalo");

            migrationBuilder.AddColumn<string>(
                name: "Facebook",
                table: "SiteSettings",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}

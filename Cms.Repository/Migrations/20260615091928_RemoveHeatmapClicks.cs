using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cms.Repository.Migrations
{
    /// <inheritdoc />
    public partial class RemoveHeatmapClicks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HeatmapClicks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HeatmapClicks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Path = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    ViewportWidth = table.Column<int>(type: "integer", nullable: false),
                    XRatio = table.Column<double>(type: "double precision", nullable: false),
                    YRatio = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeatmapClicks", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HeatmapClicks_CreatedAt",
                table: "HeatmapClicks",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_HeatmapClicks_Path",
                table: "HeatmapClicks",
                column: "Path");
        }
    }
}

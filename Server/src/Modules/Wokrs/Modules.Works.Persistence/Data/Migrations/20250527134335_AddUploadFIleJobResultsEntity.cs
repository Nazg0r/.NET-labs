using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Works.Persistence.src.Modules.Wokrs.Modules.Works.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUploadFIleJobResultsEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UploadFIleJobResults",
                schema: "works",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_id = table.Column<string>(type: "text", nullable: true),
                    work_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadFIleJobResults", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UploadFIleJobResults",
                schema: "works");
        }
    }
}
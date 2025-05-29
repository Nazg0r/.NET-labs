using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Works.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class InintialCreateWorkDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "works");

            migrationBuilder.CreateTable(
                name: "Works",
                schema: "works",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_name = table.Column<string>(type: "text", nullable: false),
                    load_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    extension = table.Column<string>(type: "text", nullable: false),
                    content = table.Column<byte[]>(type: "bytea", nullable: false),
                    student_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Works", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Works",
                schema: "works");
        }
    }
}
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _20 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeploymentLog",
                schema: "efavori");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "efavori");

            migrationBuilder.CreateTable(
                name: "DeploymentLog",
                schema: "efavori",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommitMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeployDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeployedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ProjectName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeploymentLog", x => x.Id);
                });
        }
    }
}

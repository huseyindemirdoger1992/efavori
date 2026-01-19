using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationMs",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "Referrer",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "RequestMethod",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "TraceId",
                table: "Logs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DurationMs",
                table: "Logs",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Referrer",
                table: "Logs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestMethod",
                table: "Logs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TraceId",
                table: "Logs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}

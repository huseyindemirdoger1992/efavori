using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _74 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileUrl_Ratio_1_16",
                table: "Media",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileUrl_Ratio_1_2",
                table: "Media",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileUrl_Ratio_1_4",
                table: "Media",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileUrl_Ratio_1_8",
                table: "Media",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileUrl_Ratio_1_16",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "FileUrl_Ratio_1_2",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "FileUrl_Ratio_1_4",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "FileUrl_Ratio_1_8",
                table: "Media");
        }
    }
}

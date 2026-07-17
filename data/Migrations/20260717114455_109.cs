using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _109 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AiAttributesIsOk",
                table: "CategoriesProduct",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "CategoriesProduct",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AiAttributesIsOk",
                table: "CategoriesProduct");

            migrationBuilder.DropColumn(
                name: "Path",
                table: "CategoriesProduct");
        }
    }
}

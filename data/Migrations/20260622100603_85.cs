using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _85 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "ProductReview");

            migrationBuilder.DropColumn(
                name: "AdminIsGavePermission",
                table: "ProductReview");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AdminId",
                table: "ProductReview",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AdminIsGavePermission",
                table: "ProductReview",
                type: "bit",
                nullable: true);
        }
    }
}

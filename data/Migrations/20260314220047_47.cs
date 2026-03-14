using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _47 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Interaction_BookmarkCount",
                table: "Product",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Interaction_LikeCount",
                table: "Product",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Interaction_ShareCount",
                table: "Product",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Interaction_ViewCount",
                table: "Product",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Meta_CanonicalUrl",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Meta_FocusKeywords",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Meta_MetaDescription",
                table: "Product",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Meta_MetaTitle",
                table: "Product",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Meta_OgType",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Meta_RobotsIndex",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Interaction_BookmarkCount",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Interaction_LikeCount",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Interaction_ShareCount",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Interaction_ViewCount",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Meta_CanonicalUrl",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Meta_FocusKeywords",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Meta_MetaDescription",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Meta_MetaTitle",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Meta_OgType",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Meta_RobotsIndex",
                table: "Product");
        }
    }
}

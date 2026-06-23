using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _87 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Interaction_AskSellerCount",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Interaction_CompareCount",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Interaction_FavoriteCount",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Interaction_NotifyPriceDropCount",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "InteractionCounts_AskSellerCount",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "InteractionCounts_CompareCount",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "InteractionCounts_FavoriteCount",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "InteractionCounts_NotifyPriceDropCount",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Interaction_AskSellerCount",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Interaction_CompareCount",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Interaction_FavoriteCount",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Interaction_NotifyPriceDropCount",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Interaction_AskSellerCount",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "Interaction_CompareCount",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "Interaction_FavoriteCount",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "Interaction_NotifyPriceDropCount",
                table: "Article");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Interaction_AskSellerCount",
                table: "Stores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Interaction_CompareCount",
                table: "Stores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Interaction_FavoriteCount",
                table: "Stores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Interaction_NotifyPriceDropCount",
                table: "Stores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InteractionCounts_AskSellerCount",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InteractionCounts_CompareCount",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InteractionCounts_FavoriteCount",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InteractionCounts_NotifyPriceDropCount",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Interaction_AskSellerCount",
                table: "Posts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Interaction_CompareCount",
                table: "Posts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Interaction_FavoriteCount",
                table: "Posts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Interaction_NotifyPriceDropCount",
                table: "Posts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Interaction_AskSellerCount",
                table: "Article",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Interaction_CompareCount",
                table: "Article",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Interaction_FavoriteCount",
                table: "Article",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Interaction_NotifyPriceDropCount",
                table: "Article",
                type: "int",
                nullable: true);
        }
    }
}

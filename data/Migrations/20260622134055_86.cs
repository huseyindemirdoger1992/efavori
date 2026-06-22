using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _86 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Interaction_ViewCount",
                table: "Products",
                newName: "InteractionCounts_ViewCount");

            migrationBuilder.RenameColumn(
                name: "Interaction_ShareCount",
                table: "Products",
                newName: "InteractionCounts_ShareCount");

            migrationBuilder.RenameColumn(
                name: "Interaction_RecommendCount",
                table: "Products",
                newName: "InteractionCounts_RecommendCount");

            migrationBuilder.RenameColumn(
                name: "Interaction_NotifyPriceDropCount",
                table: "Products",
                newName: "InteractionCounts_NotifyPriceDropCount");

            migrationBuilder.RenameColumn(
                name: "Interaction_FavoriteCount",
                table: "Products",
                newName: "InteractionCounts_FavoriteCount");

            migrationBuilder.RenameColumn(
                name: "Interaction_CompareCount",
                table: "Products",
                newName: "InteractionCounts_CompareCount");

            migrationBuilder.RenameColumn(
                name: "Interaction_AskSellerCount",
                table: "Products",
                newName: "InteractionCounts_AskSellerCount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InteractionCounts_ViewCount",
                table: "Products",
                newName: "Interaction_ViewCount");

            migrationBuilder.RenameColumn(
                name: "InteractionCounts_ShareCount",
                table: "Products",
                newName: "Interaction_ShareCount");

            migrationBuilder.RenameColumn(
                name: "InteractionCounts_RecommendCount",
                table: "Products",
                newName: "Interaction_RecommendCount");

            migrationBuilder.RenameColumn(
                name: "InteractionCounts_NotifyPriceDropCount",
                table: "Products",
                newName: "Interaction_NotifyPriceDropCount");

            migrationBuilder.RenameColumn(
                name: "InteractionCounts_FavoriteCount",
                table: "Products",
                newName: "Interaction_FavoriteCount");

            migrationBuilder.RenameColumn(
                name: "InteractionCounts_CompareCount",
                table: "Products",
                newName: "Interaction_CompareCount");

            migrationBuilder.RenameColumn(
                name: "InteractionCounts_AskSellerCount",
                table: "Products",
                newName: "Interaction_AskSellerCount");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _83 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Interaction_LikeCount",
                table: "Stores",
                newName: "Interaction_RecommendCount");

            migrationBuilder.RenameColumn(
                name: "Interaction_BookmarkCount",
                table: "Stores",
                newName: "Interaction_NotifyPriceDropCount");

            migrationBuilder.RenameColumn(
                name: "Interaction_LikeCount",
                table: "Products",
                newName: "Interaction_RecommendCount");

            migrationBuilder.RenameColumn(
                name: "Interaction_BookmarkCount",
                table: "Products",
                newName: "Interaction_NotifyPriceDropCount");

            migrationBuilder.RenameColumn(
                name: "Interaction_LikeCount",
                table: "Posts",
                newName: "Interaction_RecommendCount");

            migrationBuilder.RenameColumn(
                name: "Interaction_BookmarkCount",
                table: "Posts",
                newName: "Interaction_NotifyPriceDropCount");

            migrationBuilder.RenameColumn(
                name: "Interaction_LikeCount",
                table: "Article",
                newName: "Interaction_RecommendCount");

            migrationBuilder.RenameColumn(
                name: "Interaction_BookmarkCount",
                table: "Article",
                newName: "Interaction_NotifyPriceDropCount");

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
                name: "Interaction_AskSellerCount",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Interaction_CompareCount",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Interaction_FavoriteCount",
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

            migrationBuilder.AlterColumn<Guid>(
                name: "UserStoreId",
                table: "Article",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

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

            migrationBuilder.CreateTable(
                name: "ProductReview",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommentText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentReviewId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductReview", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductReview");

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
                name: "Interaction_AskSellerCount",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Interaction_CompareCount",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Interaction_FavoriteCount",
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
                name: "Interaction_AskSellerCount",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "Interaction_CompareCount",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "Interaction_FavoriteCount",
                table: "Article");

            migrationBuilder.RenameColumn(
                name: "Interaction_RecommendCount",
                table: "Stores",
                newName: "Interaction_LikeCount");

            migrationBuilder.RenameColumn(
                name: "Interaction_NotifyPriceDropCount",
                table: "Stores",
                newName: "Interaction_BookmarkCount");

            migrationBuilder.RenameColumn(
                name: "Interaction_RecommendCount",
                table: "Products",
                newName: "Interaction_LikeCount");

            migrationBuilder.RenameColumn(
                name: "Interaction_NotifyPriceDropCount",
                table: "Products",
                newName: "Interaction_BookmarkCount");

            migrationBuilder.RenameColumn(
                name: "Interaction_RecommendCount",
                table: "Posts",
                newName: "Interaction_LikeCount");

            migrationBuilder.RenameColumn(
                name: "Interaction_NotifyPriceDropCount",
                table: "Posts",
                newName: "Interaction_BookmarkCount");

            migrationBuilder.RenameColumn(
                name: "Interaction_RecommendCount",
                table: "Article",
                newName: "Interaction_LikeCount");

            migrationBuilder.RenameColumn(
                name: "Interaction_NotifyPriceDropCount",
                table: "Article",
                newName: "Interaction_BookmarkCount");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserStoreId",
                table: "Article",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}

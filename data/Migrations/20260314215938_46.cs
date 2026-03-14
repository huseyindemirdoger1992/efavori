using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _46 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Interaction_BookmarkCount",
                table: "Stores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Interaction_LikeCount",
                table: "Stores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Interaction_ShareCount",
                table: "Stores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Interaction_ViewCount",
                table: "Stores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Meta_CanonicalUrl",
                table: "Stores",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Meta_FocusKeywords",
                table: "Stores",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Meta_MetaDescription",
                table: "Stores",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Meta_MetaTitle",
                table: "Stores",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Meta_OgType",
                table: "Stores",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Meta_RobotsIndex",
                table: "Stores",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Article",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsUser = table.Column<bool>(type: "bit", nullable: false),
                    UserStoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeaturedImage = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Meta_MetaTitle = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: true),
                    Meta_MetaDescription = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: true),
                    Meta_FocusKeywords = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Meta_CanonicalUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Meta_OgType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Meta_RobotsIndex = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Interaction_ViewCount = table.Column<int>(type: "int", nullable: true),
                    Interaction_LikeCount = table.Column<int>(type: "int", nullable: true),
                    Interaction_ShareCount = table.Column<int>(type: "int", nullable: true),
                    Interaction_BookmarkCount = table.Column<int>(type: "int", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Article", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsUser = table.Column<bool>(type: "bit", nullable: false),
                    UserStoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Meta_MetaTitle = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: true),
                    Meta_MetaDescription = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: true),
                    Meta_FocusKeywords = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Meta_CanonicalUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Meta_OgType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Meta_RobotsIndex = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Interaction_ViewCount = table.Column<int>(type: "int", nullable: true),
                    Interaction_LikeCount = table.Column<int>(type: "int", nullable: true),
                    Interaction_ShareCount = table.Column<int>(type: "int", nullable: true),
                    Interaction_BookmarkCount = table.Column<int>(type: "int", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Article");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropColumn(
                name: "Interaction_BookmarkCount",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Interaction_LikeCount",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Interaction_ShareCount",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Interaction_ViewCount",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Meta_CanonicalUrl",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Meta_FocusKeywords",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Meta_MetaDescription",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Meta_MetaTitle",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Meta_OgType",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Meta_RobotsIndex",
                table: "Stores");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _58 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MetaDescription",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "MetaTitle",
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

            migrationBuilder.RenameColumn(
                name: "Meta_RobotsIndex",
                table: "Product",
                newName: "AiSeoKeywords");

            migrationBuilder.AddColumn<bool>(
                name: "AiSeoKeywordsIsOk",
                table: "Product",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StockQuantityForProduct",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuantityOfProductsPurchased = table.Column<double>(type: "float", nullable: false),
                    QuantityType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProcessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockQuantityForProduct", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockQuantityForProduct");

            migrationBuilder.DropColumn(
                name: "AiSeoKeywordsIsOk",
                table: "Product");

            migrationBuilder.RenameColumn(
                name: "AiSeoKeywords",
                table: "Product",
                newName: "Meta_RobotsIndex");

            migrationBuilder.AddColumn<string>(
                name: "MetaDescription",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaTitle",
                table: "Product",
                type: "nvarchar(max)",
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
        }
    }
}

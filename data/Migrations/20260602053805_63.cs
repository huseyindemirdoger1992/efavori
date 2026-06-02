using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _63 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pricing");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "ProductForStockQuantity");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pricing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiscountPriceAZN = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DiscountPriceEUR = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DiscountPriceTL = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DiscountPriceUSD = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PriceAZN = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PriceEUR = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PriceTL = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PriceTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PriceUSD = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pricing", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AiSeoKeywords = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AiSeoKeywordsIsOk = table.Column<bool>(type: "bit", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsFeatured = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductIsVariant = table.Column<bool>(type: "bit", nullable: true),
                    SKU = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UniqueNameSlug = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Interaction_BookmarkCount = table.Column<int>(type: "int", nullable: true),
                    Interaction_LikeCount = table.Column<int>(type: "int", nullable: true),
                    Interaction_ShareCount = table.Column<int>(type: "int", nullable: true),
                    Interaction_ViewCount = table.Column<int>(type: "int", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductForStockQuantity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProcessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuantityOfProductsPurchased = table.Column<double>(type: "float", nullable: false),
                    QuantityType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductForStockQuantity", x => x.Id);
                });
        }
    }
}

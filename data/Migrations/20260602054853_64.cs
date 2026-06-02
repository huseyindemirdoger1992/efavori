using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _64 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductAttributeCreateRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsVariantDefining = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttributeCreateRequest", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductAttributeValueCreateRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ColorHex = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttributeValueCreateRequest", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductCreateRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UniqueNameSlug = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AiSeoKeywords = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsFeatured = table.Column<bool>(type: "bit", nullable: false),
                    SpecificationsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductIsVariant = table.Column<bool>(type: "bit", nullable: false),
                    SKU = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Barcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCreateRequest", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductPricingCreateRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VariantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PriceTL = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountPriceTL = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PriceUSD = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountPriceUSD = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PriceEUR = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountPriceEUR = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PriceAZN = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountPriceAZN = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPricingCreateRequest", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductVariantCreateRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SKU = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Barcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GalleryJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    VariantTitle = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVariantCreateRequest", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductVariantValueCreateRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VariantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeValueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVariantValueCreateRequest", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductAttributeCreateRequest");

            migrationBuilder.DropTable(
                name: "ProductAttributeValueCreateRequest");

            migrationBuilder.DropTable(
                name: "ProductCreateRequest");

            migrationBuilder.DropTable(
                name: "ProductPricingCreateRequest");

            migrationBuilder.DropTable(
                name: "ProductVariantCreateRequest");

            migrationBuilder.DropTable(
                name: "ProductVariantValueCreateRequest");
        }
    }
}

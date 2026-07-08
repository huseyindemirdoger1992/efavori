using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _98 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FavoriteProductCart",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductVariantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductSlug = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductShortDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrandName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryName = table.Column<int>(type: "int", nullable: false),
                    SKU = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Barcode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SalePriceUsd = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountAmountUsd = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ShippingPriceUsd = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SalePriceTry = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountAmountTry = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ShippingPriceTry = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SalePriceEur = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountAmountEur = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ShippingPriceEur = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SalePriceAzn = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountAmountAzn = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ShippingPriceAzn = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CouponCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VatRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Desi = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DeliveryTimeText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerNote = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteProductCart", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavoriteProductCart");
        }
    }
}

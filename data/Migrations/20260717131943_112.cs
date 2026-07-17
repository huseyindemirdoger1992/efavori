using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _112 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartsProduct");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CartsProduct",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductSlug = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductVariantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    ProductSnapshot_Barcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductSnapshot_BrandName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductSnapshot_CategoryName = table.Column<int>(type: "int", nullable: true),
                    ProductSnapshot_CouponCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductSnapshot_CustomerNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductSnapshot_DeliveryTimeText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductSnapshot_Desi = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProductSnapshot_DiscountAmountAzn = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProductSnapshot_DiscountAmountEur = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProductSnapshot_DiscountAmountTry = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProductSnapshot_DiscountAmountUsd = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProductSnapshot_ProductImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductSnapshot_ProductShortDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductSnapshot_Quantity = table.Column<int>(type: "int", nullable: true),
                    ProductSnapshot_SKU = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductSnapshot_SalePriceAzn = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProductSnapshot_SalePriceEur = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProductSnapshot_SalePriceTry = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProductSnapshot_SalePriceUsd = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProductSnapshot_ShippingPriceAzn = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProductSnapshot_ShippingPriceEur = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProductSnapshot_ShippingPriceTry = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProductSnapshot_ShippingPriceUsd = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProductSnapshot_VatRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProductSnapshot_Weight = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartsProduct", x => x.Id);
                });
        }
    }
}

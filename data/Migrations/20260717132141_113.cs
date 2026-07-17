using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _113 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoriesProduct");

            migrationBuilder.CreateTable(
                name: "CartsProduct",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductVariantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductSlug = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductSnapshot_ProductShortDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductSnapshot_ProductImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductSnapshot_BrandName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductSnapshot_CategoryName = table.Column<int>(type: "int", nullable: true),
                    ProductSnapshot_SKU = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductSnapshot_Barcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductSnapshot_SalePriceUsd = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProductSnapshot_DiscountAmountUsd = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProductSnapshot_ShippingPriceUsd = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProductSnapshot_SalePriceTry = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProductSnapshot_DiscountAmountTry = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProductSnapshot_ShippingPriceTry = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProductSnapshot_SalePriceEur = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProductSnapshot_DiscountAmountEur = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProductSnapshot_ShippingPriceEur = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProductSnapshot_SalePriceAzn = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProductSnapshot_DiscountAmountAzn = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProductSnapshot_ShippingPriceAzn = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProductSnapshot_Quantity = table.Column<int>(type: "int", nullable: true),
                    ProductSnapshot_CouponCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductSnapshot_VatRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProductSnapshot_Weight = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProductSnapshot_Desi = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProductSnapshot_DeliveryTimeText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductSnapshot_CustomerNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartsProduct", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartsProduct");

            migrationBuilder.CreateTable(
                name: "CategoriesProduct",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AiAttributesIsOk = table.Column<bool>(type: "bit", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShowInMenu = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Categories_NameAz = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_NameDe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_NameEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_NameEs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_NameFr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_NameHi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_NamePt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_NameRu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_NameTr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_NameZh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_SlugAz = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_SlugDe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_SlugEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_SlugEs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_SlugFr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_SlugHi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_SlugPt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_SlugRu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_SlugTr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_SlugZh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriesProduct", x => x.Id);
                });
        }
    }
}

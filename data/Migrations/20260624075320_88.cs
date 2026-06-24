using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _88 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArticlesCategoriesAz",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticlesCategoriesAz", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArticlesCategoriesDe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticlesCategoriesDe", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArticlesCategoriesEn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticlesCategoriesEn", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArticlesCategoriesEs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticlesCategoriesEs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArticlesCategoriesFr",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticlesCategoriesFr", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArticlesCategoriesHi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticlesCategoriesHi", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArticlesCategoriesPt",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticlesCategoriesPt", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArticlesCategoriesRu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticlesCategoriesRu", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArticlesCategoriesTr",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticlesCategoriesTr", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArticlesCategoriesZh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticlesCategoriesZh", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductCart",
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
                    table.PrimaryKey("PK_ProductCart", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticlesCategoriesAz");

            migrationBuilder.DropTable(
                name: "ArticlesCategoriesDe");

            migrationBuilder.DropTable(
                name: "ArticlesCategoriesEn");

            migrationBuilder.DropTable(
                name: "ArticlesCategoriesEs");

            migrationBuilder.DropTable(
                name: "ArticlesCategoriesFr");

            migrationBuilder.DropTable(
                name: "ArticlesCategoriesHi");

            migrationBuilder.DropTable(
                name: "ArticlesCategoriesPt");

            migrationBuilder.DropTable(
                name: "ArticlesCategoriesRu");

            migrationBuilder.DropTable(
                name: "ArticlesCategoriesTr");

            migrationBuilder.DropTable(
                name: "ArticlesCategoriesZh");

            migrationBuilder.DropTable(
                name: "ProductCart");
        }
    }
}

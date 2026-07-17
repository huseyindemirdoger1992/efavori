using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _106 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "AiTitlesForArticle");

            migrationBuilder.DropTable(
                name: "Article");

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
                name: "AttributeTemplateCategories");

            migrationBuilder.DropTable(
                name: "AttributeTemplateItems");

            migrationBuilder.DropTable(
                name: "AttributeTemplates");

            migrationBuilder.DropTable(
                name: "CategoriesAz");

            migrationBuilder.DropTable(
                name: "CategoriesDe");

            migrationBuilder.DropTable(
                name: "CategoriesEn");

            migrationBuilder.DropTable(
                name: "CategoriesEs");

            migrationBuilder.DropTable(
                name: "CategoriesFr");

            migrationBuilder.DropTable(
                name: "CategoriesHi");

            migrationBuilder.DropTable(
                name: "CategoriesPt");

            migrationBuilder.DropTable(
                name: "CategoriesRu");

            migrationBuilder.DropTable(
                name: "CategoriesTr");

            migrationBuilder.DropTable(
                name: "CategoriesZh");

            migrationBuilder.DropTable(
                name: "FavoriteProductCart");

            migrationBuilder.DropTable(
                name: "ItemGallery");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropTable(
                name: "MarketplaceAttributeMappings");

            migrationBuilder.DropTable(
                name: "MarketplaceCategoryMappings");

            migrationBuilder.DropTable(
                name: "Marketplaces");

            migrationBuilder.DropTable(
                name: "ProductAttributeMappings");

            migrationBuilder.DropTable(
                name: "ProductAttributes");

            migrationBuilder.DropTable(
                name: "ProductAttributeValues");

            migrationBuilder.DropTable(
                name: "ProductBundleItems");

            migrationBuilder.DropTable(
                name: "ProductCart");

            migrationBuilder.DropTable(
                name: "ProductCategories");

            migrationBuilder.DropTable(
                name: "ProductDigitalAssets");

            migrationBuilder.DropTable(
                name: "ProductExternalMedias");

            migrationBuilder.DropTable(
                name: "ProductHistory");

            migrationBuilder.DropTable(
                name: "ProductImageVariantGroups");

            migrationBuilder.DropTable(
                name: "ProductImportJobs");

            migrationBuilder.DropTable(
                name: "ProductImportMappings");

            migrationBuilder.DropTable(
                name: "ProductImportProfiles");

            migrationBuilder.DropTable(
                name: "ProductImportRows");

            migrationBuilder.DropTable(
                name: "ProductMarketplaceListings");

            migrationBuilder.DropTable(
                name: "ProductPrices");

            migrationBuilder.DropTable(
                name: "ProductReview");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "ProductSeo");

            migrationBuilder.DropTable(
                name: "ProductSpecifications");

            migrationBuilder.DropTable(
                name: "ProductStocks");

            migrationBuilder.DropTable(
                name: "ProductTranslations");

            migrationBuilder.DropTable(
                name: "ProductVariants");

            migrationBuilder.DropTable(
                name: "ProductVariantValues");

            migrationBuilder.DropTable(
                name: "Stores");

            migrationBuilder.DropTable(
                name: "UserAddressMethod");

            migrationBuilder.DropTable(
                name: "UserPaymentMethod");

            migrationBuilder.DropTable(
                name: "UsersRoles");

            migrationBuilder.DropTable(
                name: "Warehouse");

            migrationBuilder.DropTable(
                name: "WorkstationEmployeeGroup");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "UserShortcuts");

            migrationBuilder.RenameColumn(
                name: "IsDeletedStatu",
                table: "Media",
                newName: "IsDeleted_IsDeletedStatu");

            migrationBuilder.RenameColumn(
                name: "DeletedAtDate",
                table: "Media",
                newName: "IsDeleted_DeletedAtDate");

            migrationBuilder.RenameColumn(
                name: "IsDelete",
                table: "MainCssJs",
                newName: "IsDeleted_IsDeletedStatu");

            migrationBuilder.AddColumn<DateTime>(
                name: "IsDeleted_DeletedAtDate",
                table: "UserShortcuts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted_IsDeletedStatu",
                table: "UserShortcuts",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AllowComments",
                table: "Posts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CoverImage",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "Posts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "Posts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishDate",
                table: "Posts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Posts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "IsDeleted_DeletedAtDate",
                table: "MainCssJs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoriesArticleId = table.Column<int>(type: "int", nullable: false),
                    IsUser = table.Column<bool>(type: "bit", nullable: false),
                    UserStoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CoverImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublishDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    IsFeatured = table.Column<bool>(type: "bit", nullable: false),
                    AllowComments = table.Column<bool>(type: "bit", nullable: false),
                    Meta_MetaTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Meta_MetaDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Meta_FocusKeywords = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Meta_CanonicalUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Meta_OgType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Meta_RobotsIndex = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Interaction_ViewCount = table.Column<int>(type: "int", nullable: true),
                    Interaction_ShareCount = table.Column<int>(type: "int", nullable: true),
                    Interaction_RecommendCount = table.Column<int>(type: "int", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CartsFavorite",
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
                    table.PrimaryKey("PK_CartsFavorite", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "CategoriesArticle",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ShowInMenu = table.Column<bool>(type: "bit", nullable: false),
                    Categories_NameTr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_NameEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_NameAz = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_NameDe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_NameEs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_NameFr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_NameHi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_NamePt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_NameRu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_NameZh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_SlugTr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_SlugEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_SlugAz = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_SlugDe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_SlugEs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_SlugFr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_SlugHi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_SlugPt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_SlugRu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_SlugZh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriesArticle", x => x.Id);
                });

            //migrationBuilder.CreateTable(
            //    name: "CategoriesProduct",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        ParentId = table.Column<int>(type: "int", nullable: true),
            //        IsActive = table.Column<bool>(type: "bit", nullable: false),
            //        ShowInMenu = table.Column<bool>(type: "bit", nullable: false),
            //        Categories_NameTr = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Categories_NameEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Categories_NameAz = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Categories_NameDe = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Categories_NameEs = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Categories_NameFr = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Categories_NameHi = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Categories_NamePt = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Categories_NameRu = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Categories_NameZh = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Categories_SlugTr = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Categories_SlugEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Categories_SlugAz = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Categories_SlugDe = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Categories_SlugEs = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Categories_SlugFr = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Categories_SlugHi = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Categories_SlugPt = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Categories_SlugRu = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Categories_SlugZh = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
            //        IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_CategoriesProduct", x => x.Id);
            //    });

            migrationBuilder.CreateTable(
                name: "MediaItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ItemType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MediaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ItemAddDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Store",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActiveStateAdmin = table.Column<bool>(type: "bit", nullable: true),
                    IsActiveDateAdmin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActiveStateVendor = table.Column<bool>(type: "bit", nullable: true),
                    IsActiveDateVendor = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ContactInformation_IsActiveEmail = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_PhoneEmailConfirmed = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActivePhoneNumber = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_CountryPhoneCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_FullPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActiveFacebook = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_Facebook = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_IsActiveInstagram = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_Instagram = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_IsActiveX = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_X = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_IsActiveTikTok = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_TikTok = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_IsActiveYouTube = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_YouTube = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_IsActiveLinkedin = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_Linkedin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_IsActiveWhatsApp = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_WhatsApp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_IsActiveTelegram = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_Telegram = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_IsActiveWeChat = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_WeChat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_IsActiveWeibo = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_Weibo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_IsActiveVKontakte = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_VKontakte = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_IsActiveLine = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_Line = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_IsActiveKakaoTalk = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_KakaoTalk = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_IsActivePinterest = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_Pinterest = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_IsActiveGitHub = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_GitHub = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_IsActiveBehance = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_Behance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_IsActiveDiscord = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_Discord = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_IsActiveReddit = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_Reddit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_IsActiveUserWebSite = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_UserWebSite = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfileCoverGallery_ProfileImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfileCoverGallery_CoverImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_MapTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_Latitude = table.Column<decimal>(type: "decimal(18,15)", nullable: true),
                    AddressInfo_Longitude = table.Column<decimal>(type: "decimal(18,15)", nullable: true),
                    AddressInfo_GoogleMyBusinessAccountLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkingHours_IsActiveMonday = table.Column<bool>(type: "bit", nullable: true),
                    WorkingHours_StartTimeMonday = table.Column<TimeOnly>(type: "time", nullable: true),
                    WorkingHours_FinishTimeMonday = table.Column<TimeOnly>(type: "time", nullable: true),
                    WorkingHours_IsActiveTuesday = table.Column<bool>(type: "bit", nullable: true),
                    WorkingHours_StartTimeTuesday = table.Column<TimeOnly>(type: "time", nullable: true),
                    WorkingHours_FinishTimeTuesday = table.Column<TimeOnly>(type: "time", nullable: true),
                    WorkingHours_IsActiveWednesday = table.Column<bool>(type: "bit", nullable: true),
                    WorkingHours_StartTimeWednesday = table.Column<TimeOnly>(type: "time", nullable: true),
                    WorkingHours_FinishTimeWednesday = table.Column<TimeOnly>(type: "time", nullable: true),
                    WorkingHours_IsActiveThursday = table.Column<bool>(type: "bit", nullable: true),
                    WorkingHours_StartTimeThursday = table.Column<TimeOnly>(type: "time", nullable: true),
                    WorkingHours_FinishTimeThursday = table.Column<TimeOnly>(type: "time", nullable: true),
                    WorkingHours_IsActiveFriday = table.Column<bool>(type: "bit", nullable: true),
                    WorkingHours_StartTimeFriday = table.Column<TimeOnly>(type: "time", nullable: true),
                    WorkingHours_FinishTimeFriday = table.Column<TimeOnly>(type: "time", nullable: true),
                    WorkingHours_IsActiveSaturday = table.Column<bool>(type: "bit", nullable: true),
                    WorkingHours_StartTimeSaturday = table.Column<TimeOnly>(type: "time", nullable: true),
                    WorkingHours_FinishTimeSaturday = table.Column<TimeOnly>(type: "time", nullable: true),
                    WorkingHours_IsActiveSunday = table.Column<bool>(type: "bit", nullable: true),
                    WorkingHours_StartTimeSunday = table.Column<TimeOnly>(type: "time", nullable: true),
                    WorkingHours_FinishTimeSunday = table.Column<TimeOnly>(type: "time", nullable: true),
                    CertificateOfIncorporation = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ActivityCertificate = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TaxRegistration = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TradeRegistryGazette = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SignatureCircular = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AuthorizedPersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProofOfBusinessAddress = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BankStatement = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BankAccountConfirmation = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TrademarkCertificate = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LetterOfAuthorization = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    QualityCertificates = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CustomsRegistration = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SocialSecurityRegistration = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProofOfOwnership = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Meta_MetaTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Meta_MetaDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Meta_FocusKeywords = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Meta_CanonicalUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Meta_OgType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Meta_RobotsIndex = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Interaction_ViewCount = table.Column<int>(type: "int", nullable: true),
                    Interaction_ShareCount = table.Column<int>(type: "int", nullable: true),
                    Interaction_RecommendCount = table.Column<int>(type: "int", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Store", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SupportTickets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserResponse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AdminResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssignedAdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportTickets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAddress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AddressName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddressType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeliveryReceiverFirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeliveryReceiverLastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeliveryReceiverEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeliveryReceiverPhoneCountryCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeliveryReceiverPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cities = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAddress", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserPayment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CardAlias = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CardHolderName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CardNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpirationDateMonth = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpirationDateYear = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CardAssociation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPayment", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "CartsFavorite");

            migrationBuilder.DropTable(
                name: "CartsProduct");

            migrationBuilder.DropTable(
                name: "CategoriesArticle");

            migrationBuilder.DropTable(
                name: "CategoriesProduct");

            migrationBuilder.DropTable(
                name: "MediaItems");

            migrationBuilder.DropTable(
                name: "Store");

            migrationBuilder.DropTable(
                name: "SupportTickets");

            migrationBuilder.DropTable(
                name: "UserAddress");

            migrationBuilder.DropTable(
                name: "UserPayment");

            migrationBuilder.DropColumn(
                name: "IsDeleted_DeletedAtDate",
                table: "UserShortcuts");

            migrationBuilder.DropColumn(
                name: "IsDeleted_IsDeletedStatu",
                table: "UserShortcuts");

            migrationBuilder.DropColumn(
                name: "AllowComments",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "CoverImage",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "PublishDate",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "IsDeleted_DeletedAtDate",
                table: "MainCssJs");

            migrationBuilder.RenameColumn(
                name: "IsDeleted_IsDeletedStatu",
                table: "Media",
                newName: "IsDeletedStatu");

            migrationBuilder.RenameColumn(
                name: "IsDeleted_DeletedAtDate",
                table: "Media",
                newName: "DeletedAtDate");

            migrationBuilder.RenameColumn(
                name: "IsDeleted_IsDeletedStatu",
                table: "MainCssJs",
                newName: "IsDelete");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "UserShortcuts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AddressInfo_Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_GoogleMyBusinessAccountLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_Latitude = table.Column<decimal>(type: "decimal(18,15)", nullable: true),
                    AddressInfo_Longitude = table.Column<decimal>(type: "decimal(18,15)", nullable: true),
                    AddressInfo_MapTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AiTitlesForArticle",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AiErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AiIsOk = table.Column<bool>(type: "bit", nullable: true),
                    AiProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AiRetryCount = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiTitlesForArticle", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Article",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArticleLognDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoriId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FeaturedImage = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ShotDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Slug = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SourceAiTitleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserStoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Interaction_RecommendCount = table.Column<int>(type: "int", nullable: true),
                    Interaction_ShareCount = table.Column<int>(type: "int", nullable: true),
                    Interaction_ViewCount = table.Column<int>(type: "int", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    Meta_CanonicalUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Meta_FocusKeywords = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Meta_MetaDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Meta_MetaTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Meta_OgType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Meta_RobotsIndex = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Article", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArticlesCategoriesAz",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDelete = table.Column<bool>(type: "bit", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true)
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
                    IsDelete = table.Column<bool>(type: "bit", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true)
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
                    IsDelete = table.Column<bool>(type: "bit", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true)
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
                    IsDelete = table.Column<bool>(type: "bit", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true)
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
                    IsDelete = table.Column<bool>(type: "bit", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true)
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
                    IsDelete = table.Column<bool>(type: "bit", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true)
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
                    IsDelete = table.Column<bool>(type: "bit", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true)
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
                    IsDelete = table.Column<bool>(type: "bit", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true)
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
                    IsDelete = table.Column<bool>(type: "bit", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true)
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
                    IsDelete = table.Column<bool>(type: "bit", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticlesCategoriesZh", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttributeTemplateCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TemplateGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeTemplateCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttributeTemplateItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    IsVariantSuggested = table.Column<bool>(type: "bit", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeTemplateItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttributeTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TemplateGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    VersionNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoriesAz",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExternalId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriesAz", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoriesDe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExternalId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriesDe", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoriesEn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExternalId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriesEn", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoriesEs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExternalId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriesEs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoriesFr",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExternalId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriesFr", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoriesHi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExternalId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriesHi", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoriesPt",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExternalId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriesPt", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoriesRu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExternalId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriesRu", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoriesTr",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExternalId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriesTr", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoriesZh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExternalId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriesZh", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FavoriteProductCart",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Barcode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrandName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryName = table.Column<int>(type: "int", nullable: false),
                    CouponCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerNote = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeliveryTimeText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Desi = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DiscountAmountAzn = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountAmountEur = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountAmountTry = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountAmountUsd = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductShortDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductSlug = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductVariantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    SKU = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SalePriceAzn = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SalePriceEur = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SalePriceTry = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SalePriceUsd = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ShippingPriceAzn = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ShippingPriceEur = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ShippingPriceTry = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ShippingPriceUsd = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VatRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteProductCart", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItemGallery",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: true),
                    ItemAddDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ItemType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MediaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemGallery", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Iso2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Iso3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NativeName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarketplaceAttributeMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExternalAttributeCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExternalAttributeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MarketplaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ValueMappingJson = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketplaceAttributeMappings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarketplaceCategoryMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExternalCategoryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExternalCategoryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MarketplaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketplaceCategoryMappings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Marketplaces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApiBaseUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LogoMediaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marketplaces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductAttributeMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsImageVariantAttribute = table.Column<bool>(type: "bit", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    IsVariantAttribute = table.Column<bool>(type: "bit", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttributeMappings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductAttributes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    InputType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsFilterable = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttributes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductAttributeValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ColorHex = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttributeValues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductBundleItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BundleProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChildProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChildVariantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DiscountRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductBundleItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductCart",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Barcode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrandName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryName = table.Column<int>(type: "int", nullable: false),
                    CouponCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerNote = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeliveryTimeText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Desi = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DiscountAmountAzn = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountAmountEur = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountAmountTry = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountAmountUsd = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductShortDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductSlug = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductVariantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    SKU = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SalePriceAzn = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SalePriceEur = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SalePriceTry = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SalePriceUsd = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ShippingPriceAzn = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ShippingPriceEur = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ShippingPriceTry = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ShippingPriceUsd = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VatRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCart", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductDigitalAssets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DownloadLimit = table.Column<int>(type: "int", nullable: true),
                    ExpirationDays = table.Column<int>(type: "int", nullable: true),
                    MediaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequiresLicenseKey = table.Column<bool>(type: "bit", nullable: false),
                    VariantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductDigitalAssets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductExternalMedias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    MediaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MediaType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductExternalMedias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductSlug = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductImageVariantGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeValueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CoverMediaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImageVariantGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductImportJobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnalysisReportJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BatchSize = table.Column<int>(type: "int", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DefaultCategoryId = table.Column<int>(type: "int", nullable: true),
                    DefaultCurrency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultWarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Delimiter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Encoding = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FailedCount = table.Column<int>(type: "int", nullable: false),
                    FieldMappingJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImportedCount = table.Column<int>(type: "int", nullable: false),
                    LastErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastProcessedRowIndex = table.Column<int>(type: "int", nullable: false),
                    LastRunAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SkippedCount = table.Column<int>(type: "int", nullable: false),
                    SourceMediaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StrategyReportJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SuccessRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCount = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImportJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductImportMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConfidenceLevel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConfidenceScore = table.Column<int>(type: "int", nullable: false),
                    CreateIfMissing = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsConfirmedByUser = table.Column<bool>(type: "bit", nullable: false),
                    JobId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MappingType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceItemCount = table.Column<int>(type: "int", nullable: true),
                    SourceKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceKeyHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceParentKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SuggestionsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TargetDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TargetType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TargetValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImportMappings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductImportProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DefaultCategoryId = table.Column<int>(type: "int", nullable: true),
                    DefaultWarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Delimiter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Encoding = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FieldMappingJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastRunDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImportProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductImportRows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedVariantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ErrorCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentExternalId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RawRowJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceExternalId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceProductType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceRowIndex = table.Column<int>(type: "int", nullable: false),
                    SourceSku = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarningsJson = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImportRows", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductMarketplaceListings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExternalProductId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExternalSku = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExternalUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastSyncDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MarketplaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RawSourceData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SyncStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VariantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductMarketplaceListings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductPrices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CostPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiscountedPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VariantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPrices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductReview",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AIControlDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CommentText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConfirmedByAi = table.Column<bool>(type: "bit", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ParentReviewId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WhyDidAiNotApproveIt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductReview", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AiContentStatus = table.Column<bool>(type: "bit", nullable: true),
                    AiErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AiOriginalFullDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AiOriginalName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AiOriginalShortDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AiOriginalTags = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AiProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AiRetryCount = table.Column<int>(type: "int", nullable: false),
                    ApprovedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AttributeTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BrandId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CoverMediaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExternalButtonText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExternalUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsAiManaged = table.Column<bool>(type: "bit", nullable: true),
                    IsApprovedByAdmin = table.Column<bool>(type: "bit", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PublishStatus = table.Column<bool>(type: "bit", nullable: true),
                    ShippingProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ShortDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InteractionCounts_RecommendCount = table.Column<int>(type: "int", nullable: true),
                    InteractionCounts_ShareCount = table.Column<int>(type: "int", nullable: true),
                    InteractionCounts_ViewCount = table.Column<int>(type: "int", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    Meta_CanonicalUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Meta_FocusKeywords = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Meta_MetaDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Meta_MetaTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Meta_OgType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Meta_RobotsIndex = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductSeo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CanonicalUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LanguageCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MetaJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SeoDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SeoKeywords = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SeoTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Slug = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSeo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductSpecifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AttributeValueId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CustomName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSpecifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductStocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CriticalStockLevel = table.Column<int>(type: "int", nullable: true),
                    MaxOrderQuantity = table.Column<int>(type: "int", nullable: true),
                    MinStockLevel = table.Column<int>(type: "int", nullable: true),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    StockStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrackStock = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VariantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductStocks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AiErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AiProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AiRetryCount = table.Column<int>(type: "int", nullable: false),
                    AiTranslationStatus = table.Column<bool>(type: "bit", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FullDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAiTranslated = table.Column<bool>(type: "bit", nullable: true),
                    IsManuallyEdited = table.Column<bool>(type: "bit", nullable: false),
                    LanguageCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShortDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTranslations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductVariants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Barcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    Ean = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gtin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HeightCm = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    Isbn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LengthCm = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Mpn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Sku = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Upc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WeightKg = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    WidthCm = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVariants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductVariantValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeValueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VariantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVariantValues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityCertificate = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AuthorizedPersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BankAccountConfirmation = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BankStatement = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CertificateOfIncorporation = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CustomsRegistration = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActiveDateAdmin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActiveDateVendor = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActiveStateAdmin = table.Column<bool>(type: "bit", nullable: true),
                    IsActiveStateVendor = table.Column<bool>(type: "bit", nullable: true),
                    LetterOfAuthorization = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProofOfBusinessAddress = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProofOfOwnership = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    QualityCertificates = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SignatureCircular = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SocialSecurityRegistration = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TaxRegistration = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TradeRegistryGazette = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TrademarkCertificate = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AddressInfo_Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_GoogleMyBusinessAccountLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_Latitude = table.Column<decimal>(type: "decimal(18,15)", nullable: true),
                    AddressInfo_Longitude = table.Column<decimal>(type: "decimal(18,15)", nullable: true),
                    AddressInfo_MapTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_Behance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_CountryPhoneCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_Discord = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_Facebook = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_FullPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_GitHub = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_Instagram = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_IsActiveBehance = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActiveDiscord = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActiveEmail = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActiveFacebook = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActiveGitHub = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActiveInstagram = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActiveKakaoTalk = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActiveLine = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActiveLinkedin = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActivePhoneNumber = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActivePinterest = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActiveReddit = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActiveTelegram = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActiveTikTok = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActiveUserWebSite = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActiveVKontakte = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActiveWeChat = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActiveWeibo = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActiveWhatsApp = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActiveX = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActiveYouTube = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_KakaoTalk = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_Line = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_Linkedin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_PhoneEmailConfirmed = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_Pinterest = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_Reddit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_Telegram = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_TikTok = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_UserWebSite = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_VKontakte = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_WeChat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_Weibo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_WhatsApp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_X = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_YouTube = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Interaction_RecommendCount = table.Column<int>(type: "int", nullable: true),
                    Interaction_ShareCount = table.Column<int>(type: "int", nullable: true),
                    Interaction_ViewCount = table.Column<int>(type: "int", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    Meta_CanonicalUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Meta_FocusKeywords = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Meta_MetaDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Meta_MetaTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Meta_OgType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Meta_RobotsIndex = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfileCoverGallery_CoverImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfileCoverGallery_ProfileImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkingHours_FinishTimeFriday = table.Column<TimeOnly>(type: "time", nullable: true),
                    WorkingHours_FinishTimeMonday = table.Column<TimeOnly>(type: "time", nullable: true),
                    WorkingHours_FinishTimeSaturday = table.Column<TimeOnly>(type: "time", nullable: true),
                    WorkingHours_FinishTimeSunday = table.Column<TimeOnly>(type: "time", nullable: true),
                    WorkingHours_FinishTimeThursday = table.Column<TimeOnly>(type: "time", nullable: true),
                    WorkingHours_FinishTimeTuesday = table.Column<TimeOnly>(type: "time", nullable: true),
                    WorkingHours_FinishTimeWednesday = table.Column<TimeOnly>(type: "time", nullable: true),
                    WorkingHours_IsActiveFriday = table.Column<bool>(type: "bit", nullable: true),
                    WorkingHours_IsActiveMonday = table.Column<bool>(type: "bit", nullable: true),
                    WorkingHours_IsActiveSaturday = table.Column<bool>(type: "bit", nullable: true),
                    WorkingHours_IsActiveSunday = table.Column<bool>(type: "bit", nullable: true),
                    WorkingHours_IsActiveThursday = table.Column<bool>(type: "bit", nullable: true),
                    WorkingHours_IsActiveTuesday = table.Column<bool>(type: "bit", nullable: true),
                    WorkingHours_IsActiveWednesday = table.Column<bool>(type: "bit", nullable: true),
                    WorkingHours_StartTimeFriday = table.Column<TimeOnly>(type: "time", nullable: true),
                    WorkingHours_StartTimeMonday = table.Column<TimeOnly>(type: "time", nullable: true),
                    WorkingHours_StartTimeSaturday = table.Column<TimeOnly>(type: "time", nullable: true),
                    WorkingHours_StartTimeSunday = table.Column<TimeOnly>(type: "time", nullable: true),
                    WorkingHours_StartTimeThursday = table.Column<TimeOnly>(type: "time", nullable: true),
                    WorkingHours_StartTimeTuesday = table.Column<TimeOnly>(type: "time", nullable: true),
                    WorkingHours_StartTimeWednesday = table.Column<TimeOnly>(type: "time", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAddressMethod",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AddressName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddressType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cities = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeliveryReceiverEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeliveryReceiverFirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeliveryReceiverLastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeliveryReceiverPhoneCountryCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeliveryReceiverPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAddressMethod", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserPaymentMethod",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CardAlias = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CardAssociation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CardHolderName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CardNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpirationDateMonth = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpirationDateYear = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPaymentMethod", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsersRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Warehouse",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AddressInfo_Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_GoogleMyBusinessAccountLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_Latitude = table.Column<decimal>(type: "decimal(18,15)", nullable: true),
                    AddressInfo_Longitude = table.Column<decimal>(type: "decimal(18,15)", nullable: true),
                    AddressInfo_MapTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouse", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkstationEmployeeGroup",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GroupName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_GoogleMyBusinessAccountLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_Latitude = table.Column<decimal>(type: "decimal(18,15)", nullable: true),
                    AddressInfo_Longitude = table.Column<decimal>(type: "decimal(18,15)", nullable: true),
                    AddressInfo_MapTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_Behance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_CountryPhoneCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_Discord = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_Facebook = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_FullPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_GitHub = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_Instagram = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_IsActiveBehance = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActiveDiscord = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActiveEmail = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActiveFacebook = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActiveGitHub = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActiveInstagram = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActiveKakaoTalk = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActiveLine = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActiveLinkedin = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActivePhoneNumber = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActivePinterest = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActiveReddit = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActiveTelegram = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActiveTikTok = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActiveUserWebSite = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActiveVKontakte = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActiveWeChat = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActiveWeibo = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActiveWhatsApp = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActiveX = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActiveYouTube = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_KakaoTalk = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_Line = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_Linkedin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_PhoneEmailConfirmed = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_Pinterest = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_Reddit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_Telegram = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_TikTok = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_UserWebSite = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_VKontakte = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_WeChat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_Weibo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_WhatsApp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_X = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_YouTube = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    ProfileCoverGallery_CoverImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfileCoverGallery_ProfileImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkstationEmployeeGroup", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Article_CreatedAt_Id",
                table: "Article",
                columns: new[] { "CreatedAt", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Article_Slug",
                table: "Article",
                column: "Slug");
        }
    }
}

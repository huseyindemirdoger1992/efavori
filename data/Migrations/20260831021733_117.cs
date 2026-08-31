using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _117 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductMediaTranslations");

            migrationBuilder.DropTable(
                name: "ProductMedia");

            migrationBuilder.AlterColumn<Guid>(
                name: "MediaId",
                table: "MediaItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ItemType",
                table: "MediaItems",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ItemAddDate",
                table: "MediaItems",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AltText",
                table: "MediaItems",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Caption",
                table: "MediaItems",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CropDataJson",
                table: "MediaItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FocalPointX",
                table: "MediaItems",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FocalPointY",
                table: "MediaItems",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPrimary",
                table: "MediaItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVisible",
                table: "MediaItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LinkUrl",
                table: "MediaItems",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MediaRole",
                table: "MediaItems",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "MediaItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "MediaItems",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "MediaItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Media",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "OrjFileUrl",
                table: "Media",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OrjFilePhysicalPathRoad",
                table: "Media",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileUrl_Ratio_1_8",
                table: "Media",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileUrl_Ratio_1_4",
                table: "Media",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileUrl_Ratio_1_2",
                table: "Media",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileUrl_Ratio_1_16",
                table: "Media",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileUrl",
                table: "Media",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileStoredName",
                table: "Media",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FilePhysicalPathRoad",
                table: "Media",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "Media",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileExtensionType",
                table: "Media",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "Media",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Bitrate",
                table: "Media",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BlurHash",
                table: "Media",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Codec",
                table: "Media",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DominantColor",
                table: "Media",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DurationMs",
                table: "Media",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ETag",
                table: "Media",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalProvider",
                table: "Media",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalUrl",
                table: "Media",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FrameRate",
                table: "Media",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasAlpha",
                table: "Media",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "Media",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Media",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MediaType",
                table: "Media",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "Orientation",
                table: "Media",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PageCount",
                table: "Media",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessedAt",
                table: "Media",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProcessingError",
                table: "Media",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProcessingStatus",
                table: "Media",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RenditionsJson",
                table: "Media",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sha256",
                table: "Media",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StorageBucket",
                table: "Media",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StorageKey",
                table: "Media",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StorageProvider",
                table: "Media",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Media",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Width",
                table: "Media",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MediaItems_ItemType_ItemId",
                table: "MediaItems",
                columns: new[] { "ItemType", "ItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_MediaItems_ItemType_ItemId_IsPrimary",
                table: "MediaItems",
                columns: new[] { "ItemType", "ItemId", "IsPrimary" });

            migrationBuilder.CreateIndex(
                name: "IX_MediaItems_ItemType_ItemId_MediaId_MediaRole",
                table: "MediaItems",
                columns: new[] { "ItemType", "ItemId", "MediaId", "MediaRole" },
                unique: true,
                filter: "[IsDeleted_IsDeletedStatu] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_MediaItems_ItemType_ItemId_SortOrder",
                table: "MediaItems",
                columns: new[] { "ItemType", "ItemId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_MediaItems_MediaId",
                table: "MediaItems",
                column: "MediaId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaItems_UserId",
                table: "MediaItems",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Media_CreatedAt",
                table: "Media",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Media_MediaType",
                table: "Media",
                column: "MediaType");

            migrationBuilder.CreateIndex(
                name: "IX_Media_ProcessingStatus",
                table: "Media",
                column: "ProcessingStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Media_Sha256",
                table: "Media",
                column: "Sha256");

            migrationBuilder.CreateIndex(
                name: "IX_Media_UserId",
                table: "Media",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Media_Users_UserId",
                table: "Media",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaItems_Media_MediaId",
                table: "MediaItems",
                column: "MediaId",
                principalTable: "Media",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Media_Users_UserId",
                table: "Media");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaItems_Media_MediaId",
                table: "MediaItems");

            migrationBuilder.DropIndex(
                name: "IX_MediaItems_ItemType_ItemId",
                table: "MediaItems");

            migrationBuilder.DropIndex(
                name: "IX_MediaItems_ItemType_ItemId_IsPrimary",
                table: "MediaItems");

            migrationBuilder.DropIndex(
                name: "IX_MediaItems_ItemType_ItemId_MediaId_MediaRole",
                table: "MediaItems");

            migrationBuilder.DropIndex(
                name: "IX_MediaItems_ItemType_ItemId_SortOrder",
                table: "MediaItems");

            migrationBuilder.DropIndex(
                name: "IX_MediaItems_MediaId",
                table: "MediaItems");

            migrationBuilder.DropIndex(
                name: "IX_MediaItems_UserId",
                table: "MediaItems");

            migrationBuilder.DropIndex(
                name: "IX_Media_CreatedAt",
                table: "Media");

            migrationBuilder.DropIndex(
                name: "IX_Media_MediaType",
                table: "Media");

            migrationBuilder.DropIndex(
                name: "IX_Media_ProcessingStatus",
                table: "Media");

            migrationBuilder.DropIndex(
                name: "IX_Media_Sha256",
                table: "Media");

            migrationBuilder.DropIndex(
                name: "IX_Media_UserId",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "AltText",
                table: "MediaItems");

            migrationBuilder.DropColumn(
                name: "Caption",
                table: "MediaItems");

            migrationBuilder.DropColumn(
                name: "CropDataJson",
                table: "MediaItems");

            migrationBuilder.DropColumn(
                name: "FocalPointX",
                table: "MediaItems");

            migrationBuilder.DropColumn(
                name: "FocalPointY",
                table: "MediaItems");

            migrationBuilder.DropColumn(
                name: "IsPrimary",
                table: "MediaItems");

            migrationBuilder.DropColumn(
                name: "IsVisible",
                table: "MediaItems");

            migrationBuilder.DropColumn(
                name: "LinkUrl",
                table: "MediaItems");

            migrationBuilder.DropColumn(
                name: "MediaRole",
                table: "MediaItems");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "MediaItems");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "MediaItems");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "MediaItems");

            migrationBuilder.DropColumn(
                name: "Bitrate",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "BlurHash",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "Codec",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "DominantColor",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "DurationMs",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "ETag",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "ExternalProvider",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "ExternalUrl",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "FrameRate",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "HasAlpha",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "MediaType",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "Orientation",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "PageCount",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "ProcessedAt",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "ProcessingError",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "ProcessingStatus",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "RenditionsJson",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "Sha256",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "StorageBucket",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "StorageKey",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "StorageProvider",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "Media");

            migrationBuilder.AlterColumn<Guid>(
                name: "MediaId",
                table: "MediaItems",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "ItemType",
                table: "MediaItems",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ItemAddDate",
                table: "MediaItems",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Media",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OrjFileUrl",
                table: "Media",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2048)",
                oldMaxLength: 2048,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OrjFilePhysicalPathRoad",
                table: "Media",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2048)",
                oldMaxLength: 2048,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileUrl_Ratio_1_8",
                table: "Media",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2048)",
                oldMaxLength: 2048,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileUrl_Ratio_1_4",
                table: "Media",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2048)",
                oldMaxLength: 2048,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileUrl_Ratio_1_2",
                table: "Media",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2048)",
                oldMaxLength: 2048,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileUrl_Ratio_1_16",
                table: "Media",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2048)",
                oldMaxLength: 2048,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileUrl",
                table: "Media",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2048)",
                oldMaxLength: 2048,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileStoredName",
                table: "Media",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FilePhysicalPathRoad",
                table: "Media",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2048)",
                oldMaxLength: 2048,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "Media",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileExtensionType",
                table: "Media",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "Media",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ProductMedia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    ExternalUrl = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsCover = table.Column<bool>(type: "bit", nullable: false),
                    MediaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MediaType = table.Column<byte>(type: "tinyint", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductVariantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductMedia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductMedia_Media_MediaId",
                        column: x => x.MediaId,
                        principalTable: "Media",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductMedia_ProductVariants_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductMedia_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductMediaTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AltText = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Caption = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsManuallyEdited = table.Column<bool>(type: "bit", nullable: false),
                    Language = table.Column<byte>(type: "tinyint", nullable: false),
                    ProductMediaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Source = table.Column<byte>(type: "tinyint", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductMediaTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductMediaTranslations_ProductMedia_ProductMediaId",
                        column: x => x.ProductMediaId,
                        principalTable: "ProductMedia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductMedia_MediaId",
                table: "ProductMedia",
                column: "MediaId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMedia_ProductId_DisplayOrder",
                table: "ProductMedia",
                columns: new[] { "ProductId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductMedia_ProductVariantId",
                table: "ProductMedia",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMediaTranslations_ProductMediaId_Language",
                table: "ProductMediaTranslations",
                columns: new[] { "ProductMediaId", "Language" },
                unique: true,
                filter: "[IsDeleted_IsDeletedStatu] = 0");
        }
    }
}

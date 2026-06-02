using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _67 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "IsDeleted_DeletedAtDate",
                table: "ProductVariantValueCreateRequest",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted_IsDeletedStatu",
                table: "ProductVariantValueCreateRequest",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "IsDeleted_DeletedAtDate",
                table: "ProductVariantCreateRequest",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted_IsDeletedStatu",
                table: "ProductVariantCreateRequest",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "IsDeleted_DeletedAtDate",
                table: "ProductPricingCreateRequest",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted_IsDeletedStatu",
                table: "ProductPricingCreateRequest",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "IsDeleted_DeletedAtDate",
                table: "ProductCreateRequest",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted_IsDeletedStatu",
                table: "ProductCreateRequest",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "IsDeleted_DeletedAtDate",
                table: "ProductAttributeValueCreateRequest",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted_IsDeletedStatu",
                table: "ProductAttributeValueCreateRequest",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "IsDeleted_DeletedAtDate",
                table: "ProductAttributeCreateRequest",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted_IsDeletedStatu",
                table: "ProductAttributeCreateRequest",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted_DeletedAtDate",
                table: "ProductVariantValueCreateRequest");

            migrationBuilder.DropColumn(
                name: "IsDeleted_IsDeletedStatu",
                table: "ProductVariantValueCreateRequest");

            migrationBuilder.DropColumn(
                name: "IsDeleted_DeletedAtDate",
                table: "ProductVariantCreateRequest");

            migrationBuilder.DropColumn(
                name: "IsDeleted_IsDeletedStatu",
                table: "ProductVariantCreateRequest");

            migrationBuilder.DropColumn(
                name: "IsDeleted_DeletedAtDate",
                table: "ProductPricingCreateRequest");

            migrationBuilder.DropColumn(
                name: "IsDeleted_IsDeletedStatu",
                table: "ProductPricingCreateRequest");

            migrationBuilder.DropColumn(
                name: "IsDeleted_DeletedAtDate",
                table: "ProductCreateRequest");

            migrationBuilder.DropColumn(
                name: "IsDeleted_IsDeletedStatu",
                table: "ProductCreateRequest");

            migrationBuilder.DropColumn(
                name: "IsDeleted_DeletedAtDate",
                table: "ProductAttributeValueCreateRequest");

            migrationBuilder.DropColumn(
                name: "IsDeleted_IsDeletedStatu",
                table: "ProductAttributeValueCreateRequest");

            migrationBuilder.DropColumn(
                name: "IsDeleted_DeletedAtDate",
                table: "ProductAttributeCreateRequest");

            migrationBuilder.DropColumn(
                name: "IsDeleted_IsDeletedStatu",
                table: "ProductAttributeCreateRequest");
        }
    }
}

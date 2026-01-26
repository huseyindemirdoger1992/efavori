using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "ExteriorImages",
                table: "ItemGallery");

            migrationBuilder.DropColumn(
                name: "InteriorPictures",
                table: "ItemGallery");

            migrationBuilder.DropColumn(
                name: "IsDeleted_DeletedAtDate",
                table: "ItemGallery");

            migrationBuilder.RenameColumn(
                name: "IsDeleted_IsDeletedStatu",
                table: "ItemGallery",
                newName: "IsDelete");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Logs",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Action",
                table: "Logs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "PageNameSpaceTitle",
                table: "Logs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "ItemGallery",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PageNameSpaceTitle",
                table: "Logs");

            migrationBuilder.RenameColumn(
                name: "IsDelete",
                table: "ItemGallery",
                newName: "IsDeleted_IsDeletedStatu");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Logs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Action",
                table: "Logs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Logs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "ItemGallery",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ExteriorImages",
                table: "ItemGallery",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "InteriorPictures",
                table: "ItemGallery",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "IsDeleted_DeletedAtDate",
                table: "ItemGallery",
                type: "datetime2",
                nullable: true);
        }
    }
}

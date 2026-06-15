using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _79 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AiContentStatus",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AiErrorMessage",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AiOriginalFullDescription",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AiOriginalName",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AiOriginalShortDescription",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AiOriginalTags",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AiProcessedAt",
                table: "Products",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AiRetryCount",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsAiManaged",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AiContentStatus",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AiErrorMessage",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AiOriginalFullDescription",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AiOriginalName",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AiOriginalShortDescription",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AiOriginalTags",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AiProcessedAt",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AiRetryCount",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsAiManaged",
                table: "Products");
        }
    }
}

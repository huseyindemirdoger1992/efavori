using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _95 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SourceAiTitleId",
                table: "Article",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AiErrorMessage",
                table: "AiTitlesForArticle",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AiProcessedAt",
                table: "AiTitlesForArticle",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AiRetryCount",
                table: "AiTitlesForArticle",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceAiTitleId",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "AiErrorMessage",
                table: "AiTitlesForArticle");

            migrationBuilder.DropColumn(
                name: "AiProcessedAt",
                table: "AiTitlesForArticle");

            migrationBuilder.DropColumn(
                name: "AiRetryCount",
                table: "AiTitlesForArticle");
        }
    }
}

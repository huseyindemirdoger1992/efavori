using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _22 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "IsDeleted_DeletedAtDate",
                table: "TaskStatus",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted_IsDeletedStatu",
                table: "TaskStatus",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "IsDeleted_DeletedAtDate",
                table: "TaskNotes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted_IsDeletedStatu",
                table: "TaskNotes",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsTheNoteOk",
                table: "TaskNotes",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted_DeletedAtDate",
                table: "TaskStatus");

            migrationBuilder.DropColumn(
                name: "IsDeleted_IsDeletedStatu",
                table: "TaskStatus");

            migrationBuilder.DropColumn(
                name: "IsDeleted_DeletedAtDate",
                table: "TaskNotes");

            migrationBuilder.DropColumn(
                name: "IsDeleted_IsDeletedStatu",
                table: "TaskNotes");

            migrationBuilder.DropColumn(
                name: "IsTheNoteOk",
                table: "TaskNotes");
        }
    }
}

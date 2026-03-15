using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _48 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Users",
                newName: "IsDeleted_IsDeletedStatu");

            migrationBuilder.AddColumn<DateTime>(
                name: "IsDeleted_DeletedAtDate",
                table: "Users",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted_DeletedAtDate",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "IsDeleted_IsDeletedStatu",
                table: "Users",
                newName: "IsDeleted");
        }
    }
}

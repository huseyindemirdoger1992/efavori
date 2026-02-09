using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _25 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserRolesAccessPermissions_IsItStaff",
                table: "Users",
                newName: "UserIsEmployee");

            migrationBuilder.AddColumn<Guid>(
                name: "WorkstationEmployeeGroupId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "WorkstationEmployeeGroupId",
                table: "TaskCategories",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkstationEmployeeGroupId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "WorkstationEmployeeGroupId",
                table: "TaskCategories");

            migrationBuilder.RenameColumn(
                name: "UserIsEmployee",
                table: "Users",
                newName: "UserRolesAccessPermissions_IsItStaff");
        }
    }
}

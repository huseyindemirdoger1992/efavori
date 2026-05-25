using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _55 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TryTableMulti");

            migrationBuilder.AddColumn<DateTime>(
                name: "IsDeleted_DeletedAtDate",
                table: "TryTableSingle",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted_IsDeletedStatu",
                table: "TryTableSingle",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted_DeletedAtDate",
                table: "TryTableSingle");

            migrationBuilder.DropColumn(
                name: "IsDeleted_IsDeletedStatu",
                table: "TryTableSingle");

            migrationBuilder.CreateTable(
                name: "TryTableMulti",
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
                    table.PrimaryKey("PK_TryTableMulti", x => x.Id);
                });
        }
    }
}

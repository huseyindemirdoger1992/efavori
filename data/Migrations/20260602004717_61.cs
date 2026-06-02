using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _61 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ProductIsVariant",
                table: "Product",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StoreBlockingInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BlockInfoDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsSucces = table.Column<bool>(type: "bit", nullable: false),
                    AtDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreBlockingInfos", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoreBlockingInfos");

            migrationBuilder.DropColumn(
                name: "ProductIsVariant",
                table: "Product");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AddressInfo_MapTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_Latitude = table.Column<double>(type: "float", nullable: true),
                    AddressInfo_Longitude = table.Column<double>(type: "float", nullable: true),
                    AddressInfo_GoogleMyBusinessAccountLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses");
        }
    }
}

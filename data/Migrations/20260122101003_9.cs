using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressInfo_Address",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AddressInfo_City",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AddressInfo_Country",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AddressInfo_GoogleMyBusinessAccountLink",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AddressInfo_Latitude",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AddressInfo_Longitude",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AddressInfo_MapTitle",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AddressInfo_State",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AddressInfo_ZipCode",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AddressInfo_Address",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressInfo_City",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressInfo_Country",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressInfo_GoogleMyBusinessAccountLink",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "AddressInfo_Latitude",
                table: "Users",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "AddressInfo_Longitude",
                table: "Users",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressInfo_MapTitle",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressInfo_State",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressInfo_ZipCode",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

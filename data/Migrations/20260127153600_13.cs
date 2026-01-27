using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountActivationMailCode",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "AccountActivationMailDeadline",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AccountActivationMailStatu",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Pricing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PriceTL = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountPriceTL = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PriceUSD = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountPriceUSD = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PriceEUR = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountPriceEUR = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PriceAZN = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountPriceAZN = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PriceTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pricing", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pricing");

            migrationBuilder.DropColumn(
                name: "AccountActivationMailCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AccountActivationMailDeadline",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AccountActivationMailStatu",
                table: "Users");
        }
    }
}

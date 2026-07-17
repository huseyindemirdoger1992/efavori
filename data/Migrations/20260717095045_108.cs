using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _108 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DenemeXXXXXXXXXXXXXXXXX",
                table: "CategoriesProduct");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DenemeXXXXXXXXXXXXXXXXX",
                table: "CategoriesProduct",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

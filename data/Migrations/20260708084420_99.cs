using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _99 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ConfirmedByAi",
                table: "ProductReview",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WhyDidAiNotApproveIt",
                table: "ProductReview",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfirmedByAi",
                table: "ProductReview");

            migrationBuilder.DropColumn(
                name: "WhyDidAiNotApproveIt",
                table: "ProductReview");
        }
    }
}

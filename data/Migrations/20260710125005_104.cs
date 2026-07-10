using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _104 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AllBackgroundServicesFrequencyRate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    IsCurrencyFetchEnabled = table.Column<bool>(type: "bit", nullable: false),
                    CurrencyFetchIntervalInSeconds = table.Column<int>(type: "int", nullable: false),
                    IsAiContentGenerationEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AiContentGenerationIntervalInSeconds = table.Column<int>(type: "int", nullable: false),
                    AiContentGenerationIntervalMaxAiRetry = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllBackgroundServicesFrequencyRate", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AllBackgroundServicesFrequencyRate");
        }
    }
}

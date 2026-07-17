using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    public partial class _110 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoriesProduct");

            migrationBuilder.CreateTable(
                name: "CategoriesProduct",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),

                    // Buraya CategoriesProduct modelindeki diğer bütün kolonları
                    // eski migration'daki tipleriyle ekle.
                    // Örnek:
                    // NameTr = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    // NameEn = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    // SortOrder = table.Column<int>(type: "int", nullable: false),
                    // ...
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriesProduct", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoriesProduct");

            migrationBuilder.CreateTable(
                name: "CategoriesProduct",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    ParentId = table.Column<int>(type: "int", nullable: true),

                    // Eski kolonları da aynı şekilde geri oluştur.
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriesProduct", x => x.Id);
                });
        }
    }
}
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _114 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoriesProduct",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ShowInMenu = table.Column<bool>(type: "bit", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AiAttributesIsOk = table.Column<bool>(type: "bit", nullable: true),
                    Categories_NameTr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_NameEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_NameAz = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_NameDe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_NameEs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_NameFr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_NameHi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_NamePt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_NameRu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_NameZh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_SlugTr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_SlugEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_SlugAz = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_SlugDe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_SlugEs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_SlugFr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_SlugHi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_SlugPt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_SlugRu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categories_SlugZh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriesProduct", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoriesProduct");
        }
    }
}

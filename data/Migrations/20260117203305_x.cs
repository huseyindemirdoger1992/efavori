using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class x : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "CategoriesAz",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ExternalId = table.Column<int>(type: "int", nullable: true),
            //        ParentCategoryId = table.Column<int>(type: "int", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_CategoriesAz", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "CategoriesDe",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ExternalId = table.Column<int>(type: "int", nullable: true),
            //        ParentCategoryId = table.Column<int>(type: "int", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_CategoriesDe", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "CategoriesEn",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ExternalId = table.Column<int>(type: "int", nullable: true),
            //        ParentCategoryId = table.Column<int>(type: "int", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_CategoriesEn", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "CategoriesEs",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ExternalId = table.Column<int>(type: "int", nullable: true),
            //        ParentCategoryId = table.Column<int>(type: "int", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_CategoriesEs", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "CategoriesFr",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ExternalId = table.Column<int>(type: "int", nullable: true),
            //        ParentCategoryId = table.Column<int>(type: "int", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_CategoriesFr", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "CategoriesHi",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ExternalId = table.Column<int>(type: "int", nullable: true),
            //        ParentCategoryId = table.Column<int>(type: "int", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_CategoriesHi", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "CategoriesPt",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ExternalId = table.Column<int>(type: "int", nullable: true),
            //        ParentCategoryId = table.Column<int>(type: "int", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_CategoriesPt", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "CategoriesRu",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ExternalId = table.Column<int>(type: "int", nullable: true),
            //        ParentCategoryId = table.Column<int>(type: "int", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_CategoriesRu", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "CategoriesTr",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ExternalId = table.Column<int>(type: "int", nullable: true),
            //        ParentCategoryId = table.Column<int>(type: "int", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_CategoriesTr", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "CategoriesZh",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ExternalId = table.Column<int>(type: "int", nullable: true),
            //        ParentCategoryId = table.Column<int>(type: "int", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_CategoriesZh", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Cities",
            //    columns: table => new
            //    {
            //        id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        state_id = table.Column<int>(type: "int", nullable: true),
            //        state_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        state_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        country_id = table.Column<int>(type: "int", nullable: true),
            //        country_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        country_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        latitude = table.Column<double>(type: "float", nullable: true),
            //        longitude = table.Column<double>(type: "float", nullable: true),
            //        native = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        type = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        level = table.Column<int>(type: "int", nullable: true),
            //        parent_id = table.Column<int>(type: "int", nullable: true),
            //        population = table.Column<long>(type: "bigint", nullable: true),
            //        timezone = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        translations = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        wikiDataId = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Cities", x => x.id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Country",
            //    columns: table => new
            //    {
            //        id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        name = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        iso3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        iso2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        numeric_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        phonecode = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        capital = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        currency_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        currency_symbol = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        tld = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        native = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        population = table.Column<long>(type: "bigint", nullable: true),
            //        gdp = table.Column<long>(type: "bigint", nullable: true),
            //        region = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        region_id = table.Column<int>(type: "int", nullable: true),
            //        subregion = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        subregion_id = table.Column<int>(type: "int", nullable: true),
            //        nationality = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        area_sq_km = table.Column<long>(type: "bigint", nullable: true),
            //        postal_code_format = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        postal_code_regex = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        timezones = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        translations = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        latitude = table.Column<decimal>(type: "decimal(18,8)", nullable: true),
            //        longitude = table.Column<decimal>(type: "decimal(18,8)", nullable: true),
            //        emoji = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        emojiU = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        wikiDataId = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        flag = table.Column<bool>(type: "bit", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Country", x => x.id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Regions",
            //    columns: table => new
            //    {
            //        id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
            //        translations = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        wikiDataId = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        flag = table.Column<bool>(type: "bit", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Regions", x => x.id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "States",
            //    columns: table => new
            //    {
            //        id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
            //        country_id = table.Column<int>(type: "int", nullable: true),
            //        country_code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
            //        country_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
            //        iso2 = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
            //        iso3166_2 = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
            //        fips_code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
            //        type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
            //        level = table.Column<int>(type: "int", nullable: true),
            //        parent_id = table.Column<int>(type: "int", nullable: true),
            //        native = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
            //        latitude = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
            //        longitude = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
            //        timezone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
            //        translations = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        wikiDataId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
            //        population = table.Column<long>(type: "bigint", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_States", x => x.id);
            //    });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "CategoriesAz");

            //migrationBuilder.DropTable(
            //    name: "CategoriesDe");

            //migrationBuilder.DropTable(
            //    name: "CategoriesEn");

            //migrationBuilder.DropTable(
            //    name: "CategoriesEs");

            //migrationBuilder.DropTable(
            //    name: "CategoriesFr");

            //migrationBuilder.DropTable(
            //    name: "CategoriesHi");

            //migrationBuilder.DropTable(
            //    name: "CategoriesPt");

            //migrationBuilder.DropTable(
            //    name: "CategoriesRu");

            //migrationBuilder.DropTable(
            //    name: "CategoriesTr");

            //migrationBuilder.DropTable(
            //    name: "CategoriesZh");

            //migrationBuilder.DropTable(
            //    name: "Cities");

            //migrationBuilder.DropTable(
            //    name: "Country");

            //migrationBuilder.DropTable(
            //    name: "Regions");

            //migrationBuilder.DropTable(
            //    name: "States");
        }
    }
}

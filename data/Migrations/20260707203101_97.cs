using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _97 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LogOutTimer",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Article",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Article",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            // İndeks zaten varsa hata almamak için SQL üzerinden kontrol ederek ekliyoruz
            migrationBuilder.Sql(@"
        IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Article_CreatedAt_Id' AND object_id = OBJECT_ID('Article'))
        BEGIN
            CREATE INDEX [IX_Article_CreatedAt_Id] ON [Article] ([CreatedAt], [Id]);
        END");

            // Slug indeksi yoksa oluştur
            migrationBuilder.Sql(@"
        IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Article_Slug' AND object_id = OBJECT_ID('Article'))
        BEGIN
            CREATE INDEX [IX_Article_Slug] ON [Article] ([Slug]);
        END");
        }
    }
}

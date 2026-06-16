using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _81 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. ADIM: Mevcut tüm string kayıtları '1' yap. 
            // Böylece hem hepsi true olacak hem de AlterColumn sırasında SQL Server cast hatası vermeyecek.
            migrationBuilder.Sql("UPDATE Products SET PublishStatus = '1'");

            // 2. ADIM: Sütun tipini nvarchar'dan bit'e dönüştür. ('1' değerleri otomatik olarak true bit'ine dönüşür)
            migrationBuilder.AlterColumn<bool>(
                name: "PublishStatus",
                table: "Products",
                type: "bit",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Geri alma durumunda önce sütunu tekrar string'e çeviriyoruz.
            migrationBuilder.AlterColumn<string>(
                name: "PublishStatus",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            // Geri alınınca hepsini eski varsayılan string haline getiriyoruz.
            migrationBuilder.Sql("UPDATE Products SET PublishStatus = 'Published'");
        }
    }
}
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _16 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsDeleted_IsDeletedStatu",
                table: "Media",
                newName: "IsDeletedStatu");

            migrationBuilder.RenameColumn(
                name: "IsDeleted_DeletedAtDate",
                table: "Media",
                newName: "DeletedAtDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsDeletedStatu",
                table: "Media",
                newName: "IsDeleted_IsDeletedStatu");

            migrationBuilder.RenameColumn(
                name: "DeletedAtDate",
                table: "Media",
                newName: "IsDeleted_DeletedAtDate");
        }
    }
}

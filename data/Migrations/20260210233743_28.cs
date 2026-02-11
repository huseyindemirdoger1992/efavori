using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _28 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "TaskStatus",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsInEditing",
                table: "TaskStatus",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsInProgress",
                table: "TaskStatus",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsNew",
                table: "TaskStatus",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsToBeDone",
                table: "TaskStatus",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "TaskStatus");

            migrationBuilder.DropColumn(
                name: "IsInEditing",
                table: "TaskStatus");

            migrationBuilder.DropColumn(
                name: "IsInProgress",
                table: "TaskStatus");

            migrationBuilder.DropColumn(
                name: "IsNew",
                table: "TaskStatus");

            migrationBuilder.DropColumn(
                name: "IsToBeDone",
                table: "TaskStatus");
        }
    }
}

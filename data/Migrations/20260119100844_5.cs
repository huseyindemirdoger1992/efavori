using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "TermsOfUse",
                table: "Users",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RegistrationDate",
                table: "Users",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Language",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Users",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActiveVendorStatu",
                table: "Users",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Users",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "Users",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Currency",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "ContactInformation_PhoneNumberConfirmed",
                table: "Users",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "ContactInformation_PhoneEmailConfirmed",
                table: "Users",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<bool>(
                name: "UserRolesAccessPermissions_IsItStaff",
                table: "Users",
                type: "bit",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Logs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ItemGallery",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InteriorPictures = table.Column<bool>(type: "bit", nullable: true),
                    ExteriorImages = table.Column<bool>(type: "bit", nullable: true),
                    ItemType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemRoad = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemAddDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemGallery", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NativeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Iso2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Iso3 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoginTry",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsersId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AttemptDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsSuccessful = table.Column<bool>(type: "bit", nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Platform = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Browser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestPath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginTry", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Media",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FileStoredName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePhysicalPathRoad = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileExtensionType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    ContentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OriginalSize = table.Column<long>(type: "bigint", nullable: true),
                    CompressedSize = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Media", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdminPermissionPending = table.Column<bool>(type: "bit", nullable: true),
                    IsActiveStateAdmin = table.Column<bool>(type: "bit", nullable: true),
                    IsActiveDateAdmin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActiveStateVendor = table.Column<bool>(type: "bit", nullable: true),
                    IsActiveDateVendor = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ContactInformation_IsActiveEmail = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_PhoneEmailConfirmed = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActivePhoneNumber = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_CountryPhoneCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActiveFacebook = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_Facebook = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_IsActiveInstagram = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_Instagram = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_IsActiveX = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_X = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_IsActiveTikTok = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_TikTok = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_IsActiveYouTube = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_YouTube = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_IsActiveLinkedin = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_Linkedin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_IsActiveWhatsApp = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_WhatsApp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_IsActiveTelegram = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_Telegram = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_IsActiveWeChat = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_WeChat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_IsActiveWeibo = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_Weibo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_IsActiveVKontakte = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_VKontakte = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_IsActiveLine = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_Line = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_IsActiveKakaoTalk = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_KakaoTalk = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_IsActivePinterest = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_Pinterest = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_IsActiveGitHub = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_GitHub = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_IsActiveBehance = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_Behance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_IsActiveDiscord = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_Discord = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_IsActiveReddit = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_Reddit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_IsActiveUserWebSite = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_UserWebSite = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfileCoverGallery_ProfileImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfileCoverGallery_CoverImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_MapTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_Latitude = table.Column<double>(type: "float", nullable: true),
                    AddressInfo_Longitude = table.Column<double>(type: "float", nullable: true),
                    AddressInfo_GoogleMyBusinessAccountLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkingHours_IsActiveMonday = table.Column<bool>(type: "bit", nullable: true),
                    WorkingHours_StartTimeMonday = table.Column<TimeOnly>(type: "time", nullable: true),
                    WorkingHours_FinishTimeMonday = table.Column<TimeOnly>(type: "time", nullable: true),
                    WorkingHours_IsActiveTuesday = table.Column<bool>(type: "bit", nullable: true),
                    WorkingHours_StartTimeTuesday = table.Column<TimeOnly>(type: "time", nullable: true),
                    WorkingHours_FinishTimeTuesday = table.Column<TimeOnly>(type: "time", nullable: true),
                    WorkingHours_IsActiveWednesday = table.Column<bool>(type: "bit", nullable: true),
                    WorkingHours_StartTimeWednesday = table.Column<TimeOnly>(type: "time", nullable: true),
                    WorkingHours_FinishTimeWednesday = table.Column<TimeOnly>(type: "time", nullable: true),
                    WorkingHours_IsActiveThursday = table.Column<bool>(type: "bit", nullable: true),
                    WorkingHours_StartTimeThursday = table.Column<TimeOnly>(type: "time", nullable: true),
                    WorkingHours_FinishTimeThursday = table.Column<TimeOnly>(type: "time", nullable: true),
                    WorkingHours_IsActiveFriday = table.Column<bool>(type: "bit", nullable: true),
                    WorkingHours_StartTimeFriday = table.Column<TimeOnly>(type: "time", nullable: true),
                    WorkingHours_FinishTimeFriday = table.Column<TimeOnly>(type: "time", nullable: true),
                    WorkingHours_IsActiveSaturday = table.Column<bool>(type: "bit", nullable: true),
                    WorkingHours_StartTimeSaturday = table.Column<TimeOnly>(type: "time", nullable: true),
                    WorkingHours_FinishTimeSaturday = table.Column<TimeOnly>(type: "time", nullable: true),
                    WorkingHours_IsActiveSunday = table.Column<bool>(type: "bit", nullable: true),
                    WorkingHours_StartTimeSunday = table.Column<TimeOnly>(type: "time", nullable: true),
                    WorkingHours_FinishTimeSunday = table.Column<TimeOnly>(type: "time", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsersRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersRoles", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemGallery");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropTable(
                name: "LoginTry");

            migrationBuilder.DropTable(
                name: "Media");

            migrationBuilder.DropTable(
                name: "Stores");

            migrationBuilder.DropTable(
                name: "UsersRoles");

            migrationBuilder.DropColumn(
                name: "UserRolesAccessPermissions_IsItStaff",
                table: "Users");

            migrationBuilder.AlterColumn<bool>(
                name: "TermsOfUse",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RegistrationDate",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Language",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActiveVendorStatu",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Currency",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "ContactInformation_PhoneNumberConfirmed",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "ContactInformation_PhoneEmailConfirmed",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Logs",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");
        }
    }
}

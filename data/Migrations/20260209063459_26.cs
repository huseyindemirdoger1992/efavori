using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _26 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "UserRolesAccessPermissions_IsItStaff",
                table: "Users",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WorkstationEmployeeGroup",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_IsActiveEmail = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_PhoneEmailConfirmed = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_IsActivePhoneNumber = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_CountryPhoneCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_FullPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    AddressInfo_MapTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressInfo_Latitude = table.Column<double>(type: "float", nullable: true),
                    AddressInfo_Longitude = table.Column<double>(type: "float", nullable: true),
                    AddressInfo_GoogleMyBusinessAccountLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkstationEmployeeGroup", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkstationEmployeeGroup");

            migrationBuilder.DropColumn(
                name: "UserRolesAccessPermissions_IsItStaff",
                table: "Users");
        }
    }
}

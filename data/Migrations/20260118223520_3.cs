using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsersType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HeaderMenuType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsActiveVendorStatu = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    TermsOfUse = table.Column<bool>(type: "bit", nullable: false),
                    BackgroundImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_IsActiveEmail = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_PhoneEmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    ContactInformation_IsActivePhoneNumber = table.Column<bool>(type: "bit", nullable: true),
                    ContactInformation_CountryPhoneCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInformation_PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
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
                    IsPrivateOrPublic_IsProfilePublic = table.Column<bool>(type: "bit", nullable: true),
                    IsPrivateOrPublic_IsAllowPostsView = table.Column<bool>(type: "bit", nullable: true),
                    IsPrivateOrPublic_IsShowLastSeen = table.Column<bool>(type: "bit", nullable: true),
                    IsPrivateOrPublic_IsShowOnlineStatus = table.Column<bool>(type: "bit", nullable: true),
                    IsPrivateOrPublic_IsSearchEngineIndexingAllowed = table.Column<bool>(type: "bit", nullable: true),
                    IsPrivateOrPublic_IsAllowFriendRequest = table.Column<bool>(type: "bit", nullable: true),
                    IsPrivateOrPublic_IsAllowDirectMessages = table.Column<bool>(type: "bit", nullable: true),
                    IsPrivateOrPublic_IsAllowVoiceCalls = table.Column<bool>(type: "bit", nullable: true),
                    IsPrivateOrPublic_IsAllowVideoCalls = table.Column<bool>(type: "bit", nullable: true),
                    IsPrivateOrPublic_IsReadReceiptsEnabled = table.Column<bool>(type: "bit", nullable: true),
                    IsPrivateOrPublic_IsLocationVisible = table.Column<bool>(type: "bit", nullable: true),
                    IsPrivateOrPublic_IsShowFollowerCount = table.Column<bool>(type: "bit", nullable: true),
                    IsPrivateOrPublic_IsShowFollowingList = table.Column<bool>(type: "bit", nullable: true),
                    IsPrivateOrPublic_IsShowFollowerList = table.Column<bool>(type: "bit", nullable: true),
                    IsPrivateOrPublic_IsAllowComments = table.Column<bool>(type: "bit", nullable: true),
                    IsPrivateOrPublic_IsAllowTagging = table.Column<bool>(type: "bit", nullable: true),
                    IsPrivateOrPublic_IsAllowStorySharing = table.Column<bool>(type: "bit", nullable: true),
                    IsPrivateOrPublic_IsAllowPostDownloading = table.Column<bool>(type: "bit", nullable: true),
                    IsPrivateOrPublic_IsEmailNotificationEnabled = table.Column<bool>(type: "bit", nullable: true),
                    IsPrivateOrPublic_IsPushNotificationEnabled = table.Column<bool>(type: "bit", nullable: true),
                    IsPrivateOrPublic_IsSmsNotificationEnabled = table.Column<bool>(type: "bit", nullable: true),
                    IsPrivateOrPublic_IsTwoFactorAuthEnabled = table.Column<bool>(type: "bit", nullable: true),
                    IsPrivateOrPublic_IsPersonalizedAdsEnabled = table.Column<bool>(type: "bit", nullable: true),
                    IsPrivateOrPublic_IsDataCollectionAllowed = table.Column<bool>(type: "bit", nullable: true),
                    IsPrivateOrPublic_IsAiTrainingAllowed = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserShortcuts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShortcutName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShortcutUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShortcutIcon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserShortcuts", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "UserShortcuts");
        }
    }
}

using Microsoft.EntityFrameworkCore;

namespace data.Owned
{
    [Owned]
    public class ContactInformation
    {
        public bool? IsActiveEmail { get; set; }
        public string? Email { get; set; }
        public bool PhoneEmailConfirmed { get; set; }


        public bool? IsActivePhoneNumber { get; set; }
        public string? CountryPhoneCode { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FullPhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }


        // Global Devler
        public bool? IsActiveFacebook { get; set; }
        public string? Facebook { get; set; }

        public bool? IsActiveInstagram { get; set; }
        public string? Instagram { get; set; }

        public bool? IsActiveX { get; set; }
        public string? X { get; set; }

        public bool? IsActiveTikTok { get; set; }
        public string? TikTok { get; set; }

        public bool? IsActiveYouTube { get; set; }
        public string? YouTube { get; set; }

        public bool? IsActiveLinkedin { get; set; }
        public string? Linkedin { get; set; }

        public bool? IsActiveWhatsApp { get; set; }
        public string? WhatsApp { get; set; }

        public bool? IsActiveTelegram { get; set; }
        public string? Telegram { get; set; }

        // Bölgesel Popüler Platformlar
        public bool? IsActiveWeChat { get; set; }
        public string? WeChat { get; set; }

        public bool? IsActiveWeibo { get; set; }
        public string? Weibo { get; set; }

        public bool? IsActiveVKontakte { get; set; }
        public string? VKontakte { get; set; }

        public bool? IsActiveLine { get; set; }
        public string? Line { get; set; }

        public bool? IsActiveKakaoTalk { get; set; }
        public string? KakaoTalk { get; set; }

        // Profesyonel/Niş Platformlar
        public bool? IsActivePinterest { get; set; }
        public string? Pinterest { get; set; }

        public bool? IsActiveGitHub { get; set; }
        public string? GitHub { get; set; }

        public bool? IsActiveBehance { get; set; }
        public string? Behance { get; set; }

        public bool? IsActiveDiscord { get; set; }
        public string? Discord { get; set; }

        public bool? IsActiveReddit { get; set; }
        public string? Reddit { get; set; }

        public bool? IsActiveUserWebSite { get; set; }
        public string? UserWebSite { get; set; }
    }
}
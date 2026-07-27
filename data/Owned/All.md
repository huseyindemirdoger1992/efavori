## Dosya: AddressInfo.cs
```csharp
﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace data.Owned
{
    [Owned]
    public class AddressInfo
    {
        public string? MapTitle { get; set; } // Örn: "Evim", "Merkez Ofis", "Depo", "Mağaza" gibi harita üzerinde gösterilecek başlık

        public string? Country { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public string? ZipCode { get; set; }
        public string? Address { get; set; }

        [Column(TypeName = "decimal(18,15)")]
        public decimal? Latitude { get; set; }

        [Column(TypeName = "decimal(18,15)")]
        public decimal? Longitude { get; set; }
        public string? GoogleMyBusinessAccountLink { get; set; } // Google Benim İşletmem Hesabı Linki 
    }
}```

## Dosya: CartProductSnapshot.cs
```csharp
﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace data.Owned
{
    [Owned]
    public class CartProductSnapshot
    {
        public string ProductShortDescription { get; set; }
        public string ProductImageUrl { get; set; }

        public string BrandName { get; set; }
        public int CategoryName { get; set; }

        public string SKU { get; set; }
        public string Barcode { get; set; }

        public decimal SalePriceUsd { get; set; }
        public decimal DiscountAmountUsd { get; set; }
        public decimal ShippingPriceUsd { get; set; }

        public decimal SalePriceTry { get; set; }
        public decimal DiscountAmountTry { get; set; }
        public decimal ShippingPriceTry { get; set; }

        public decimal SalePriceEur { get; set; }
        public decimal DiscountAmountEur { get; set; }
        public decimal ShippingPriceEur { get; set; }

        public decimal SalePriceAzn { get; set; }
        public decimal DiscountAmountAzn { get; set; }
        public decimal ShippingPriceAzn { get; set; }

        public int Quantity { get; set; }
        public string CouponCode { get; set; }

        public decimal VatRate { get; set; }

        public decimal? Weight { get; set; }
        public decimal? Desi { get; set; }

        public string DeliveryTimeText { get; set; }
        public string CustomerNote { get; set; }

    }
}
```

## Dosya: Categories.cs
```csharp
﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace data.Owned
{
    [Owned]
    public class Categories
    {
        // -------------------- CATEGORY NAMES --------------------

        public string NameTr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string NameAz { get; set; } = string.Empty;
        public string NameDe { get; set; } = string.Empty;
        public string NameEs { get; set; } = string.Empty;
        public string NameFr { get; set; } = string.Empty;
        public string NameHi { get; set; } = string.Empty;
        public string NamePt { get; set; } = string.Empty;
        public string NameRu { get; set; } = string.Empty;
        public string NameZh { get; set; } = string.Empty;

        // -------------------- SEO URL --------------------

        public string? SlugTr { get; set; }
        public string? SlugEn { get; set; }
        public string? SlugAz { get; set; }
        public string? SlugDe { get; set; }
        public string? SlugEs { get; set; }
        public string? SlugFr { get; set; }
        public string? SlugHi { get; set; }
        public string? SlugPt { get; set; }
        public string? SlugRu { get; set; }
        public string? SlugZh { get; set; }
    }
}
```

## Dosya: ContactInformation.cs
```csharp
﻿using Microsoft.EntityFrameworkCore;

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
}```

## Dosya: InteractionCounts.cs
```csharp
﻿using Microsoft.EntityFrameworkCore;

namespace data.Owned
{
    [Owned]
    public class InteractionCounts
    {
        public int? ViewCount { get; set; }            // Görüntülenme
        public int? ShareCount { get; set; }           // Paylaşma
        public int? RecommendCount { get; set; }       // Tavsiye Etme

    }
}```

## Dosya: IsDeleted.cs
```csharp
﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace data.Owned
{
    [Owned]
    public class IsDeleted
    {
        // Silinme durumu (Soft Delete)
        public bool? IsDeletedStatu { get; set; } = false;

        // Silinme tarihi
        public DateTime? DeletedAtDate { get; set; }
    }
}
```

## Dosya: IsPrivateOrPublic.cs
```csharp
﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace data.Owned
{
    [Owned]
    public class IsPrivateOrPublic
    {
        // ==========================================
        // 1. TEMEL GÖRÜNÜRLÜK AYARLARI (Mevcut)
        // ==========================================
        public bool? IsProfilePublic { get; set; }           // Profil genel erişime açık mı?
        public bool? IsAllowPostsView { get; set; }          // Paylaşımları kimler görebilir?
        public bool? IsShowLastSeen { get; set; }            // Son görülme bilgisi
        public bool? IsShowOnlineStatus { get; set; }        // Çevrimiçi durumu (Yeni)
        public bool? IsSearchEngineIndexingAllowed { get; set; } // Google vb. aramalarda listelenme (Yeni)

        // ==========================================
        // 2. İLETİŞİM VE BAĞLANTI AYARLARI
        // ==========================================
        public bool? IsAllowFriendRequest { get; set; }      // Arkadaşlık isteği izni (Mevcut)
        public bool? IsAllowDirectMessages { get; set; }     // DM (Mesaj) izni (Yeni)
        public bool? IsAllowVoiceCalls { get; set; }         // Sesli arama izni (Yeni)
        public bool? IsAllowVideoCalls { get; set; }         // Görüntülü arama izni (Yeni)
        public bool? IsReadReceiptsEnabled { get; set; }     // Okundu bilgisi (Mavi tık) (Yeni)

        // ==========================================
        // 3. KİŞİSEL BİLGİ GİZLİLİĞİ
        // ==========================================
        public bool? IsLocationVisible { get; set; }         // Konum bilgisi paylaşımı (Yeni)
        public bool? IsShowFollowerCount { get; set; }       // Takipçi sayısını göster (Yeni)
        public bool? IsShowFollowingList { get; set; }       // Takip edilen listesini göster (Yeni)
        public bool? IsShowFollowerList { get; set; }       // Takipçi listesini göster (Yeni)

        // ==========================================
        // 4. ETKİLEŞİM VE İÇERİK YÖNETİMİ
        // ==========================================
        public bool? IsAllowComments { get; set; }           // Yorum yapma izni (Mevcut)
        public bool? IsAllowTagging { get; set; }            // Etiketlenme izni (Mevcut)
        public bool? IsAllowStorySharing { get; set; }       // Hikaye paylaşım/repost izni (Yeni)
        public bool? IsAllowPostDownloading { get; set; }    // İçerik indirme izni (Yeni)

        // ==========================================
        // 5. BİLDİRİM VE GÜVENLİK AYARLARI
        // ==========================================
        public bool? IsEmailNotificationEnabled { get; set; } // E-posta bildirimleri (Mevcut)
        public bool? IsPushNotificationEnabled { get; set; }  // Uygulama bildirimleri (Yeni)
        public bool? IsSmsNotificationEnabled { get; set; }   // SMS bildirimleri (Yeni)
        public bool? IsTwoFactorAuthEnabled { get; set; }     // İki faktörlü doğrulama (Yeni)

        // ==========================================
        // 6. VERİ POLİTİKASI VE REKLAM
        // ==========================================
        public bool? IsPersonalizedAdsEnabled { get; set; }   // Kişiselleştirilmiş reklamlar (Yeni)
        public bool? IsDataCollectionAllowed { get; set; }    // Anonim veri toplama izni (Yeni)
        public bool? IsAiTrainingAllowed { get; set; }        // Yapay zeka eğitim izni (Yeni)
    }
}```

## Dosya: Meta.cs
```csharp
﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace data.Owned
{
    [Owned]
    public class Meta
    {

        public string MetaTitle { get; set; } // Google'da görünecek başlık

        public string MetaDescription { get; set; } // Arama sonuçlarındaki açıklama

        public string FocusKeywords { get; set; } // Virgülle ayrılmış anahtar kelimeler

        public string CanonicalUrl { get; set; } // Yinelenen içerik engelleme linki

        public string OgType { get; set; } = "article"; // Open Graph tipi (Facebook/Twitter için)

        // Robot talimatları: "index, follow" vb.
        public string RobotsIndex { get; set; } = "index, follow";
    }
}
```

## Dosya: ProfileCoverGallery.cs
```csharp
﻿using Microsoft.EntityFrameworkCore;

namespace data.Owned
{
    [Owned] // Bu nitelik, EF Core'a bu sınıfın başka bir tabloya ait olduğunu söyler.
    public class ProfileCoverGallery
    {
        public string? ProfileImagePath { get; set; }
        public string? CoverImagePath { get; set; }
    }
}
```

## Dosya: UserRolesAccessPermissions.cs
```csharp
﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace data.Owned
{
    [Owned]
    public class UserRolesAccessPermissions
    {
        public bool? IsItStaff { get; set; }
    }
}
```

## Dosya: WorkingHours.cs
```csharp
﻿using Microsoft.EntityFrameworkCore;

namespace data.Owned
{
    [Owned]
    public class WorkingHours
    {
        public bool? IsActiveMonday { get; set; }
        public TimeOnly? StartTimeMonday { get; set; }
        public TimeOnly? FinishTimeMonday { get; set; }
        public bool? IsActiveTuesday { get; set; }
        public TimeOnly? StartTimeTuesday { get; set; }
        public TimeOnly? FinishTimeTuesday { get; set; }
        public bool? IsActiveWednesday { get; set; }
        public TimeOnly? StartTimeWednesday { get; set; }
        public TimeOnly? FinishTimeWednesday { get; set; }
        public bool? IsActiveThursday { get; set; }
        public TimeOnly? StartTimeThursday { get; set; }
        public TimeOnly? FinishTimeThursday { get; set; }
        public bool? IsActiveFriday { get; set; }
        public TimeOnly? StartTimeFriday { get; set; }
        public TimeOnly? FinishTimeFriday { get; set; }
        public bool? IsActiveSaturday { get; set; }
        public TimeOnly? StartTimeSaturday { get; set; }
        public TimeOnly? FinishTimeSaturday { get; set; }
        public bool? IsActiveSunday { get; set; }
        public TimeOnly? StartTimeSunday { get; set; }
        public TimeOnly? FinishTimeSunday { get; set; }
    }
}```


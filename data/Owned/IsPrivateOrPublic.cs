using Microsoft.EntityFrameworkCore;
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
}
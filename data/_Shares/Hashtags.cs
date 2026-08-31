using System;

namespace data._Shares
{
    /// <summary>
    /// HASHTAG SÖZLÜĞÜ (§21) — platform genelinde tekil etiket kaydı.
    ///
    /// Etiketler gönderi metninden ayrıştırılıp burada normalize edilerek saklanır.
    /// Aynı etiketin "#Kahve", "#kahve", "#KAHVE" yazımları TEK satıra düşer:
    /// tekillik <see cref="NormalizedName"/> üzerindedir.
    ///
    /// TÜRKÇE KARAKTER KURALI: Normalizasyon uygulama katmanında yapılır ve Türkçe
    /// "İ/ı" sorununu önlemek için invariant culture ile küçük harfe çevrilir,
    /// ardından aksanlar sadeleştirilir. Bu kural tek bir yardımcı sınıfta
    /// (HashtagNormalizer) toplanmalı ve hem yazma hem arama yolunda kullanılmalıdır.
    /// </summary>
    public class Hashtags : SocialEntityBase
    {
        /// <summary>Etiketin kullanıcıya gösterilen hâli ("#" olmadan, ör. "KahveKeyfi").</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Normalize edilmiş hâl (küçük harf, aksansız). GLOBAL TEKİLDİR ve tüm
        /// aramalar bu kolon üzerinden yapılır.
        /// </summary>
        public string NormalizedName { get; set; } = string.Empty;

        /// <summary>SEO uyumlu URL parçası ("kahve-keyfi").</summary>
        public string? Slug { get; set; }

        /// <summary>Etiketin açıklaması (editoryal — konu sayfası için).</summary>
        public string? Description { get; set; }

        /// <summary>
        /// Etiketi kullanan toplam gönderi sayısı (ÖNBELLEK).
        /// Kaynak: <see cref="PostHashtags"/>. Trend hesabında kullanılır.
        /// </summary>
        public int PostCount { get; set; }

        /// <summary>Son 24 saatteki kullanım sayısı (önbellek — trend listesi için).</summary>
        public int RecentUsageCount { get; set; }

        /// <summary>Etiketin en son kullanıldığı an (UTC).</summary>
        public DateTime? LastUsedAtUtc { get; set; }

        /// <summary>Editör tarafından öne çıkarıldı mı?</summary>
        public bool IsFeatured { get; set; }

        /// <summary>
        /// Etiket engellendi mi? (spam veya politika ihlali). Engelli etiketler
        /// aramada ve trendlerde görünmez; mevcut gönderiler etkilenmez.
        /// </summary>
        public bool IsBlocked { get; set; }

        /// <summary>Engelleme gerekçesi.</summary>
        public string? BlockReason { get; set; }
    }

    /// <summary>
    /// GÖNDERİ–HASHTAG İLİŞKİSİ (§21).
    /// TEKİLLİK: (PostId, HashtagId) — aynı etiket bir gönderide bir kez sayılır.
    /// </summary>
    public class PostHashtags : SocialEntityBase
    {
        /// <summary>Etiketi içeren gönderi (Posts.Id).</summary>
        public Guid PostId { get; set; }

        /// <summary>Kullanılan etiket (Hashtags.Id).</summary>
        public Guid HashtagId { get; set; }

        /// <summary>
        /// Etiketin metin içindeki ilk geçiş konumu (0 tabanlı). Aynı etiket birden
        /// çok kez geçse bile ilişki tek satırdır; konum bilgisi vurgulama içindir.
        /// </summary>
        public int? StartIndex { get; set; }
    }
}

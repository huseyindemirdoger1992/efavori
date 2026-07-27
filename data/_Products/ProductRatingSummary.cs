using System;

namespace data._Products
{
    /// <summary>
    /// ÜRÜN PUAN ÖZETİ / İSTATİSTİK — ürün detay sayfasındaki "4.6 ★ (1.243 değerlendirme)"
    /// bloğunun ve yıldız dağılım çubuklarının veri kaynağı.
    ///
    /// TASARIM GEREKÇESİ: Ortalama, ana <see cref="Products"/> tablosuna YAZILMAZ.
    /// Her yeni yorumda ürün satırını kilitlemek yerine, özet burada AYRI bir kayıtta
    /// tutulur ve bir BackgroundService (ör. AIReviewSummaryRecalculator) tarafından
    /// yeniden hesaplanır. ProductId benzersizdir (ürün başına tek özet satırı).
    ///
    /// Bu tablo TÜRETİLMİŞ (denormalize) veridir; kaynak gerçek yorumlardır
    /// (<see cref="ProductReviews"/>, yalnızca Status = Approved &amp; ParentReviewId = null).
    /// Kaybolsa bile yorumlardan yeniden üretilebilir.
    /// </summary>
    public class ProductRatingSummary : ProductEntityBase
    {
        /// <summary>Özeti tutulan ürün (Products.Id) — benzersiz.</summary>
        public Guid ProductId { get; set; }

        /// <summary>Onaylı kök yorum (puanlı değerlendirme) sayısı.</summary>
        public int RatingCount { get; set; }

        /// <summary>
        /// Onaylı yorum (metinli) sayısı — puansız ama metinli olanlar dâhil olabilir;
        /// RatingCount'tan farklı sayılabilir.
        /// </summary>
        public int ReviewCount { get; set; }

        /// <summary>Ortalama yıldız puanı (0..5). Yorum yoksa 0.</summary>
        public decimal AverageRating { get; set; }

        // ── Yıldız dağılımı (dağılım çubukları için) ─────────────────────────
        /// <summary>5 yıldız veren yorum sayısı.</summary>
        public int FiveStarCount { get; set; }

        /// <summary>4 yıldız veren yorum sayısı.</summary>
        public int FourStarCount { get; set; }

        /// <summary>3 yıldız veren yorum sayısı.</summary>
        public int ThreeStarCount { get; set; }

        /// <summary>2 yıldız veren yorum sayısı.</summary>
        public int TwoStarCount { get; set; }

        /// <summary>1 yıldız veren yorum sayısı.</summary>
        public int OneStarCount { get; set; }

        // ── Alt puan ortalamaları (opsiyonel kırılımlar) ─────────────────────
        /// <summary>Kalite alt puanı ortalaması (0..5; veri yoksa null).</summary>
        public decimal? AverageQualityRating { get; set; }

        /// <summary>Kargo alt puanı ortalaması (0..5; veri yoksa null).</summary>
        public decimal? AverageShippingRating { get; set; }

        /// <summary>Fiyat/performans alt puanı ortalaması (0..5; veri yoksa null).</summary>
        public decimal? AverageValueRating { get; set; }

        /// <summary>Doğrulanmış satın alma ile yapılan yorum sayısı.</summary>
        public int VerifiedPurchaseCount { get; set; }

        /// <summary>Özetin en son yeniden hesaplandığı an (UTC).</summary>
        public DateTime? LastRecalculatedAtUtc { get; set; }
    }
}

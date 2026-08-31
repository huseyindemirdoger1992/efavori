using Microsoft.EntityFrameworkCore;

namespace data.Owned
{
    /// <summary>
    /// SEO META VERİSİ (§55) — owned tip olarak sahibinin tablosuna gömülür.
    ///
    /// Kullanım kuralı: SEO alanları YALNIZCA arama motorunda indekslenmesi anlamlı
    /// olan varlıklarda bulunur (ürün, kategori, makale, mağaza, herkese açık gönderi).
    /// Sosyal kullanıcı profillerine SEO alanı doldurulmaz — profil görünürlüğü
    /// gizlilik ayarlarına tabidir ve arama motoru izni ayrı bir boolean ile yönetilir
    /// (<c>UserProfiles.AllowSearchEngineIndexing</c>).
    ///
    /// Tüm alanlar NULLABLE'dır: SEO isteğe bağlıdır, boş bırakıldığında uygulama
    /// katmanı içerikten otomatik türetir.
    /// </summary>
    [Owned]
    public class Meta
    {
        /// <summary>Arama sonucunda görünecek başlık (önerilen: 50-60 karakter).</summary>
        public string? MetaTitle { get; set; }

        /// <summary>Arama sonucundaki açıklama (önerilen: 150-160 karakter).</summary>
        public string? MetaDescription { get; set; }

        /// <summary>Virgülle ayrılmış odak anahtar kelimeler.</summary>
        public string? FocusKeywords { get; set; }

        /// <summary>Yinelenen içerik bildirimi için kanonik adres.</summary>
        public string? CanonicalUrl { get; set; }

        /// <summary>Open Graph içerik tipi ("article", "product", "profile").</summary>
        public string? OgType { get; set; } = "article";

        /// <summary>Arama robotu talimatı ("index, follow" / "noindex, nofollow").</summary>
        public string? RobotsIndex { get; set; } = "index, follow";
    }
}

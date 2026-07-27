using System;
using data._Attribute; // mevcut Language enum'ı (10 dil) yeniden kullanılır

namespace data._Products
{
    /// <summary>
    /// Ürünün DİLE ÖZGÜ tüm metinleri — ana tabloyu metin yükünden kurtarır.
    /// (ProductId, Language) benzersizdir; bir ürün için en fazla 10 satır oluşur.
    ///
    /// AI ÇEVİRİ AKIŞI (hizmet bedeli karşılığı otomatik çeviri):
    ///  1. Satıcı kaynak dilde (genelde Tr) metinleri girer →
    ///     o satır IsOriginalLanguage = true, Source = Manual, Status = Completed olur.
    ///  2. Satıcı AI çeviri hizmetini satın alırsa hedef diller için
    ///     Status = Pending satırlar açılır (metinler boş).
    ///  3. AI çeviri BackgroundService'i Pending satırları çeker, kaynak satırdan
    ///     çevirir; Source = AIGenerated, Status = Completed / NeedsReview yapar,
    ///     BilledCharacterCount ile faturalandırma karakter sayısını yazar.
    ///  4. IsManuallyEdited = true olan satırın ÜZERİNE AI ASLA yazmaz
    ///     (Attribute System V3'teki manuel koruma kuralının aynısı).
    /// </summary>
    public class ProductTranslations : ProductEntityBase
    {
        /// <summary>Bağlı ürün (Products.Id).</summary>
        public Guid ProductId { get; set; }

        /// <summary>Bu satırın dili (mevcut 10 dillik Language enum'ı).</summary>
        public Language Language { get; set; }

        /// <summary>
        /// Bu dil, satıcının metinleri girdiği ORİJİNAL/KAYNAK dil mi?
        /// Ürün başına yalnızca BİR satırda true olur; AI çevirileri bu satırı
        /// kaynak alır.
        /// </summary>
        public bool IsOriginalLanguage { get; set; }

        // ── Vitrin metinleri ─────────────────────────────────────────────────
        /// <summary>Ürün adı / başlığı (bu dilde).</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Alt başlık (opsiyonel — başlığın altında görünen kısa slogan).</summary>
        public string? SubTitle { get; set; }

        /// <summary>Kısa açıklama (liste/kart görünümleri ve özet alanı).</summary>
        public string? ShortDescription { get; set; }

        /// <summary>Detaylı açıklama (HTML — ürün sayfası ana içerik).</summary>
        public string? DescriptionHtml { get; set; }

        /// <summary>
        /// Öne çıkan madde işaretleri (Amazon "bullet points" karşılığı).
        /// JSON dizi olarak saklanır: ["6.1 inç OLED ekran","IP68 dayanıklılık",...]
        /// </summary>
        public string? BulletPointsJson { get; set; }

        /// <summary>Site içi arama için ek anahtar kelimeler (virgülle ayrılmış; vitrine yazılmaz).</summary>
        public string? SearchKeywords { get; set; }

        // ── SEO alanları ─────────────────────────────────────────────────────
        /// <summary>SEO dostu URL parçası (bu dilde tekil; ör. "akilli-telefon-x-128gb").</summary>
        public string Slug { get; set; } = string.Empty;

        /// <summary>Meta title (Google'da görünecek başlık; boşsa Name kullanılır).</summary>
        public string? MetaTitle { get; set; }

        /// <summary>Meta description (arama sonuçlarındaki açıklama).</summary>
        public string? MetaDescription { get; set; }

        /// <summary>Odak anahtar kelimeler (virgülle ayrılmış).</summary>
        public string? FocusKeywords { get; set; }

        /// <summary>Canonical URL (yinelenen içerik engelleme; boşsa slug'dan üretilir).</summary>
        public string? CanonicalUrl { get; set; }

        /// <summary>Open Graph başlığı (sosyal paylaşım; boşsa MetaTitle/Name kullanılır).</summary>
        public string? OgTitle { get; set; }

        /// <summary>Open Graph açıklaması.</summary>
        public string? OgDescription { get; set; }

        /// <summary>Robots talimatı (varsayılan "index, follow").</summary>
        public string RobotsIndex { get; set; } = "index, follow";

        // ── Çeviri yönetişimi (AI entegrasyonu) ──────────────────────────────
        /// <summary>Çevirinin kaynağı/yöntemi (Manual / AIGenerated).</summary>
        public TranslationSource Source { get; set; } = TranslationSource.Manual;

        /// <summary>Çeviri durumu (Pending / Completed / NeedsReview ...).</summary>
        public TranslationStatus Status { get; set; } = TranslationStatus.Completed;

        /// <summary>
        /// Admin/satıcı bu çeviriyi elle düzenledi mi?
        /// true ise AI bu satırın üzerine ASLA yazmaz (ezme kilidi).
        /// </summary>
        public bool IsManuallyEdited { get; set; }

        /// <summary>Çevirinin türetildiği kaynak dil (AI çevirilerde dolu; ör. Tr).</summary>
        public Language? TranslatedFromLanguage { get; set; }

        /// <summary>Çeviriyi üreten AI modeli (ör. "gemini-2.5-pro").</summary>
        public string? AiModel { get; set; }

        /// <summary>AI çeviri güven skoru (0..1).</summary>
        public decimal? AiConfidence { get; set; }

        /// <summary>AI çevirinin üretildiği an (UTC).</summary>
        public DateTime? AiTranslatedAtUtc { get; set; }

        /// <summary>
        /// Bu çeviri için faturalandırılan kaynak karakter sayısı
        /// (hizmet bedeli karşılığı çeviri ücretlendirmesinin dayanağı).
        /// </summary>
        public int? BilledCharacterCount { get; set; }

        /// <summary>Son çeviri denemesindeki hata mesajı (Status = Failed iken dolu).</summary>
        public string? ErrorMessage { get; set; }
    }
}

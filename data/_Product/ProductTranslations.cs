using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// ÜRÜN İÇERİĞİNİN TEK GERÇEK KAYNAĞI — dil başına bir satır.
    /// BENZERSİZLİK: (ProductId, LanguageCode) — Fluent API'de unique index tanımlanır.
    ///
    /// KAYNAK SATIR: LanguageCode == Products.SourceLanguageCode olan satır orijinal içeriktir
    /// (insan girişi veya marketplace'ten içe aktarım). Diğer satırlar AI çevirisidir.
    ///
    /// AI KORUMA KURALI: IsManuallyEdited = true olan satırın ÜZERİNE AI asla yazmaz.
    ///
    /// KAYIPSIZ İÇE AKTARIM ALANLARI:
    ///   BulletPointsJson → Amazon 5 madde / Mercado Libre highlights / Temu key specs
    ///   SearchKeywords   → Amazon "generic keywords" (görünmez arka plan arama terimleri, Tags'ten farklı)
    ///   RichContentJson  → Amazon A+ (EBC) modülleri, AliExpress detay blokları, garanti/kutu içeriği
    ///                      bölümleri gibi yapısal zengin içerik (yeniden render edilebilir JSON)
    ///   Subtitle         → eBay alt başlık
    /// </summary>
    public class ProductTranslations
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // Ürün (Products.Id)

        // "tr","en","az","de","es","fr","hi","pt","ru","zh"
        public string LanguageCode { get; set; } = "tr";

        // === İçerik Alanları ===
        public string? Name { get; set; } // Ürün adı / başlık
        public string? Subtitle { get; set; } // Alt başlık (eBay subtitle) — null ise kullanılmaz
        public string? ShortDescription { get; set; } // Kısa açıklama
        public string? FullDescription { get; set; } // Tam açıklama (HTML destekli)
        public string? Tags { get; set; } // Görünür etiketler (virgülle ayrılmış)

        public string? BulletPointsJson { get; set; } // Öne çıkan maddeler (JSON dizi): ["4K çözünürlük","HDR10+",...]
        public string? SearchKeywords { get; set; } // Arka plan arama terimleri (sayfada görünmez, aramada kullanılır)
        public string? RichContentJson { get; set; } // Zengin içerik modülleri (A+ / detay blokları) — yapısal JSON

        public string? ExternalButtonText { get; set; } // Harici ürün buton metni (ProductType = "External")

        // === İçerik Kaynağı / İzlenebilirlik ===
        // "Human"       → satıcı/admin elle girdi
        // "Import"      → marketplace'ten içe aktarıldı (orijinal kaynak içerik)
        // "AiGenerated" → AI sıfırdan üretti (kaynak dil zenginleştirme)
        // "AiTranslated"→ AI başka dilden çevirdi
        public string? ContentSource { get; set; } = "Human";
        public string? TranslatedFromLanguageCode { get; set; } // AI çevirisiyse hangi dilden çevrildiği (kalite takibi)

        // === AI İşlem Yönetimi ===
        // null → işlenmedi | true → başarılı | false → hata
        public bool? AiStatus { get; set; }
        public string? AiErrorMessage { get; set; }
        public int AiRetryCount { get; set; } = 0;
        public DateTime? AiProcessedAt { get; set; }

        // Satıcı elle düzenlediyse AI bir daha ÜZERİNE YAZMAZ
        public bool IsManuallyEdited { get; set; } = false;

        // === Orijinal Yedek (AI kapatılırsa geri yüklenir — Products'taki AiOriginal* deseni buraya taşındı) ===
        public string? AiOriginalName { get; set; }
        public string? AiOriginalShortDescription { get; set; }
        public string? AiOriginalFullDescription { get; set; }
        public string? AiOriginalTags { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public IsDeleted? IsDeleted { get; set; } = new();
    }
}

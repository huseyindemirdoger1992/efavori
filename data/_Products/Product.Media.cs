using System;
using data._Attribute; // Language enum'ı

namespace data._Products
{
    /// <summary>
    /// Ürün MEDYA bağlantısı — görsel, video, 3B model ve doküman.
    ///
    /// Kaynak kuralı (ikisinden yalnızca biri dolar):
    ///  • <see cref="MediaId"/> dolu       → dosya, mevcut _Galleries.Media
    ///    altyapısında barındırılır (boyut varyasyonları, AVIF vb. oradan gelir).
    ///  • <see cref="ExternalUrl"/> dolu   → dış kaynak (ör. YouTube embed).
    ///
    /// Kapsam kuralı:
    ///  • <see cref="ProductVariantId"/> = null → ürünün genel galerisi.
    ///  • <see cref="ProductVariantId"/> dolu  → o varyanta özel medya
    ///    (ör. Siyah varyant seçilince siyah ürün fotoğrafları öne gelir).
    /// </summary>
    public class ProductMedia : ProductEntityBase
    {
        /// <summary>Bağlı ürün (Products.Id).</summary>
        public Guid ProductId { get; set; }

        /// <summary>Bağlı varyant (ProductVariants.Id — null = ürün geneli medya).</summary>
        public Guid? ProductVariantId { get; set; }

        /// <summary>Medya türü (Image / Video / VideoEmbed / Model3D / Document).</summary>
        public ProductMediaType MediaType { get; set; } = ProductMediaType.Image;

        /// <summary>İç medya kaydı (_Galleries.Media.Id — sunucuda barındırılan dosya).</summary>
        public Guid? MediaId { get; set; }

        /// <summary>Dış kaynak URL'i (MediaType = VideoEmbed için; ör. YouTube linki).</summary>
        public string? ExternalUrl { get; set; }

        /// <summary>Kapak (ana) görsel mi? Kapsam başına (ürün/varyant) bir adet true.</summary>
        public bool IsCover { get; set; }

        /// <summary>Galerideki sıralama.</summary>
        public int DisplayOrder { get; set; }

        /// <summary>Medya aktif mi? (false = geçici gizli, kayıt korunur).</summary>
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// Ürün medyasının DİLE ÖZGÜ SEO metinleri (alt text / başlık / altyazı).
    /// Görsel SEO (Google Images) ve erişilebilirlik için tutulur; ürün metinleriyle
    /// aynı AI çeviri akışına dâhildir. (ProductMediaId, Language) benzersizdir.
    /// </summary>
    public class ProductMediaTranslations : ProductEntityBase
    {
        /// <summary>Bağlı medya satırı (ProductMedia.Id).</summary>
        public Guid ProductMediaId { get; set; }

        /// <summary>Dil.</summary>
        public Language Language { get; set; }

        /// <summary>Görselin alt (alternatif) metni — img alt="" / erişilebilirlik.</summary>
        public string? AltText { get; set; }

        /// <summary>Medya başlığı (title attribute / lightbox başlığı).</summary>
        public string? Title { get; set; }

        /// <summary>Altyazı / açıklama (galeri altında gösterilen metin).</summary>
        public string? Caption { get; set; }

        /// <summary>Çevirinin kaynağı (Manual / AIGenerated).</summary>
        public TranslationSource Source { get; set; } = TranslationSource.Manual;

        /// <summary>Çeviri durumu.</summary>
        public TranslationStatus Status { get; set; } = TranslationStatus.Completed;

        /// <summary>Elle düzenlendi mi? (AI ezme kilidi).</summary>
        public bool IsManuallyEdited { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Ürünün galeri dışı medya kaynakları: video URL'leri, 360 derece görünüm,
    /// embed içerikler ve PDF/doküman dosyaları.
    /// Sistemde yüklü dosyalar MediaId ile (Media.Id), harici kaynaklar Url ile bağlanır.
    ///
    /// REVİZYON: LanguageCode eklendi — dile özgü medya desteklenir.
    /// Örn: Almanca kullanım kılavuzu PDF'i yalnızca "de" sayfasında gösterilir.
    /// Null = tüm dillerde gösterilir (dil bağımsız medya).
    /// </summary>
    public class ProductExternalMedias
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // Ürün (Products.Id)

        // "VideoUrl", "View360", "Embed", "Document"
        public string? MediaType { get; set; } = "VideoUrl";

        public string? Url { get; set; } // Harici kaynak adresi (YouTube, Vimeo, 360 viewer vb.)
        public Guid? MediaId { get; set; } // Sistemde yüklü dosya (Media.Id) — PDF/doküman için

        public string? Title { get; set; } // Gösterim başlığı (Örn: "Kurulum Kılavuzu")
        public string? LanguageCode { get; set; } // Medyanın dili — null = tüm dillerde gösterilir
        public int DisplayOrder { get; set; } = 0; // Gösterim sırası

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
    }
}

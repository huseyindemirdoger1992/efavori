using data._Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace data._Product
{
    public class ProductTranslations
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; }              // Products.Id

        // "tr","en","az","de","es","fr","hi","pt","ru","zh"
        public string LanguageCode { get; set; } = "tr";

        // === Products çevrilebilir alanlarının birebir karşılığı ===
        public string? Name { get; set; }
        public string? ShortDescription { get; set; }
        public string? FullDescription { get; set; }
        public string? Tags { get; set; }

        // === AI çeviri yönetimi (mevcut AiContentStatus deseninizle uyumlu) ===
        public bool? IsAiTranslated { get; set; } = false;
        // null → çevrilmedi | true → başarılı | false → hata
        public bool? AiTranslationStatus { get; set; }
        public string? AiErrorMessage { get; set; }
        public int AiRetryCount { get; set; } = 0;
        public DateTime? AiProcessedAt { get; set; }

        // Satıcı elle düzenlediyse AI bir daha ÜZERİNE YAZMAZ
        public bool IsManuallyEdited { get; set; } = false;

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public IsDeleted? IsDeleted { get; set; } = new();
    }
}

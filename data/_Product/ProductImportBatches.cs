using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Her toplu ürün yükleme işleminin takibini sağlar.
    /// Satıcı CSV yüklediğinde bir batch oluşturulur, işlem sonucu burada saklanır.
    /// Products tablosundaki ImportBatchId bu tabloya referans verir.
    /// </summary>
    public class ProductImportBatches
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }          // İşlemi yapan satıcı
        public Guid StoreId { get; set; }         // Bağlı olduğu mağaza
        public Guid ImportProfileId { get; set; } // Kullanılan import profili

        // === Dosya Bilgileri ===
        public Guid? MediaId { get; set; }         // Yüklenen CSV dosyasının Media tablosundaki ID'si
        public string? OriginalFileName { get; set; } // Yüklenen dosyanın orijinal adı

        // === İşlem Durumu ===
        // "Pending" = Beklemede, "Processing" = İşleniyor, "Completed" = Tamamlandı,
        // "PartiallyCompleted" = Kısmen Tamamlandı, "Failed" = Başarısız
        public string? Status { get; set; } = "Pending";

        // === İstatistikler ===
        public int TotalRows { get; set; } = 0;       // CSV'deki toplam satır sayısı
        public int SuccessCount { get; set; } = 0;     // Başarıyla aktarılan ürün sayısı
        public int FailCount { get; set; } = 0;        // Hatalı satır sayısı
        public int SkipCount { get; set; } = 0;        // Atlanan satır sayısı (zaten mevcut vb.)

        // === Hata Detayları (JSON) ===
        // Hatalı satırların detaylı raporu
        // Örn: [{"Row":5,"Error":"SKU boş olamaz"},{"Row":12,"Error":"Kategori bulunamadı"}]
        public string? ErrorDetailsJson { get; set; }

        public DateTime? StartedAt { get; set; }    // İşlem başlangıç zamanı
        public DateTime? CompletedAt { get; set; }  // İşlem bitiş zamanı
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public IsDeleted? IsDeleted { get; set; } = new();
    }
}

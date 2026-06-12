using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product.BulkWordPressProductImport
{
    /// <summary>
    /// İçe aktarım OTURUMU (job/run). WooCommerce "Dışa Aktar" CSV dosyasının aktarım
    /// sürecini baştan sona temsil eder ve KALDIĞI YERDEN DEVAM (resume) yeteneğinin kalbidir.
    ///
    /// HAFIZA MANTIĞI:
    ///   - SourceMediaId  → galeriden seçilen CSV dosyası (Media.Id). Sayfa kapansa bile job
    ///                      hangi dosyayla çalıştığını hatırlar.
    ///   - LastProcessedRowIndex → işlenen son satır. Yeniden başlatıldığında buradan devam eder.
    ///   - FieldMappingJson / AnalysisReportJson → analiz ve alan eşleşmeleri job içinde saklanır,
    ///     böylece kullanıcı her açışında baştan eşleştirme yapmaz.
    ///
    /// IDEMPOTENT/RESUMABLE: Gerçek satır-bazlı ilerleme ProductImportRows tablosunda tutulur;
    /// bu tablo job seviyesindeki özet sayaçları ve durumu yönetir.
    /// </summary>
    public class ProductImportJobs
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // === İlişkiler (ID bazlı, navigation property kullanılmaz) ===
        public Guid UserId { get; set; }                 // İçe aktarımı başlatan kullanıcı (Users.Id)
        public Guid StoreId { get; set; }                // Hedef mağaza (Store.Id)
        public Guid? ProfileId { get; set; }             // İsteğe bağlı eşleme profili (ProductImportProfiles.Id)
        public Guid SourceMediaId { get; set; }          // Galeriden seçilen kaynak CSV dosyası (Media.Id) — HAFIZA

        // === Tanım ===
        public string? Name { get; set; }                // Job adı (Örn: "Başer Elektrik - Aralık Aktarımı")

        // "CSV", "WooCommerce", "Trendyol", "XML", "JSON" ... — bu akışta varsayılan WooCommerce
        public string? SourceType { get; set; } = "WooCommerce";
        public string? Delimiter { get; set; } = ",";    // CSV ayıracı (WooCommerce dışa aktarımı virgül kullanır)
        public string? Encoding { get; set; } = "UTF-8"; // Karakter seti (BOM'lu UTF-8)

        // === Durum Makinesi ===
        // "Draft"     → oluşturuldu, henüz hazır değil (analiz/eşleştirme sürüyor)
        // "Ready"     → tüm eşleştirmeler onaylandı, başlatılabilir
        // "Running"   → aktif olarak satır işliyor
        // "Paused"    → kullanıcı duraklattı (resume edilebilir)
        // "Completed" → tüm satırlar işlendi
        // "Failed"    → kurtarılamaz hata ile durdu
        // "Cancelled" → kullanıcı iptal etti
        public string? Status { get; set; } = "Draft";

        // === Canlı Sayaçlar (Dashboard panelini besler) ===
        public int TotalCount { get; set; } = 0;         // Toplam Ürün (CSV satır sayısı)
        public int ImportedCount { get; set; } = 0;      // Aktarılan
        public int FailedCount { get; set; } = 0;        // Hatalı
        public int SkippedCount { get; set; } = 0;       // Atlanan (duplicate / kapsam dışı)
        // Bekleyen = TotalCount - ImportedCount - FailedCount - SkippedCount (uygulama katmanında hesaplanır)
        public decimal SuccessRate { get; set; } = 0m;   // Başarı Oranı (%) — denormalize, raporlama kolaylığı

        // === Kaldığı Yerden Devam (Resume Cursor) ===
        public int LastProcessedRowIndex { get; set; } = 0; // İşlenen son satırın sıfır-tabanlı indeksi
        public int BatchSize { get; set; } = 500;           // Tek seferde işlenecek satır sayısı (bellek dostu)

        // === Eşleşme & Analiz Hafızası (JSON) ===
        public string? FieldMappingJson { get; set; }       // Kolon → sistem alanı eşlemeleri (onaylanmış)
        public string? AnalysisReportJson { get; set; }     // AŞAMA-1 kolon/veri tipi/boş alan analiz raporu
        public string? StrategyReportJson { get; set; }     // AŞAMA-13 import strateji raporu

        // === Varsayılanlar (eşlenemeyen veriler için) ===
        public Guid? DefaultWarehouseId { get; set; }       // Stok yazılacak varsayılan depo (Warehouse.Id)
        public int? DefaultCategoryId { get; set; }         // Eşlenemeyen ürünler için varsayılan kategori (CategoriesTr.Id)
        public string? DefaultCurrency { get; set; } = "TRY"; // CSV fiyatlarının para birimi

        // === Tarih Bilgileri ===
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
        public DateTime? StartedAt { get; set; }                    // İlk başlatılma tarihi
        public DateTime? LastRunAt { get; set; }                    // Son batch çalıştırma tarihi
        public DateTime? CompletedAt { get; set; }                  // Tamamlanma/iptal tarihi
        public DateTime? UpdatedAt { get; set; }                    // Son güncelleme tarihi

        public string? LastErrorMessage { get; set; }              // Job seviyesinde son kritik hata

        public IsDeleted? IsDeleted { get; set; } = new();         // Silinme durumu (soft delete)
    }
}

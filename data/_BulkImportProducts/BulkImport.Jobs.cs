using System;

namespace data._BulkImportProducts
{
    /// <summary>
    /// İÇE AKTARIM İŞİ (bir çalıştırma / batch). Kullanıcı bir profili tetiklediğinde
    /// (dosya yükleyerek veya API'yi çalıştırarak) bir job oluşur. Job; kaynak
    /// verinin alınması → ayrıştırılması → (gerekirse) kullanıcının eşleştirmeyi
    /// tamamlaması → ürünlere dönüştürülmesi yaşam döngüsünü izler.
    ///
    /// Arka plan işleme deseni Attribute V3'teki AiGenerationJob ile aynıdır:
    /// <see cref="LeasedBy"/> + <see cref="LeasedUntilUtc"/> ile kiralama (çift işleme
    /// koruması) ve <see cref="IdempotencyKey"/> ile tekilleştirme yapılır.
    ///
    /// İlerleme sayaçları (Total/Processed/Success/Fail/Skip) canlı ilerleme
    /// çubuğu ve "self-healing counter" için tutulur; kaynağı gerçek satırlardır
    /// (<see cref="ImportRow"/>).
    /// </summary>
    public class ImportJob : BulkImportEntityBase
    {
        /// <summary>İşi başlatan kullanıcı (Users.Id) — zorunlu.</summary>
        public Guid UserId { get; set; }

        /// <summary>Kullanılan profil (ImportProfile.Id).</summary>
        public Guid ImportProfileId { get; set; }

        /// <summary>Hedef mağaza (Store.Id — profilden devralınır, override edilebilir).</summary>
        public Guid? StoreId { get; set; }

        /// <summary>Denormalize platform referansı (izleme/raporlama; IntegrationPlatform.Id).</summary>
        public Guid IntegrationPlatformId { get; set; }

        /// <summary>Bu çalıştırmanın kaynak türü (profilden devralınır).</summary>
        public ImportSourceType SourceType { get; set; }

        // ── Kaynak referansı ──────────────────────────────────────────────────
        /// <summary>
        /// Dosya tabanlı içe aktarımda yüklenen dosyanın medya kaydı
        /// (data._Galleries.Media.Id). API/feed işlerinde null.
        /// </summary>
        public Guid? SourceMediaId { get; set; }

        /// <summary>Yüklenen dosyanın orijinal adı (izleme için).</summary>
        public string? OriginalFileName { get; set; }

        /// <summary>API/feed işinde kullanılan çekim adresi (izleme için).</summary>
        public string? SourceUrl { get; set; }

        // ── Bu çalıştırmaya sabitlenen davranışlar (profil anlık kopyası) ─────
        /// <summary>Oluştur/güncelle davranışı (job anında profilden kopyalanır).</summary>
        public ImportMode Mode { get; set; } = ImportMode.CreateOnly;

        /// <summary>Yineleme stratejisi.</summary>
        public DuplicateStrategy DuplicateStrategy { get; set; } = DuplicateStrategy.Skip;

        /// <summary>Eşleşme anahtarı.</summary>
        public ImportMatchKey MatchKey { get; set; } = ImportMatchKey.Gtin;

        // ── Durum + kuyruk kontrolü (lease/idempotency) ──────────────────────
        /// <summary>İşin durumu.</summary>
        public ImportJobStatus Status { get; set; } = ImportJobStatus.Pending;

        /// <summary>
        /// Tekilleştirme anahtarı (global tekil). Ör: hash(UserId + ProfileId +
        /// dosya hash'i / feed URL). Aynı dosya/çalıştırma ikinci kez kuyruklanmaz.
        /// </summary>
        public string? IdempotencyKey { get; set; }

        /// <summary>İşi kiralayan worker kimliği (makine/instance).</summary>
        public string? LeasedBy { get; set; }

        /// <summary>Kiralama bitiş anı (UTC) — süresi dolarsa iş yeniden alınabilir.</summary>
        public DateTime? LeasedUntilUtc { get; set; }

        /// <summary>Deneme sayısı.</summary>
        public int AttemptCount { get; set; }

        /// <summary>İzin verilen maksimum deneme.</summary>
        public int MaxAttempts { get; set; } = 3;

        /// <summary>Sonraki yeniden deneme anı (UTC).</summary>
        public DateTime? NextRetryAtUtc { get; set; }

        // ── İlerleme sayaçları (self-healing; kaynak: ImportRow) ─────────────
        /// <summary>Ayrıştırılan toplam satır sayısı.</summary>
        public int TotalRows { get; set; }

        /// <summary>İşlenmiş (herhangi bir sonuca ulaşmış) satır sayısı.</summary>
        public int ProcessedRows { get; set; }

        /// <summary>Başarıyla oluşturulan ürün sayısı.</summary>
        public int CreatedCount { get; set; }

        /// <summary>Güncellenen ürün sayısı.</summary>
        public int UpdatedCount { get; set; }

        /// <summary>Atlanan satır sayısı (yineleme + geçersiz).</summary>
        public int SkippedCount { get; set; }

        /// <summary>Hatalı satır sayısı.</summary>
        public int FailedCount { get; set; }

        /// <summary>İnceleme bekleyen satır sayısı.</summary>
        public int NeedsReviewCount { get; set; }

        /// <summary>
        /// Kesintide devam edebilmek için en son işlenen satır indeksi (checkpoint).
        /// </summary>
        public int LastProcessedRowIndex { get; set; }

        // ── Zaman damgaları + sonuç ──────────────────────────────────────────
        /// <summary>Kaynak alım/işleme başlangıcı (UTC).</summary>
        public DateTime? StartedAtUtc { get; set; }

        /// <summary>Tamamlanma anı (UTC).</summary>
        public DateTime? CompletedAtUtc { get; set; }

        /// <summary>Sonuç özeti (ör. "1.240 ürün: 1.100 yeni, 90 güncel, 50 atlandı").</summary>
        public string? ResultSummary { get; set; }

        /// <summary>Genel hata mesajı (Status = Failed iken).</summary>
        public string? ErrorMessage { get; set; }

        /// <summary>Satır bazlı hata özetlerinin JSON kopyası (opsiyonel rapor).</summary>
        public string? ErrorDetailsJson { get; set; }
    }
}

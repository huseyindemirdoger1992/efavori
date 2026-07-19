using System;

namespace data._Attribute
{
    /// <summary>
    /// Yapay zekâ üretim iş kuyruğu. İki BackgroundService (Category Analyzer ve
    /// Option Generator) işleri buradan çeker.
    ///
    /// Çift işleme koruması: <see cref="LeasedBy"/> + <see cref="LeasedUntilUtc"/>
    /// ile kiralama (lease) yapılır; birden çok worker/örnek aynı işi kapamaz.
    /// Idempotency: <see cref="IdempotencyKey"/> global tekildir; aynı kategori/iş
    /// ikinci kez kuyruğa girmez. Yeniden deneme: <see cref="AttemptCount"/> /
    /// <see cref="MaxAttempts"/> / <see cref="NextRetryAtUtc"/>.
    /// </summary>
    public class AiGenerationJob : AttributeEntityBase
    {
        /// <summary>İşin türü (kategori analizi / option üretimi / çeviri...).</summary>
        public AiJobType JobType { get; set; }

        /// <summary>İşin durumu.</summary>
        public AiJobStatus Status { get; set; } = AiJobStatus.Pending;

        /// <summary>Hedef kategori (analiz işleri için; CategoriesProduct.Id — Guid).</summary>
        public Guid? TargetCategoryId { get; set; }

        /// <summary>Hedef attribute (option/çeviri üretimi için).</summary>
        public Guid? TargetAttributeDefinitionId { get; set; }

        /// <summary>Öncelik (küçük = önce).</summary>
        public int Priority { get; set; }

        /// <summary>
        /// Tekilleştirme anahtarı (global tekil). Ör: hash(JobType + kategori yolu +
        /// PromptVersion). Aynı iş tekrar kuyruklanmaz.
        /// </summary>
        public string IdempotencyKey { get; set; } = string.Empty;

        /// <summary>Analiz edilen kategori yolunun anlık kopyası (izlenebilirlik).</summary>
        public string? CategoryPathSnapshot { get; set; }

        /// <summary>Kullanılan prompt sürümü.</summary>
        public string? PromptVersion { get; set; }

        /// <summary>Kullanılan prompt hash'i.</summary>
        public string? PromptHash { get; set; }

        /// <summary>Deneme sayısı.</summary>
        public int AttemptCount { get; set; }

        /// <summary>İzin verilen maksimum deneme.</summary>
        public int MaxAttempts { get; set; } = 3;

        /// <summary>Sonraki yeniden deneme zamanı (UTC).</summary>
        public DateTime? NextRetryAtUtc { get; set; }

        /// <summary>İşi kiralayan worker kimliği (makine/instance).</summary>
        public string? LeasedBy { get; set; }

        /// <summary>Kiralama bitiş anı (UTC) — süresi dolarsa iş yeniden alınabilir.</summary>
        public DateTime? LeasedUntilUtc { get; set; }

        /// <summary>İşleme başlangıç anı (UTC).</summary>
        public DateTime? StartedAtUtc { get; set; }

        /// <summary>Tamamlanma anı (UTC).</summary>
        public DateTime? CompletedAtUtc { get; set; }

        /// <summary>Sonuç özeti (ör. "12 attribute, 84 option üretildi").</summary>
        public string? ResultSummary { get; set; }

        /// <summary>Hata mesajı (başarısızlıkta).</summary>
        public string? ErrorMessage { get; set; }

        /// <summary>Bu işte oluşturulan kayıt sayısı.</summary>
        public int CreatedRecordCount { get; set; }
    }

    /// <summary>
    /// Yapay zekâ üretim/onay geçmişi — SALT-EKLEME (append-only) denetim günlüğü.
    /// "AI Audit / AI History / AI Versioning" gereksinimlerini karşılar. Her AI
    /// üretimi, güncellemesi, onayı/reddi ve yeniden üretimi burada iz bırakır.
    /// Hedef kayıt (EntityType, EntityId) ile gevşek bağlıdır (polimorfik).
    /// </summary>
    public class AiGenerationHistory
    {
        /// <summary>Birincil anahtar.</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Hedef entity türü (ör. "AttributeDefinition", "AttributeOption").</summary>
        public string EntityType { get; set; } = string.Empty;

        /// <summary>Hedef entity kimliği.</summary>
        public Guid EntityId { get; set; }

        /// <summary>Bu izi üreten AI işi (varsa).</summary>
        public Guid? AiGenerationJobId { get; set; }

        /// <summary>Eylem (ör. "Created", "Updated", "Approved", "Rejected", "Regenerated").</summary>
        public string Action { get; set; } = string.Empty;

        /// <summary>AI modeli.</summary>
        public string? AiModel { get; set; }

        /// <summary>AI model sürümü.</summary>
        public string? AiModelVersion { get; set; }

        /// <summary>Güven skoru (0..1).</summary>
        public decimal? Confidence { get; set; }

        /// <summary>Prompt hash.</summary>
        public string? PromptHash { get; set; }

        /// <summary>Prompt sürümü.</summary>
        public string? PromptVersion { get; set; }

        /// <summary>Önceki değerin JSON kopyası (versiyonlama/geri alma için).</summary>
        public string? PreviousValueJson { get; set; }

        /// <summary>Yeni değerin JSON kopyası.</summary>
        public string? NewValueJson { get; set; }

        /// <summary>Manuel eylemi (onay/ret) yapan kullanıcı.</summary>
        public Guid? PerformedByUserId { get; set; }

        /// <summary>Kaynak (AI/Manual/Import/System).</summary>
        public ContentSource Source { get; set; } = ContentSource.Ai;

        /// <summary>Gerekçe/not.</summary>
        public string? Reason { get; set; }

        /// <summary>Kayıt anı (UTC).</summary>
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}

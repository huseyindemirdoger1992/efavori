## Dosya: _Attribute.Ai.cs
```csharp
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
```

## Dosya: _Attribute.CategoryLinks.cs
```csharp
using System;

namespace data._Attribute
{
    /// <summary>
    /// Bir attribute'ın belirli bir KATEGORİYE atanmış çözümlenmiş kaydı ve o
    /// kategoriye özgü override'ları. Attribute'lar kategoriye doğrudan, template
    /// üzerinden veya üst kategoriden miras (inheritance) yoluyla gelebilir.
    ///
    /// Miras + override kuralı: Üst kategoride tanımlı Brand/Warranty/Country gibi
    /// attribute'lar alt kategorilere otomatik aktarılır; alt kategori isterse
    /// override eder (<see cref="IsOverride"/>) veya bastırır (<see cref="IsSuppressed"/>).
    /// Çözümleme önceliği: doğrudan/override > template > miras.
    ///
    /// Not: CategoryId, ürün kategorisi CategoriesProduct (Guid PK) tablosuna işaret eder.
    /// (CategoryId, AttributeDefinitionId) benzersizdir.
    /// </summary>
    public class CategoryAttribute : AttributeEntityBase
    {
        /// <summary>Bağlı kategori (CategoriesProduct.Id — Guid).</summary>
        public Guid CategoryId { get; set; }

        /// <summary>Bağlı attribute.</summary>
        public Guid AttributeDefinitionId { get; set; }

        /// <summary>Bu atama bir template'ten geldiyse kaynak template.</summary>
        public Guid? SourceTemplateId { get; set; }

        // ── Miras (inheritance) durumu ──────────────────────────────────────
        /// <summary>Üst kategoriden miras yoluyla mı geldi?</summary>
        public bool IsInherited { get; set; }

        /// <summary>Miras kaynağı üst kategori (CategoriesProduct.Id — Guid).</summary>
        public Guid? InheritedFromCategoryId { get; set; }

        /// <summary>Miras/template ayarını bu kategoride override ediyor mu?</summary>
        public bool IsOverride { get; set; }

        /// <summary>Miras gelen attribute bu kategoride gizlensin (bastırılsın) mı?</summary>
        public bool IsSuppressed { get; set; }

        // ── Kategori bazlı davranış override'ları (null = attribute varsayılanı) ─
        /// <summary>Bu kategoride zorunlu mu? (null = varsayılan).</summary>
        public bool? IsRequired { get; set; }

        /// <summary>
        /// Bu kategoride varyant mı? (null = attribute varsayılanı).
        /// Ör: giyimde Renk = true (varyant), mobilyada Renk = false (yalnız bilgi).
        /// </summary>
        public bool? IsVariant { get; set; }

        /// <summary>Bu kategoride filtrelenebilir mi? (null = varsayılan).</summary>
        public bool? IsFilterable { get; set; }

        /// <summary>Bu kategoride aranabilir mi? (null = varsayılan).</summary>
        public bool? IsSearchable { get; set; }

        /// <summary>Bu kategoride karşılaştırılabilir mi? (null = varsayılan).</summary>
        public bool? IsComparable { get; set; }

        /// <summary>Bu kategoride ürün sayfasında görünür mü? (null = varsayılan).</summary>
        public bool? IsVisibleOnProductPage { get; set; }

        /// <summary>Bu kategorideki sıralama.</summary>
        public int DisplayOrder { get; set; }

        /// <summary>Bu kategorideki görsel grup override'ı (opsiyonel).</summary>
        public Guid? AttributeGroupOverrideId { get; set; }

        /// <summary>Atama aktif mi?</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>Yapay zekâ izlenebilirlik + manuel koruma bilgisi.</summary>
        public AiMetadata Ai { get; set; } = new();
    }
}
```

## Dosya: _Attribute.Common.cs
```csharp
using System;
using System.Globalization;
using System.Text;

// ════════════════════════════════════════════════════════════════════════════
//  OWNED TİP UYUMU — mevcut projedeki ortak soft-delete tipi (data.Owned.IsDeleted:
//  { bool? IsDeletedStatu; DateTime? DeletedAtDate }) kullanılır.
// ════════════════════════════════════════════════════════════════════════════
using data.Owned;

namespace data._Attribute
{
    /// <summary>
    /// Attribute kataloğundaki tüm entity'lerin ortak tabanı.
    /// GUID birincil anahtar (mevcut konvansiyonla uyumlu), denetim (audit)
    /// alanları, soft-delete ve iyimser eşzamanlılık (RowVersion) sağlar.
    /// Navigation property İÇERMEZ — ilişkiler yalnızca ID üzerinden kurulur.
    /// </summary>
    public abstract class AttributeEntityBase
    {
        /// <summary>Birincil anahtar. Non-sequential GUID (mevcut proje konvansiyonu).</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Kaydın oluşturulma anı (UTC).</summary>
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        /// <summary>Kaydı oluşturan kullanıcı (varsa). Sistem/AI kayıtlarında null olabilir.</summary>
        public Guid? CreatedByUserId { get; set; }

        /// <summary>Son güncelleme anı (UTC).</summary>
        public DateTime? UpdatedAtUtc { get; set; }

        /// <summary>Son güncelleyen kullanıcı (varsa).</summary>
        public Guid? UpdatedByUserId { get; set; }

        /// <summary>Soft-delete durumu (mevcut ortak owned tip).</summary>
        public IsDeleted IsDeleted { get; set; } = new();

        /// <summary>
        /// İyimser eşzamanlılık damgası (rowversion). Çok örnekli
        /// BackgroundService'lerde aynı kaydın yarış koşuluyla ezilmesini önler.
        /// </summary>
        public byte[]? RowVersion { get; set; }
    }

    /// <summary>
    /// Yapay zekâ üretilebilir kayıtlar için izlenebilirlik + manuel koruma bilgisi.
    /// Owned tip olarak (aynı tabloda ek kolonlar) gömülür.
    /// Manuel düzenleme kuralı: <see cref="IsManuallyEdited"/> = true ise
    /// AI ve Import bu kaydın ÜZERİNE ASLA yazmaz.
    /// </summary>
    public class AiMetadata
    {
        /// <summary>Kaydın kaynağı (System/Manual/Ai/Import).</summary>
        public ContentSource Source { get; set; } = ContentSource.Manual;

        /// <summary>Admin tarafından manuel düzenlendi mi? (AI/Import kilidi).</summary>
        public bool IsManuallyEdited { get; set; }

        /// <summary>Manuel düzenlemeyi yapan kullanıcı.</summary>
        public Guid? ManuallyEditedByUserId { get; set; }

        /// <summary>Manuel düzenleme anı (UTC).</summary>
        public DateTime? ManuallyEditedAtUtc { get; set; }

        /// <summary>Kaydı üreten AI modeli (ör. "gemini-2.5-pro").</summary>
        public string? AiModel { get; set; }

        /// <summary>AI model sürümü / etiketi.</summary>
        public string? AiModelVersion { get; set; }

        /// <summary>AI güven skoru (0..1).</summary>
        public decimal? Confidence { get; set; }

        /// <summary>Kullanılan prompt'un hash'i (tekrarlanabilirlik/izlenebilirlik).</summary>
        public string? PromptHash { get; set; }

        /// <summary>Kullanılan prompt sürümü.</summary>
        public string? PromptVersion { get; set; }

        /// <summary>AI üretim anı (UTC).</summary>
        public DateTime? AiGeneratedAtUtc { get; set; }

        /// <summary>Onay durumu.</summary>
        public AiApprovalStatus ApprovalStatus { get; set; } = AiApprovalStatus.NotRequired;

        /// <summary>İnceleme durumu.</summary>
        public AiReviewStatus ReviewStatus { get; set; } = AiReviewStatus.NotReviewed;

        /// <summary>İnceleme/ret gerekçesi veya notu.</summary>
        public string? ReviewNote { get; set; }

        /// <summary>İncelemeyi yapan kullanıcı.</summary>
        public Guid? ReviewedByUserId { get; set; }

        /// <summary>İnceleme anı (UTC).</summary>
        public DateTime? ReviewedAtUtc { get; set; }

        /// <summary>
        /// Bu kaydın üretilmesini tetikleyen kaynak bağlam
        /// (ör. analiz edilen kategori yolu: "Elektronik/Bilgisayar/Laptop").
        /// </summary>
        public string? CategorySource { get; set; }
    }

    /// <summary>
    /// Dedup (tekilleştirme) ve import eşleştirmesinin temelini oluşturan
    /// dilden bağımsız metin normalizasyonu. AI/Import bir Attribute veya Option
    /// üretmeden önce üreteceği metni normalize eder ve mevcut kanonik/synonym
    /// tokenlarıyla karşılaştırır; eşleşme varsa YENİ kayıt oluşturmaz.
    /// Örn: "RAM", "Memory", "Bellek ", "System  Memory" → "ram", "memory", "bellek", "system memory".
    /// </summary>
    public static class TextNormalizer
    {
        /// <summary>
        /// Metni normalize eder: küçük harf + diyakritik kaldırma + noktalama temizliği
        /// + tek boşluk. Kanonik token üretimi ve karşılaştırma için kullanılır.
        /// </summary>
        public static string Normalize(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            // 1) Türkçe'ye özgü karakterleri güvenli biçimde sadeleştir.
            var pre = value
                .Replace('İ', 'I').Replace('ı', 'i')
                .Replace('Ş', 'S').Replace('ş', 's')
                .Replace('Ğ', 'G').Replace('ğ', 'g')
                .Replace('Ç', 'C').Replace('ç', 'c')
                .Replace('Ö', 'O').Replace('ö', 'o')
                .Replace('Ü', 'U').Replace('ü', 'u');

            // 2) Küçük harf (kültürden bağımsız).
            pre = pre.ToLowerInvariant();

            // 3) Kalan diyakritikleri Unicode ayrıştırmasıyla kaldır.
            var decomposed = pre.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(decomposed.Length);
            var lastWasSpace = false;

            foreach (var ch in decomposed)
            {
                var cat = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (cat == UnicodeCategory.NonSpacingMark)
                    continue; // diyakritik işareti at

                if (char.IsLetterOrDigit(ch))
                {
                    sb.Append(ch);
                    lastWasSpace = false;
                }
                else if (char.IsWhiteSpace(ch) || ch == '-' || ch == '_' || ch == '/')
                {
                    // ayırıcıları tek boşluğa indir
                    if (!lastWasSpace && sb.Length > 0)
                    {
                        sb.Append(' ');
                        lastWasSpace = true;
                    }
                }
                // diğer noktalama karakterleri atılır
            }

            return sb.ToString().Trim().Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Kanonik kod üretir: normalize edilmiş metindeki boşlukları alt çizgiye çevirir.
        /// Örn: "Screen Resolution" → "screen_resolution".
        /// </summary>
        public static string ToCanonicalCode(string? value)
            => Normalize(value).Replace(' ', '_');
    }
}
```

## Dosya: _Attribute.Definitions.cs
```csharp
using System;

namespace data._Attribute
{
    /// <summary>
    /// Merkezî attribute TANIMI — sistemin "sözlüğü". Bir attribute (ör. RAM, Renk,
    /// Ekran Boyutu) tüm sistemde YALNIZCA BİR KEZ tanımlanır; sonra Category,
    /// Template, Import, Export, Filter, Search ve AI tarafından tekrar kullanılır.
    ///
    /// Not: Sınıf adı "Attribute" YERİNE "AttributeDefinition"dır; çünkü "Attribute"
    /// .NET'te <see cref="System.Attribute"/> ile çakışır ve karışıklık yaratır.
    ///
    /// Merkezî felsefe: Satıcı yeni attribute OLUŞTURAMAZ — bu tablo yalnızca sistem
    /// (Admin/AI) tarafından yönetilir; bu yüzden burada satıcı/mağaza sahipliği yoktur.
    /// </summary>
    public class AttributeDefinition : AttributeEntityBase
    {
        /// <summary>
        /// Dilden bağımsız, GLOBAL TEKİL kanonik kod (ör. "ram", "color",
        /// "screen_size"). Dedup ve tüm eşleştirmelerin çapasıdır.
        /// Aynı kavramın farklı bağlamları için ayrıştırıcı ekleyin
        /// (ör. giyim bedeni → "size_clothing", ekran boyutu → "size_screen").
        /// </summary>
        public string CanonicalCode { get; set; } = string.Empty;

        /// <summary>
        /// Kanonik adın normalize edilmiş biçimi (küçük harf, diyakritiksiz).
        /// AI/Import dedup'ında hızlı arama için indekslidir.
        /// </summary>
        public string NormalizedName { get; set; } = string.Empty;

        /// <summary>Varsayılan (Türkçe) görünen ad — hızlı okuma için satır içi.</summary>
        public string CanonicalName { get; set; } = string.Empty;

        /// <summary>Bağlı görsel grup (opsiyonel).</summary>
        public Guid? AttributeGroupId { get; set; }

        /// <summary>Veri tipi (saklama/doğrulama davranışı).</summary>
        public AttributeDataType DataType { get; set; } = AttributeDataType.Text;

        /// <summary>Arayüz giriş bileşeni (widget).</summary>
        public AttributeInputType InputType { get; set; } = AttributeInputType.TextBox;

        // ── Ölçü (Measurement) attribute'ları için ──────────────────────────
        /// <summary>Ölçü attribute'ında izin verilen birim grubu (ör. Uzunluk).</summary>
        public Guid? UnitGroupId { get; set; }

        /// <summary>Normalize değeri saklamak için baz/temel birim (ör. mm).</summary>
        public Guid? BaseUnitId { get; set; }

        // ── Davranış varsayılanları (Category/Template ile override edilebilir) ─
        /// <summary>Birden fazla değer/option seçilebilir mi?</summary>
        public bool IsMultiValued { get; set; }

        /// <summary>Varsayılan olarak zorunlu mu?</summary>
        public bool IsRequiredByDefault { get; set; }

        /// <summary>
        /// Varsayılan olarak varyant üretebilir mi? (ör. giyimde Renk = true).
        /// Kategori bazında override edilebilir (mobilyada Renk yalnızca bilgi).
        /// </summary>
        public bool IsVariantByDefault { get; set; }

        /// <summary>Varsayılan olarak filtrelenebilir (facet) mi?</summary>
        public bool IsFilterableByDefault { get; set; }

        /// <summary>Varsayılan olarak arama motoru alanı mı?</summary>
        public bool IsSearchableByDefault { get; set; }

        /// <summary>Varsayılan olarak karşılaştırma (compare) alanı mı?</summary>
        public bool IsComparableByDefault { get; set; }

        /// <summary>Varsayılan olarak ürün sayfasında görünür mü?</summary>
        public bool IsVisibleOnProductPageByDefault { get; set; } = true;

        /// <summary>Öne çıkan/başlık özelliği mi? (ör. Laptop için CPU/RAM).</summary>
        public bool IsKeyAttribute { get; set; }

        // ── Doğrulama (Validation) kuralları ────────────────────────────────
        /// <summary>Regex doğrulama deseni (opsiyonel).</summary>
        public string? RegexPattern { get; set; }

        /// <summary>Sayısal minimum (Integer/Decimal/Measurement).</summary>
        public decimal? MinNumericValue { get; set; }

        /// <summary>Sayısal maksimum.</summary>
        public decimal? MaxNumericValue { get; set; }

        /// <summary>Sayısal artış adımı (ör. 0.5).</summary>
        public decimal? NumericStep { get; set; }

        /// <summary>Ondalık basamak sayısı.</summary>
        public int? DecimalScale { get; set; }

        /// <summary>Metin minimum uzunluğu.</summary>
        public int? MinLength { get; set; }

        /// <summary>Metin maksimum uzunluğu.</summary>
        public int? MaxLength { get; set; }

        /// <summary>Değer ürün/varyant kapsamında benzersiz mi olmalı?</summary>
        public bool IsUnique { get; set; }

        /// <summary>Varsayılan değer (opsiyonel).</summary>
        public string? DefaultValue { get; set; }

        /// <summary>Genel sıralama önceliği.</summary>
        public int DisplayOrder { get; set; }

        /// <summary>Attribute aktif mi?</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>Sistemsel/kritik attribute — silinemez/salt-okunur korunur.</summary>
        public bool IsSystemLocked { get; set; }

        /// <summary>Sürüm numarası (immutable evrim izleme için).</summary>
        public int Version { get; set; } = 1;

        /// <summary>Yapay zekâ izlenebilirlik + manuel koruma bilgisi.</summary>
        public AiMetadata Ai { get; set; } = new();
    }

    /// <summary>
    /// <see cref="AttributeDefinition"/> için dile özgü metinler.
    /// (AttributeDefinitionId, Language) benzersizdir.
    /// </summary>
    public class AttributeTranslation : AttributeEntityBase
    {
        /// <summary>Bağlı attribute.</summary>
        public Guid AttributeDefinitionId { get; set; }

        /// <summary>Dil.</summary>
        public Language Language { get; set; }

        /// <summary>Bu dildeki attribute adı (ör. RAM → "Bellek").</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Bu dildeki açıklama.</summary>
        public string? Description { get; set; }

        /// <summary>Bu dildeki yardım metni (form ipucu).</summary>
        public string? HelpText { get; set; }

        /// <summary>Bu dildeki placeholder metni.</summary>
        public string? Placeholder { get; set; }

        /// <summary>Kaydın kaynağı.</summary>
        public ContentSource Source { get; set; } = ContentSource.Manual;

        /// <summary>Admin bu çeviriyi elle düzenledi mi? (AI/Import ezme kilidi).</summary>
        public bool IsManuallyEdited { get; set; }
    }

    /// <summary>
    /// İNSANA DÖNÜK alternatif görünen ad (alias). Ör: RAM için TR'de "Bellek".
    /// Görüntüleme/eşleştirme kolaylığı sağlar. (Synonym'den farkı: alias görünen
    /// etikettir; synonym ise makine eşleştirmesi/dedup tokenıdır.)
    /// </summary>
    public class AttributeAlias : AttributeEntityBase
    {
        /// <summary>Bağlı attribute.</summary>
        public Guid AttributeDefinitionId { get; set; }

        /// <summary>Alias'ın dili (null = dilden bağımsız).</summary>
        public Language? Language { get; set; }

        /// <summary>Alias metni (görünen biçim).</summary>
        public string Alias { get; set; } = string.Empty;

        /// <summary>Alias'ın normalize edilmiş biçimi (indeksli).</summary>
        public string NormalizedAlias { get; set; } = string.Empty;

        /// <summary>Kaydın kaynağı.</summary>
        public ContentSource Source { get; set; } = ContentSource.Manual;
    }

    /// <summary>
    /// MAKİNE eşleştirme/dedup tokenı (synonym). Ör: RAM için "system memory",
    /// "main memory", "bellek", "memory". AI/Import bir attribute üretmeden önce
    /// ürettiği metnin normalize tokenını burada arar; eşleşirse YENİ attribute
    /// oluşturmaz, mevcut olanı kullanır.
    ///
    /// <see cref="NormalizedToken"/> GLOBAL TEKİLDİR: bir token yalnızca tek bir
    /// attribute'a işaret edebilir (aksi hâlde eşleştirme belirsizleşir).
    /// </summary>
    public class AttributeSynonym : AttributeEntityBase
    {
        /// <summary>Token'ın işaret ettiği attribute.</summary>
        public Guid AttributeDefinitionId { get; set; }

        /// <summary>Ham token metni.</summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>Normalize edilmiş token (global tekil, indeksli).</summary>
        public string NormalizedToken { get; set; } = string.Empty;

        /// <summary>Token'ın dili (null = dilden bağımsız).</summary>
        public Language? Language { get; set; }

        /// <summary>Kaynak (özellikle AI üretimi için).</summary>
        public ContentSource Source { get; set; } = ContentSource.Manual;

        /// <summary>AI eşleştirmesinde güven skoru (0..1).</summary>
        public decimal? Confidence { get; set; }
    }
}
```

## Dosya: _Attribute.Enums.cs
```csharp
using System;

namespace data._Attribute
{
    // ════════════════════════════════════════════════════════════════════════
    //  efavori — Enterprise Marketplace Attribute System V3
    //  Enum kataloğu — dilden bağımsız, tinyint olarak saklanacak sabit değerler
    //  Not: Enum değerlerine ASLA arada değer eklemeyin; yalnızca sona ekleyin.
    //       Aksi halde veritabanındaki mevcut kayıtların anlamı değişir.
    // ════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Sistemin desteklediği 10 dil. Görünen tüm metinler için çeviri satırlarında
    /// bu değer saklanır. Kanonik (varsayılan) kaynak dil Türkçe'dir (Tr).
    /// </summary>
    public enum Language : byte
    {
        /// <summary>Türkçe — kanonik / kaynak dil.</summary>
        Tr = 1,
        /// <summary>İngilizce.</summary>
        En = 2,
        /// <summary>Azerbaycanca.</summary>
        Az = 3,
        /// <summary>Almanca.</summary>
        De = 4,
        /// <summary>İspanyolca.</summary>
        Es = 5,
        /// <summary>Fransızca.</summary>
        Fr = 6,
        /// <summary>Hintçe.</summary>
        Hi = 7,
        /// <summary>Portekizce.</summary>
        Pt = 8,
        /// <summary>Rusça.</summary>
        Ru = 9,
        /// <summary>Çince.</summary>
        Zh = 10
    }

    /// <summary>
    /// Attribute'ın veri tipi. Değerin nasıl saklanacağını, doğrulanacağını ve
    /// arama/filtreleme davranışını belirler. UI widget'ından bağımsızdır.
    /// </summary>
    public enum AttributeDataType : byte
    {
        /// <summary>Kısa serbest metin (ör. Model kodu).</summary>
        Text = 1,
        /// <summary>Çok satırlı serbest metin.</summary>
        MultilineText = 2,
        /// <summary>Zengin metin (HTML).</summary>
        RichText = 3,
        /// <summary>Tam sayı.</summary>
        Integer = 4,
        /// <summary>Ondalık sayı.</summary>
        Decimal = 5,
        /// <summary>Evet/Hayır.</summary>
        Boolean = 6,
        /// <summary>Tarih.</summary>
        Date = 7,
        /// <summary>Tarih + saat.</summary>
        DateTime = 8,
        /// <summary>Tek seçimli option (ör. İşletim Sistemi).</summary>
        SingleSelect = 9,
        /// <summary>Çok seçimli option (ör. Bağlantı: Wifi + Bluetooth + NFC).</summary>
        MultiSelect = 10,
        /// <summary>Ölçü değeri (sayı + birim, ör. 15.6 inç).</summary>
        Measurement = 11,
        /// <summary>Renk (hex + option).</summary>
        Color = 12,
        /// <summary>URL.</summary>
        Url = 13,
        /// <summary>E-posta.</summary>
        Email = 14,
        /// <summary>Yapılandırılmış JSON.</summary>
        Json = 15
    }

    /// <summary>
    /// Attribute'ın admin/satıcı arayüzünde hangi giriş bileşeni ile gösterileceği.
    /// Semantik <see cref="AttributeDataType"/>'dan ayrıdır (aynı tip farklı widget'larla sunulabilir).
    /// </summary>
    public enum AttributeInputType : byte
    {
        /// <summary>Tek satır metin kutusu.</summary>
        TextBox = 1,
        /// <summary>Çok satırlı metin alanı.</summary>
        TextArea = 2,
        /// <summary>Zengin metin editörü.</summary>
        RichTextEditor = 3,
        /// <summary>Sayı kutusu.</summary>
        NumberBox = 4,
        /// <summary>Açılır liste (tek seçim).</summary>
        Dropdown = 5,
        /// <summary>Radyo düğmeleri (tek seçim).</summary>
        RadioGroup = 6,
        /// <summary>Onay kutusu listesi (çok seçim).</summary>
        CheckboxList = 7,
        /// <summary>Çok seçimli açılır liste.</summary>
        MultiSelectDropdown = 8,
        /// <summary>Aç/Kapa anahtarı (boolean).</summary>
        Toggle = 9,
        /// <summary>Tarih seçici.</summary>
        DatePicker = 10,
        /// <summary>Tarih + saat seçici.</summary>
        DateTimePicker = 11,
        /// <summary>Renk seçici.</summary>
        ColorPicker = 12,
        /// <summary>Ölçü kutusu (değer + birim seçimi).</summary>
        MeasurementBox = 13,
        /// <summary>URL kutusu.</summary>
        UrlBox = 14
    }

    /// <summary>
    /// Bir kaydın hangi kaynaktan üretildiğini belirtir. Manuel düzenleme koruması
    /// ve izlenebilirlik için kullanılır.
    /// </summary>
    public enum ContentSource : byte
    {
        /// <summary>Sistem tarafından (seed/migrasyon) oluşturuldu.</summary>
        System = 1,
        /// <summary>Admin tarafından manuel oluşturuldu.</summary>
        Manual = 2,
        /// <summary>Yapay zekâ tarafından üretildi.</summary>
        Ai = 3,
        /// <summary>Dış platform aktarımından (import) geldi.</summary>
        Import = 4
    }

    /// <summary>Yapay zekâ üretim işinin türü (BackgroundService ayrımı).</summary>
    public enum AiJobType : byte
    {
        /// <summary>SERVICE 1 — kategori yolunu analiz edip Attribute üretir.</summary>
        CategoryAttributeAnalysis = 1,
        /// <summary>SERVICE 2 — üretilmiş Attribute'lar için Option listesi üretir.</summary>
        OptionGeneration = 2,
        /// <summary>Attribute/Option için çok dilli çeviri üretir.</summary>
        TranslationGeneration = 3,
        /// <summary>Attribute/Option için alias & synonym (eşanlamlı) üretir.</summary>
        SynonymGeneration = 4,
        /// <summary>Kategoriye uygun Template eşleştirmesi/önerisi üretir.</summary>
        TemplateSuggestion = 5
    }

    /// <summary>Yapay zekâ üretim işinin yaşam döngüsü durumu.</summary>
    public enum AiJobStatus : byte
    {
        /// <summary>Kuyrukta, işlenmeyi bekliyor.</summary>
        Pending = 1,
        /// <summary>Bir worker tarafından kiralandı (lease) — çift işleme karşı.</summary>
        Leased = 2,
        /// <summary>İşleniyor.</summary>
        Processing = 3,
        /// <summary>Başarıyla tamamlandı.</summary>
        Completed = 4,
        /// <summary>Hata ile sonuçlandı (yeniden denenebilir).</summary>
        Failed = 5,
        /// <summary>Tamamlandı ancak admin onayı bekliyor.</summary>
        NeedsReview = 6,
        /// <summary>İptal edildi.</summary>
        Cancelled = 7,
        /// <summary>Zaten mevcut olduğu için atlandı (idempotent).</summary>
        Skipped = 8
    }

    /// <summary>Yapay zekâ üretilen kaydın onay durumu.</summary>
    public enum AiApprovalStatus : byte
    {
        /// <summary>Onay gerekmiyor (ör. manuel kayıt).</summary>
        NotRequired = 1,
        /// <summary>Onay bekliyor.</summary>
        PendingApproval = 2,
        /// <summary>Admin tarafından onaylandı.</summary>
        Approved = 3,
        /// <summary>Admin tarafından reddedildi.</summary>
        Rejected = 4
    }

    /// <summary>Yapay zekâ üretilen kaydın inceleme durumu.</summary>
    public enum AiReviewStatus : byte
    {
        /// <summary>Henüz incelenmedi.</summary>
        NotReviewed = 1,
        /// <summary>İnceleme sürecinde.</summary>
        InReview = 2,
        /// <summary>İncelendi.</summary>
        Reviewed = 3
    }

    /// <summary>Attribute bağımlılığında (dependency) koşul karşılaştırma operatörü.</summary>
    public enum DependencyOperator : byte
    {
        /// <summary>Eşittir.</summary>
        Equals = 1,
        /// <summary>Eşit değildir.</summary>
        NotEquals = 2,
        /// <summary>Listedekilerden biri.</summary>
        In = 3,
        /// <summary>Listedekilerden hiçbiri.</summary>
        NotIn = 4,
        /// <summary>Büyüktür.</summary>
        GreaterThan = 5,
        /// <summary>Büyük veya eşittir.</summary>
        GreaterOrEqual = 6,
        /// <summary>Küçüktür.</summary>
        LessThan = 7,
        /// <summary>Küçük veya eşittir.</summary>
        LessOrEqual = 8,
        /// <summary>Herhangi bir değere sahip (dolu).</summary>
        IsSet = 9,
        /// <summary>Boş (değeri yok).</summary>
        IsNotSet = 10,
        /// <summary>Metin içerir.</summary>
        Contains = 11
    }

    /// <summary>Bağımlılık koşulu sağlandığında hedef attribute'a uygulanacak eylem.</summary>
    public enum DependencyAction : byte
    {
        /// <summary>Hedef attribute'ı göster.</summary>
        Show = 1,
        /// <summary>Hedef attribute'ı gizle.</summary>
        Hide = 2,
        /// <summary>Hedef attribute'ı zorunlu yap.</summary>
        Require = 3,
        /// <summary>Hedef attribute'ı opsiyonel yap.</summary>
        MakeOptional = 4,
        /// <summary>Hedef attribute'ı etkinleştir.</summary>
        Enable = 5,
        /// <summary>Hedef attribute'ı devre dışı bırak.</summary>
        Disable = 6
    }

    /// <summary>Aynı koşul grubundaki bağımlılık koşullarının mantıksal birleşimi.</summary>
    public enum DependencyLogic : byte
    {
        /// <summary>Gruptaki tüm koşullar sağlanmalı.</summary>
        And = 1,
        /// <summary>Gruptaki koşullardan biri sağlanmalı.</summary>
        Or = 2
    }

    /// <summary>Dış platform eşleştirmesinin yönü (import / export).</summary>
    public enum MappingDirection : byte
    {
        /// <summary>Yalnızca içeri aktarım (dış → efavori).</summary>
        Import = 1,
        /// <summary>Yalnızca dışa aktarım (efavori → dış).</summary>
        Export = 2,
        /// <summary>Her iki yön.</summary>
        Bidirectional = 3
    }

    /// <summary>Bir attribute'ın dış platform açısından zorunluluk seviyesi.</summary>
    public enum PlatformRequirementLevel : byte
    {
        /// <summary>Opsiyonel.</summary>
        Optional = 1,
        /// <summary>Önerilen.</summary>
        Recommended = 2,
        /// <summary>Zorunlu (aktarım için gerekli).</summary>
        Required = 3
    }

    /// <summary>Birim grubunun fiziksel boyutu / ölçüm ailesi.</summary>
    public enum UnitDimension : byte
    {
        /// <summary>Ağırlık (kg, g...).</summary>
        Weight = 1,
        /// <summary>Uzunluk (m, cm, mm...).</summary>
        Length = 2,
        /// <summary>Alan (m², cm²...).</summary>
        Area = 3,
        /// <summary>Hacim (l, ml...).</summary>
        Volume = 4,
        /// <summary>Veri depolama (GB, MB, TB...).</summary>
        DataStorage = 5,
        /// <summary>Güç (W, kW...).</summary>
        Power = 6,
        /// <summary>Frekans (Hz, GHz...).</summary>
        Frequency = 7,
        /// <summary>Sıcaklık (°C, K, °F...).</summary>
        Temperature = 8,
        /// <summary>Basınç (bar, Pa...).</summary>
        Pressure = 9,
        /// <summary>Bellek boyutu (RAM: GB...).</summary>
        MemorySize = 10,
        /// <summary>Ekran boyutu (inç, cm).</summary>
        ScreenSize = 11,
        /// <summary>Zaman/süre (sa, dk, sn).</summary>
        Time = 12,
        /// <summary>Hız (km/h...).</summary>
        Speed = 13,
        /// <summary>Gerilim (V).</summary>
        Voltage = 14,
        /// <summary>Enerji/kapasite (mAh, Wh...).</summary>
        Energy = 15,
        /// <summary>Adet/sayı.</summary>
        Count = 16,
        /// <summary>Özel/tanımsız.</summary>
        Custom = 100
    }

    /// <summary>
    /// Normalizasyon/transform kuralının türü. Import ve dedup sırasında metni
    /// kanonik hâle getirmek için kullanılır.
    /// </summary>
    public enum NormalizationRuleType : byte
    {
        /// <summary>Küçük harfe çevir.</summary>
        Lowercase = 1,
        /// <summary>Baş/son boşlukları kırp.</summary>
        TrimWhitespace = 2,
        /// <summary>İç boşlukları tek boşluğa indir.</summary>
        CollapseWhitespace = 3,
        /// <summary>Diyakritikleri kaldır (ör. ş→s, ç→c).</summary>
        RemoveDiacritics = 4,
        /// <summary>Noktalama işaretlerini kaldır.</summary>
        RemovePunctuation = 5,
        /// <summary>Regex ile bul/değiştir.</summary>
        RegexReplace = 6,
        /// <summary>Birim ekini ayır (ör. "16GB" → "16").</summary>
        StripUnit = 7,
        /// <summary>Değeri sabit bir eşlemeye göre çevir (ör. "grey" → "gray").</summary>
        MapValue = 8,
        /// <summary>Özel/kod tarafından işlenen kural.</summary>
        Custom = 100
    }

    /// <summary><see cref="Language"/> enum'ı için kültür kodu yardımcıları.</summary>
    public static class LanguageExtensions
    {
        /// <summary>İki harfli ISO kültür kodunu döndürür (ör. Language.Tr → "tr").</summary>
        public static string ToCultureCode(this Language language) => language switch
        {
            Language.Tr => "tr",
            Language.En => "en",
            Language.Az => "az",
            Language.De => "de",
            Language.Es => "es",
            Language.Fr => "fr",
            Language.Hi => "hi",
            Language.Pt => "pt",
            Language.Ru => "ru",
            Language.Zh => "zh",
            _ => "tr"
        };

        /// <summary>Kültür kodundan <see cref="Language"/> çözer; bilinmiyorsa Tr döner.</summary>
        public static Language FromCultureCode(string? code) => (code?.Trim().ToLowerInvariant()) switch
        {
            "tr" => Language.Tr,
            "en" => Language.En,
            "az" => Language.Az,
            "de" => Language.De,
            "es" => Language.Es,
            "fr" => Language.Fr,
            "hi" => Language.Hi,
            "pt" => Language.Pt,
            "ru" => Language.Ru,
            "zh" => Language.Zh,
            _ => Language.Tr
        };
    }
}
```

## Dosya: _Attribute.Groups.cs
```csharp
using System;

namespace data._Attribute
{
    /// <summary>
    /// Attribute'ları arayüzde mantıksal başlıklar altında toplayan grup
    /// (ör. "Teknik Özellikler", "Boyutlar", "Bağlantı", "Genel").
    /// Template'ten ayrıdır: Group görsel/mantıksal kümeleme; Template ise
    /// kategoriye atanan yeniden kullanılabilir attribute kümesidir.
    /// </summary>
    public class AttributeGroup : AttributeEntityBase
    {
        /// <summary>
        /// Dilden bağımsız, tekil ve kararlı kod (ör. "technical_specs").
        /// Kod tabanında/mapping'de referans olarak kullanılır.
        /// </summary>
        public string CanonicalCode { get; set; } = string.Empty;

        /// <summary>Varsayılan (Türkçe) görünen ad — hızlı okuma için satır içi tutulur.</summary>
        public string CanonicalName { get; set; } = string.Empty;

        /// <summary>Sıralama önceliği (küçük = önce).</summary>
        public int DisplayOrder { get; set; }

        /// <summary>Font Awesome / Bootstrap Icons ikon sınıfı (ör. "fa fa-cogs").</summary>
        public string? IconCssClass { get; set; }

        /// <summary>Grup aktif mi?</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>Yapay zekâ izlenebilirlik + manuel koruma bilgisi.</summary>
        public AiMetadata Ai { get; set; } = new();
    }

    /// <summary>
    /// <see cref="AttributeGroup"/> için dile özgü metinler.
    /// (AttributeGroupId, Language) benzersizdir.
    /// </summary>
    public class AttributeGroupTranslation : AttributeEntityBase
    {
        /// <summary>Bağlı grup.</summary>
        public Guid AttributeGroupId { get; set; }

        /// <summary>Dil.</summary>
        public Language Language { get; set; }

        /// <summary>Bu dildeki grup adı.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Bu dildeki açıklama (opsiyonel).</summary>
        public string? Description { get; set; }

        /// <summary>Kaydın kaynağı (manuel/AI/import).</summary>
        public ContentSource Source { get; set; } = ContentSource.Manual;

        /// <summary>Admin bu çeviriyi elle düzenledi mi? (AI/Import ezme kilidi).</summary>
        public bool IsManuallyEdited { get; set; }
    }
}
```

## Dosya: _Attribute.Integration.cs
```csharp
using System;

namespace data._Attribute
{
    /// <summary>
    /// Dış entegrasyon platformu (referans tablo). Enum yerine tablo tercih edildi:
    /// yeni platform (yeni pazaryeri/format) yayın gerektirmeden eklenebilir.
    /// Ör: amazon, trendyol, hepsiburada, n11, ebay, aliexpress, temu,
    /// google_merchant, facebook_catalog, woocommerce, csv, json, xml, api.
    /// </summary>
    public class IntegrationPlatform : AttributeEntityBase
    {
        /// <summary>Tekil kod (ör. "trendyol").</summary>
        public string CanonicalCode { get; set; } = string.Empty;

        /// <summary>İnsan-okur ad (ör. "Trendyol").</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Platform aktif mi?</summary>
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// İç <see cref="AttributeDefinition"/> ile dış platform attribute'ının
    /// eşleştirmesi. Import/Export ve platform-zorunluluğu bilgisini taşır.
    /// (IntegrationPlatformId, AttributeDefinitionId, Direction) benzersizdir.
    /// </summary>
    public class AttributeMapping : AttributeEntityBase
    {
        /// <summary>Bağlı platform.</summary>
        public Guid IntegrationPlatformId { get; set; }

        /// <summary>Bağlı iç attribute.</summary>
        public Guid AttributeDefinitionId { get; set; }

        /// <summary>Dış platformdaki attribute kodu.</summary>
        public string ExternalCode { get; set; } = string.Empty;

        /// <summary>Dış platformdaki attribute kimliği (varsa).</summary>
        public string? ExternalId { get; set; }

        /// <summary>Dış platformdaki attribute adı (referans).</summary>
        public string? ExternalName { get; set; }

        /// <summary>Eşleştirme yönü (import/export/iki yönlü).</summary>
        public MappingDirection Direction { get; set; } = MappingDirection.Bidirectional;

        /// <summary>Bu attribute'ın platform açısından zorunluluk seviyesi.</summary>
        public PlatformRequirementLevel RequirementLevel { get; set; } = PlatformRequirementLevel.Optional;

        /// <summary>Dönüşüm kuralı yapılandırması (JSON).</summary>
        public string? TransformRuleJson { get; set; }

        /// <summary>Uygulanacak normalizasyon kuralı (opsiyonel).</summary>
        public Guid? NormalizationRuleId { get; set; }

        /// <summary>Eşleştirme aktif mi?</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>Yapay zekâ izlenebilirlik + manuel koruma bilgisi.</summary>
        public AiMetadata Ai { get; set; } = new();
    }

    /// <summary>
    /// İç <see cref="AttributeOption"/> ile dış platform option/değerinin eşleştirmesi.
    /// Ör: iç "black" ↔ Amazon "BLACK". <see cref="AttributeDefinitionId"/> denormalize
    /// tutulur; import'ta (platform + attribute + dış kod) ile hızlı option bulunur.
    /// (IntegrationPlatformId, AttributeOptionId, Direction) benzersizdir.
    /// </summary>
    public class AttributeOptionMapping : AttributeEntityBase
    {
        /// <summary>Bağlı platform.</summary>
        public Guid IntegrationPlatformId { get; set; }

        /// <summary>Bağlı iç option.</summary>
        public Guid AttributeOptionId { get; set; }

        /// <summary>Denormalize attribute referansı (import lookup indeksleri için).</summary>
        public Guid AttributeDefinitionId { get; set; }

        /// <summary>Dış platformdaki option kodu.</summary>
        public string ExternalCode { get; set; } = string.Empty;

        /// <summary>Dış platformdaki option değeri/etiketi (referans).</summary>
        public string? ExternalValue { get; set; }

        /// <summary>Dış platformdaki option kimliği (varsa).</summary>
        public string? ExternalId { get; set; }

        /// <summary>Eşleştirme yönü.</summary>
        public MappingDirection Direction { get; set; } = MappingDirection.Bidirectional;

        /// <summary>Eşleştirme aktif mi?</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>Yapay zekâ izlenebilirlik + manuel koruma bilgisi.</summary>
        public AiMetadata Ai { get; set; } = new();
    }
}
```

## Dosya: _Attribute.ModelConfiguration.cs
```csharp
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using data.Owned; // ortak soft-delete owned tipi (data.Owned.IsDeleted)

namespace data._Attribute
{
    /// <summary>
    /// Attribute kataloğu (V3) için EF Core Fluent yapılandırması. Mevcut
    /// _ProductModelConfiguration desenini izler. DbContext.OnModelCreating içinden
    /// tek satırla çağrılır:
    ///
    ///     protected override void OnModelCreating(ModelBuilder modelBuilder)
    ///     {
    ///         base.OnModelCreating(modelBuilder);
    ///         _AttributeModelConfiguration.Apply(modelBuilder);   // ← bu satır
    ///     }
    ///
    /// Konvansiyon: navigation property YOK; ilişkiler HasOne(typeof(X)).WithMany()
    /// .HasForeignKey("...") ile yalnızca FK üzerinden kurulur.
    /// </summary>
    public static class _AttributeModelConfiguration
    {
        /// <summary>
        /// Soft-delete filtresi. Benzersiz indekslerin, soft-delete edilmiş kayıtlarla
        /// çakışmaması için kullanılır (aynı kanonik kod tekrar oluşturulabilsin).
        /// data.Owned.IsDeleted → IsDeletedStatu alanı owned olarak "IsDeleted_IsDeletedStatu"
        /// kolonuna map edilir. EF owned kolonlarını farklı adlandırdıysan bu tek satırı güncelle.
        /// </summary>
        private const string SoftDeleteFilter = "[IsDeleted_IsDeletedStatu] = 0";

        /// <summary>Tüm attribute-kataloğu yapılandırmasını uygular.</summary>
        public static void Apply(ModelBuilder modelBuilder)
        {
            // Önce entity'ler modele kaydedilir (Entity<T> çağrıları) ...
            ApplyGroups(modelBuilder);
            ApplyDefinitions(modelBuilder);
            ApplyOptions(modelBuilder);
            ApplyUnits(modelBuilder);
            ApplyTemplates(modelBuilder);
            ApplyCategoryLinks(modelBuilder);
            ApplyRules(modelBuilder);
            ApplyAi(modelBuilder);
            ApplyIntegration(modelBuilder);

            // ... sonra ortak konvansiyonlar (owned IsDeleted/Ai, RowVersion) tüm
            // kayıtlı entity'lere tek yerden uygulanır. Sıra ÖNEMLİDİR: bu döngü
            // GetEntityTypes() üzerinde çalıştığı için entity'lerin önce kaydı gerekir.
            ApplyBaseConventions(modelBuilder);
        }

        // ── Ortak konvansiyonlar: owned tipler, rowversion, Ai owned ──────────
        private static void ApplyBaseConventions(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes().ToList())
            {
                var clr = entityType.ClrType;
                if (!typeof(AttributeEntityBase).IsAssignableFrom(clr))
                    continue;

                var b = modelBuilder.Entity(clr);

                // Soft-delete owned tipi (mevcut ortak IsDeleted tipi)
                b.OwnsOne(typeof(IsDeleted), nameof(AttributeEntityBase.IsDeleted));

                // İyimser eşzamanlılık damgası
                b.Property(nameof(AttributeEntityBase.RowVersion)).IsRowVersion();

                // AI izlenebilirlik owned tipi (yalnızca Ai property'si olan entity'lerde)
                var aiProp = clr.GetProperty("Ai");
                if (aiProp != null && aiProp.PropertyType == typeof(AiMetadata))
                {
                    b.OwnsOne(typeof(AiMetadata), "Ai", a =>
                    {
                        a.Property(nameof(AiMetadata.Confidence)).HasPrecision(5, 4);
                        a.Property(nameof(AiMetadata.AiModel)).HasMaxLength(128);
                        a.Property(nameof(AiMetadata.AiModelVersion)).HasMaxLength(64);
                        a.Property(nameof(AiMetadata.PromptHash)).HasMaxLength(128);
                        a.Property(nameof(AiMetadata.PromptVersion)).HasMaxLength(64);
                        a.Property(nameof(AiMetadata.ReviewNote)).HasMaxLength(2048);
                        a.Property(nameof(AiMetadata.CategorySource)).HasMaxLength(1024);
                    });
                }
            }
        }

        // ── AttributeGroup ────────────────────────────────────────────────────
        private static void ApplyGroups(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AttributeGroup>(b =>
            {
                b.Property(x => x.CanonicalCode).HasMaxLength(256).IsRequired();
                b.Property(x => x.CanonicalName).HasMaxLength(512).IsRequired();
                b.Property(x => x.IconCssClass).HasMaxLength(128);
                b.HasIndex(x => x.CanonicalCode).IsUnique().HasFilter(SoftDeleteFilter);
            });

            modelBuilder.Entity<AttributeGroupTranslation>(b =>
            {
                b.Property(x => x.Name).HasMaxLength(512).IsRequired();
                b.HasIndex(x => new { x.AttributeGroupId, x.Language }).IsUnique().HasFilter(SoftDeleteFilter);
                b.HasOne(typeof(AttributeGroup)).WithMany()
                    .HasForeignKey(nameof(AttributeGroupTranslation.AttributeGroupId))
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        // ── AttributeDefinition + çeviri/alias/synonym ─────────────────────────
        private static void ApplyDefinitions(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AttributeDefinition>(b =>
            {
                b.Property(x => x.CanonicalCode).HasMaxLength(256).IsRequired();
                b.Property(x => x.NormalizedName).HasMaxLength(256).IsRequired();
                b.Property(x => x.CanonicalName).HasMaxLength(512).IsRequired();
                b.Property(x => x.RegexPattern).HasMaxLength(1024);
                b.Property(x => x.DefaultValue).HasMaxLength(1024);
                b.Property(x => x.MinNumericValue).HasPrecision(38, 10);
                b.Property(x => x.MaxNumericValue).HasPrecision(38, 10);
                b.Property(x => x.NumericStep).HasPrecision(38, 10);

                b.HasIndex(x => x.CanonicalCode).IsUnique().HasFilter(SoftDeleteFilter);
                b.HasIndex(x => x.NormalizedName);
                b.HasIndex(x => x.AttributeGroupId);

                b.HasOne(typeof(AttributeGroup)).WithMany()
                    .HasForeignKey(nameof(AttributeDefinition.AttributeGroupId))
                    .OnDelete(DeleteBehavior.SetNull);
                b.HasOne(typeof(UnitGroup)).WithMany()
                    .HasForeignKey(nameof(AttributeDefinition.UnitGroupId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Unit)).WithMany()
                    .HasForeignKey(nameof(AttributeDefinition.BaseUnitId))
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<AttributeTranslation>(b =>
            {
                b.Property(x => x.Name).HasMaxLength(512).IsRequired();
                b.HasIndex(x => new { x.AttributeDefinitionId, x.Language }).IsUnique().HasFilter(SoftDeleteFilter);
                b.HasOne(typeof(AttributeDefinition)).WithMany()
                    .HasForeignKey(nameof(AttributeTranslation.AttributeDefinitionId))
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<AttributeAlias>(b =>
            {
                b.Property(x => x.Alias).HasMaxLength(512).IsRequired();
                b.Property(x => x.NormalizedAlias).HasMaxLength(256).IsRequired();
                b.HasIndex(x => new { x.AttributeDefinitionId, x.Language, x.NormalizedAlias })
                    .IsUnique().HasFilter(SoftDeleteFilter);
                b.HasOne(typeof(AttributeDefinition)).WithMany()
                    .HasForeignKey(nameof(AttributeAlias.AttributeDefinitionId))
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<AttributeSynonym>(b =>
            {
                b.Property(x => x.Token).HasMaxLength(512).IsRequired();
                b.Property(x => x.NormalizedToken).HasMaxLength(256).IsRequired();
                b.Property(x => x.Confidence).HasPrecision(5, 4);
                // Global tekil: bir token yalnızca tek attribute'a işaret eder.
                b.HasIndex(x => x.NormalizedToken).IsUnique().HasFilter(SoftDeleteFilter);
                b.HasIndex(x => x.AttributeDefinitionId);
                b.HasOne(typeof(AttributeDefinition)).WithMany()
                    .HasForeignKey(nameof(AttributeSynonym.AttributeDefinitionId))
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        // ── AttributeOption + çeviri/alias/synonym ─────────────────────────────
        private static void ApplyOptions(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AttributeOption>(b =>
            {
                b.Property(x => x.CanonicalCode).HasMaxLength(256).IsRequired();
                b.Property(x => x.NormalizedValue).HasMaxLength(256).IsRequired();
                b.Property(x => x.CanonicalValue).HasMaxLength(512).IsRequired();
                b.Property(x => x.ColorHex).HasMaxLength(16);
                b.Property(x => x.ImageUrl).HasMaxLength(1024);
                b.Property(x => x.PathCodes).HasMaxLength(2048);
                b.Property(x => x.NumericValue).HasPrecision(38, 10);

                b.HasIndex(x => new { x.AttributeDefinitionId, x.CanonicalCode }).IsUnique().HasFilter(SoftDeleteFilter);
                b.HasIndex(x => new { x.AttributeDefinitionId, x.NormalizedValue });
                b.HasIndex(x => x.ParentOptionId);

                b.HasOne(typeof(AttributeDefinition)).WithMany()
                    .HasForeignKey(nameof(AttributeOption.AttributeDefinitionId))
                    .OnDelete(DeleteBehavior.Cascade);
                // Ağaç kendine referans → döngü/çoklu-yol hatasını önlemek için NoAction.
                b.HasOne(typeof(AttributeOption)).WithMany()
                    .HasForeignKey(nameof(AttributeOption.ParentOptionId))
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<AttributeOptionTranslation>(b =>
            {
                b.Property(x => x.Value).HasMaxLength(512).IsRequired();
                b.HasIndex(x => new { x.AttributeOptionId, x.Language }).IsUnique().HasFilter(SoftDeleteFilter);
                b.HasOne(typeof(AttributeOption)).WithMany()
                    .HasForeignKey(nameof(AttributeOptionTranslation.AttributeOptionId))
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<AttributeOptionAlias>(b =>
            {
                b.Property(x => x.Alias).HasMaxLength(512).IsRequired();
                b.Property(x => x.NormalizedAlias).HasMaxLength(256).IsRequired();
                b.HasIndex(x => new { x.AttributeOptionId, x.Language, x.NormalizedAlias })
                    .IsUnique().HasFilter(SoftDeleteFilter);
                b.HasOne(typeof(AttributeOption)).WithMany()
                    .HasForeignKey(nameof(AttributeOptionAlias.AttributeOptionId))
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<AttributeOptionSynonym>(b =>
            {
                b.Property(x => x.Token).HasMaxLength(512).IsRequired();
                b.Property(x => x.NormalizedToken).HasMaxLength(256).IsRequired();
                b.Property(x => x.Confidence).HasPrecision(5, 4);
                // Attribute kapsamında tekil: aynı token aynı attribute içinde tek option'a.
                b.HasIndex(x => new { x.AttributeDefinitionId, x.NormalizedToken })
                    .IsUnique().HasFilter(SoftDeleteFilter);
                b.HasIndex(x => x.AttributeOptionId);
                b.HasOne(typeof(AttributeOption)).WithMany()
                    .HasForeignKey(nameof(AttributeOptionSynonym.AttributeOptionId))
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        // ── Unit / UnitGroup ──────────────────────────────────────────────────
        private static void ApplyUnits(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UnitGroup>(b =>
            {
                b.Property(x => x.CanonicalCode).HasMaxLength(256).IsRequired();
                b.Property(x => x.CanonicalName).HasMaxLength(512).IsRequired();
                b.HasIndex(x => x.CanonicalCode).IsUnique().HasFilter(SoftDeleteFilter);
                // Baz birim dairesel FK → NoAction.
                b.HasOne(typeof(Unit)).WithMany()
                    .HasForeignKey(nameof(UnitGroup.BaseUnitId))
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<UnitGroupTranslation>(b =>
            {
                b.Property(x => x.Name).HasMaxLength(512).IsRequired();
                b.HasIndex(x => new { x.UnitGroupId, x.Language }).IsUnique().HasFilter(SoftDeleteFilter);
                b.HasOne(typeof(UnitGroup)).WithMany()
                    .HasForeignKey(nameof(UnitGroupTranslation.UnitGroupId))
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Unit>(b =>
            {
                b.Property(x => x.CanonicalCode).HasMaxLength(256).IsRequired();
                b.Property(x => x.Symbol).HasMaxLength(32).IsRequired();
                b.Property(x => x.CanonicalName).HasMaxLength(512).IsRequired();
                b.Property(x => x.ConversionFactorToBase).HasPrecision(38, 18);
                b.Property(x => x.ConversionOffset).HasPrecision(38, 18);
                b.HasIndex(x => new { x.UnitGroupId, x.CanonicalCode }).IsUnique().HasFilter(SoftDeleteFilter);
                b.HasIndex(x => new { x.UnitGroupId, x.Symbol }).IsUnique().HasFilter(SoftDeleteFilter);
                b.HasOne(typeof(UnitGroup)).WithMany()
                    .HasForeignKey(nameof(Unit.UnitGroupId))
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UnitTranslation>(b =>
            {
                b.Property(x => x.Name).HasMaxLength(512).IsRequired();
                b.Property(x => x.PluralName).HasMaxLength(512);
                b.HasIndex(x => new { x.UnitId, x.Language }).IsUnique().HasFilter(SoftDeleteFilter);
                b.HasOne(typeof(Unit)).WithMany()
                    .HasForeignKey(nameof(UnitTranslation.UnitId))
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        // ── Template ──────────────────────────────────────────────────────────
        private static void ApplyTemplates(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AttributeTemplate>(b =>
            {
                b.Property(x => x.CanonicalCode).HasMaxLength(256).IsRequired();
                b.Property(x => x.CanonicalName).HasMaxLength(512).IsRequired();
                b.HasIndex(x => x.CanonicalCode).IsUnique().HasFilter(SoftDeleteFilter);
                b.HasOne(typeof(AttributeTemplate)).WithMany()
                    .HasForeignKey(nameof(AttributeTemplate.SupersededByTemplateId))
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<AttributeTemplateTranslation>(b =>
            {
                b.Property(x => x.Name).HasMaxLength(512).IsRequired();
                b.HasIndex(x => new { x.AttributeTemplateId, x.Language }).IsUnique().HasFilter(SoftDeleteFilter);
                b.HasOne(typeof(AttributeTemplate)).WithMany()
                    .HasForeignKey(nameof(AttributeTemplateTranslation.AttributeTemplateId))
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TemplateAttribute>(b =>
            {
                b.HasIndex(x => new { x.AttributeTemplateId, x.AttributeDefinitionId }).IsUnique().HasFilter(SoftDeleteFilter);
                b.HasIndex(x => x.AttributeDefinitionId);
                b.HasOne(typeof(AttributeTemplate)).WithMany()
                    .HasForeignKey(nameof(TemplateAttribute.AttributeTemplateId))
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(AttributeDefinition)).WithMany()
                    .HasForeignKey(nameof(TemplateAttribute.AttributeDefinitionId))
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TemplateCategory>(b =>
            {
                b.HasIndex(x => new { x.AttributeTemplateId, x.CategoryId }).IsUnique().HasFilter(SoftDeleteFilter);
                b.HasIndex(x => x.CategoryId);
                b.HasOne(typeof(AttributeTemplate)).WithMany()
                    .HasForeignKey(nameof(TemplateCategory.AttributeTemplateId))
                    .OnDelete(DeleteBehavior.Cascade);
                // CategoryId → CategoriesProduct (Guid PK): dış tabloya FK isterseniz burada
                // .HasOne(typeof(CategoriesProduct))... ekleyin (varsayılan: yalnızca indeks).
            });
        }

        // ── CategoryAttribute ─────────────────────────────────────────────────
        private static void ApplyCategoryLinks(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CategoryAttribute>(b =>
            {
                b.HasIndex(x => new { x.CategoryId, x.AttributeDefinitionId }).IsUnique().HasFilter(SoftDeleteFilter);
                b.HasIndex(x => x.AttributeDefinitionId);
                b.HasIndex(x => x.SourceTemplateId);
                b.HasOne(typeof(AttributeDefinition)).WithMany()
                    .HasForeignKey(nameof(CategoryAttribute.AttributeDefinitionId))
                    .OnDelete(DeleteBehavior.Restrict);
                b.HasOne(typeof(AttributeTemplate)).WithMany()
                    .HasForeignKey(nameof(CategoryAttribute.SourceTemplateId))
                    .OnDelete(DeleteBehavior.NoAction);
                // CategoryId → CategoriesProduct (Guid PK): dış FK isterseniz burada ekleyin.
            });
        }

        // ── Kurallar: Dependency, NormalizationRule ────────────────────────────
        private static void ApplyRules(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AttributeDependency>(b =>
            {
                b.Property(x => x.ExpectedValue).HasMaxLength(1024);
                b.HasIndex(x => new { x.CategoryId, x.SourceAttributeDefinitionId });
                b.HasIndex(x => x.TargetAttributeDefinitionId);
                // Aynı tabloya iki FK (Source/Target) → çoklu-yol hatasını önlemek için NoAction.
                b.HasOne(typeof(AttributeDefinition)).WithMany()
                    .HasForeignKey(nameof(AttributeDependency.SourceAttributeDefinitionId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(AttributeDefinition)).WithMany()
                    .HasForeignKey(nameof(AttributeDependency.TargetAttributeDefinitionId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(AttributeOption)).WithMany()
                    .HasForeignKey(nameof(AttributeDependency.ExpectedOptionId))
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<NormalizationRule>(b =>
            {
                b.Property(x => x.CanonicalCode).HasMaxLength(256).IsRequired();
                b.Property(x => x.Name).HasMaxLength(512).IsRequired();
                b.Property(x => x.Pattern).HasMaxLength(2048);
                b.Property(x => x.Replacement).HasMaxLength(2048);
                b.HasIndex(x => x.CanonicalCode).IsUnique().HasFilter(SoftDeleteFilter);
            });
        }

        // ── AI yönetişimi ─────────────────────────────────────────────────────
        private static void ApplyAi(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AiGenerationJob>(b =>
            {
                b.Property(x => x.IdempotencyKey).HasMaxLength(256).IsRequired();
                b.Property(x => x.PromptVersion).HasMaxLength(64);
                b.Property(x => x.PromptHash).HasMaxLength(128);
                b.Property(x => x.LeasedBy).HasMaxLength(128);
                b.Property(x => x.CategoryPathSnapshot).HasMaxLength(2048);
                b.Property(x => x.ResultSummary).HasMaxLength(2048);
                b.Property(x => x.ErrorMessage).HasMaxLength(4000);
                b.HasIndex(x => x.IdempotencyKey).IsUnique().HasFilter(SoftDeleteFilter);
                b.HasIndex(x => new { x.Status, x.Priority, x.NextRetryAtUtc });
                b.HasIndex(x => new { x.JobType, x.Status });
            });

            modelBuilder.Entity<AiGenerationHistory>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.EntityType).HasMaxLength(128).IsRequired();
                b.Property(x => x.Action).HasMaxLength(64).IsRequired();
                b.Property(x => x.AiModel).HasMaxLength(128);
                b.Property(x => x.AiModelVersion).HasMaxLength(64);
                b.Property(x => x.PromptHash).HasMaxLength(128);
                b.Property(x => x.PromptVersion).HasMaxLength(64);
                b.Property(x => x.Reason).HasMaxLength(2048);
                b.Property(x => x.Confidence).HasPrecision(5, 4);
                b.HasIndex(x => new { x.EntityType, x.EntityId, x.CreatedAtUtc });
                b.HasIndex(x => x.AiGenerationJobId);
            });
        }

        // ── Entegrasyon eşleştirmeleri ─────────────────────────────────────────
        private static void ApplyIntegration(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IntegrationPlatform>(b =>
            {
                b.Property(x => x.CanonicalCode).HasMaxLength(256).IsRequired();
                b.Property(x => x.Name).HasMaxLength(256).IsRequired();
                b.HasIndex(x => x.CanonicalCode).IsUnique().HasFilter(SoftDeleteFilter);
            });

            modelBuilder.Entity<AttributeMapping>(b =>
            {
                b.Property(x => x.ExternalCode).HasMaxLength(256).IsRequired();
                b.Property(x => x.ExternalId).HasMaxLength(256);
                b.Property(x => x.ExternalName).HasMaxLength(512);
                b.Property(x => x.TransformRuleJson).HasMaxLength(4000);
                b.HasIndex(x => new { x.IntegrationPlatformId, x.AttributeDefinitionId, x.Direction })
                    .IsUnique().HasFilter(SoftDeleteFilter);
                b.HasIndex(x => new { x.IntegrationPlatformId, x.ExternalCode });
                // Attribute → Cascade; Platform → NoAction (çoklu cascade-yolu engeli).
                b.HasOne(typeof(AttributeDefinition)).WithMany()
                    .HasForeignKey(nameof(AttributeMapping.AttributeDefinitionId))
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(IntegrationPlatform)).WithMany()
                    .HasForeignKey(nameof(AttributeMapping.IntegrationPlatformId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(NormalizationRule)).WithMany()
                    .HasForeignKey(nameof(AttributeMapping.NormalizationRuleId))
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<AttributeOptionMapping>(b =>
            {
                b.Property(x => x.ExternalCode).HasMaxLength(256).IsRequired();
                b.Property(x => x.ExternalValue).HasMaxLength(512);
                b.Property(x => x.ExternalId).HasMaxLength(256);
                b.HasIndex(x => new { x.IntegrationPlatformId, x.AttributeOptionId, x.Direction })
                    .IsUnique().HasFilter(SoftDeleteFilter);
                b.HasIndex(x => new { x.IntegrationPlatformId, x.AttributeDefinitionId, x.ExternalCode });
                b.HasOne(typeof(AttributeOption)).WithMany()
                    .HasForeignKey(nameof(AttributeOptionMapping.AttributeOptionId))
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(IntegrationPlatform)).WithMany()
                    .HasForeignKey(nameof(AttributeOptionMapping.IntegrationPlatformId))
                    .OnDelete(DeleteBehavior.NoAction);
                // AttributeDefinitionId denormalize → yalnızca indeks (ek FK yok).
            });
        }
    }
}
```

## Dosya: _Attribute.Options.cs
```csharp
using System;

namespace data._Attribute
{
    /// <summary>
    /// Bir attribute'ın seçilebilir KANONİK değeri (ör. Renk → Siyah; RAM → 16;
    /// CPU → Intel Core Ultra 7). Merkezî sözlüğün "değer" tarafıdır; ürünün
    /// gerçek attribute değeri (ProductAttributeValues) bu option'a ID ile bağlanır.
    ///
    /// Ağaç (tree) desteği: <see cref="ParentOptionId"/> ile hiyerarşik option'lar
    /// (ör. Renk → Maviler → Lacivert) modellenebilir.
    /// </summary>
    public class AttributeOption : AttributeEntityBase
    {
        /// <summary>Bağlı attribute.</summary>
        public Guid AttributeDefinitionId { get; set; }

        /// <summary>Üst option (ağaç yapısı için, opsiyonel).</summary>
        public Guid? ParentOptionId { get; set; }

        /// <summary>
        /// Attribute içinde tekil kanonik kod (ör. "black", "16", "intel_core_ultra_7").
        /// (AttributeDefinitionId, CanonicalCode) benzersizdir.
        /// </summary>
        public string CanonicalCode { get; set; } = string.Empty;

        /// <summary>Normalize edilmiş değer (dedup/arama için indeksli).</summary>
        public string NormalizedValue { get; set; } = string.Empty;

        /// <summary>Varsayılan (Türkçe) görünen değer.</summary>
        public string CanonicalValue { get; set; } = string.Empty;

        /// <summary>Renk option'ları için hex kod (ör. "#000000").</summary>
        public string? ColorHex { get; set; }

        /// <summary>Option için görsel/örnek (swatch) URL'i.</summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Sayısal option'lar için karşılaştırılabilir sayısal değer
        /// (ör. RAM "16" → 16). Doğru sayısal sıralama/filtreleme sağlar.
        /// </summary>
        public decimal? NumericValue { get; set; }

        /// <summary>Sıralama önceliği.</summary>
        public int DisplayOrder { get; set; }

        /// <summary>Ağaç derinliği (denormalize; sorgu kolaylığı için).</summary>
        public int Level { get; set; }

        /// <summary>
        /// Materialize edilmiş yol (denormalize; ağaç sorguları için, opsiyonel).
        /// Ör: "renkler/maviler/lacivert".
        /// </summary>
        public string? PathCodes { get; set; }

        /// <summary>Option aktif mi?</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>Yapay zekâ izlenebilirlik + manuel koruma bilgisi.</summary>
        public AiMetadata Ai { get; set; } = new();
    }

    /// <summary>
    /// <see cref="AttributeOption"/> için dile özgü metinler.
    /// (AttributeOptionId, Language) benzersizdir.
    /// </summary>
    public class AttributeOptionTranslation : AttributeEntityBase
    {
        /// <summary>Bağlı option.</summary>
        public Guid AttributeOptionId { get; set; }

        /// <summary>Dil.</summary>
        public Language Language { get; set; }

        /// <summary>Bu dildeki görünen değer (ör. "Siyah" / "Black" / "Schwarz").</summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>Kaydın kaynağı.</summary>
        public ContentSource Source { get; set; } = ContentSource.Manual;

        /// <summary>Admin bu çeviriyi elle düzenledi mi? (AI/Import ezme kilidi).</summary>
        public bool IsManuallyEdited { get; set; }
    }

    /// <summary>Option için insana dönük alternatif görünen ad (alias).</summary>
    public class AttributeOptionAlias : AttributeEntityBase
    {
        /// <summary>Bağlı option.</summary>
        public Guid AttributeOptionId { get; set; }

        /// <summary>Alias dili (null = dilden bağımsız).</summary>
        public Language? Language { get; set; }

        /// <summary>Alias metni.</summary>
        public string Alias { get; set; } = string.Empty;

        /// <summary>Normalize edilmiş alias (indeksli).</summary>
        public string NormalizedAlias { get; set; } = string.Empty;

        /// <summary>Kaydın kaynağı.</summary>
        public ContentSource Source { get; set; } = ContentSource.Manual;
    }

    /// <summary>
    /// Option için makine eşleştirme/dedup tokenı (synonym).
    /// Ör: "Gray", "Grey", "Gri" → aynı option. AI aynı option'ı ikinci kez üretmez.
    /// (AttributeDefinitionId, NormalizedToken) attribute kapsamında tekildir.
    /// </summary>
    public class AttributeOptionSynonym : AttributeEntityBase
    {
        /// <summary>Bağlı option.</summary>
        public Guid AttributeOptionId { get; set; }

        /// <summary>
        /// Denormalize attribute referansı — dedup'ı attribute kapsamında
        /// (option bazında değil) hızlı kısıtlamak için tutulur.
        /// </summary>
        public Guid AttributeDefinitionId { get; set; }

        /// <summary>Ham token metni.</summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>Normalize edilmiş token (indeksli).</summary>
        public string NormalizedToken { get; set; } = string.Empty;

        /// <summary>Token dili (null = dilden bağımsız).</summary>
        public Language? Language { get; set; }

        /// <summary>Kaynak.</summary>
        public ContentSource Source { get; set; } = ContentSource.Manual;

        /// <summary>AI güven skoru (0..1).</summary>
        public decimal? Confidence { get; set; }
    }
}
```

## Dosya: _Attribute.Rules.cs
```csharp
using System;

namespace data._Attribute
{
    /// <summary>
    /// Attribute bağımlılığı (koşullu görünürlük/zorunluluk).
    /// Ör: "Has Bluetooth = false" ise "Bluetooth Version" gizlenir;
    ///     "Gender = Female" ise "Bra Size" gösterilir.
    ///
    /// Birden çok koşul <see cref="ConditionGroup"/> ile gruplanabilir ve grup içi
    /// mantık <see cref="GroupLogic"/> (AND/OR) ile belirlenir. Bağımlılık global
    /// (CategoryId null) veya belirli bir kategoriye özgü olabilir.
    /// </summary>
    public class AttributeDependency : AttributeEntityBase
    {
        /// <summary>Kapsam kategorisi (null = global; CategoriesProduct.Id — Guid).</summary>
        public Guid? CategoryId { get; set; }

        /// <summary>Koşulu tetikleyen (kontrol eden) attribute.</summary>
        public Guid SourceAttributeDefinitionId { get; set; }

        /// <summary>Karşılaştırma operatörü.</summary>
        public DependencyOperator Operator { get; set; } = DependencyOperator.Equals;

        /// <summary>Karşılaştırılan option (option tabanlı koşullarda).</summary>
        public Guid? ExpectedOptionId { get; set; }

        /// <summary>Karşılaştırılan skaler değer (sayı/metin/boolean koşullarında).</summary>
        public string? ExpectedValue { get; set; }

        /// <summary>Koşul sağlandığında etkilenen hedef attribute.</summary>
        public Guid TargetAttributeDefinitionId { get; set; }

        /// <summary>Uygulanacak eylem (göster/gizle/zorunlu yap...).</summary>
        public DependencyAction Action { get; set; } = DependencyAction.Show;

        /// <summary>Koşul grubu numarası (aynı grup birlikte değerlendirilir).</summary>
        public int ConditionGroup { get; set; }

        /// <summary>Grup içi mantıksal birleşim (AND/OR).</summary>
        public DependencyLogic GroupLogic { get; set; } = DependencyLogic.And;

        /// <summary>Sıralama önceliği.</summary>
        public int DisplayOrder { get; set; }

        /// <summary>Kural aktif mi?</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>Yapay zekâ izlenebilirlik + manuel koruma bilgisi.</summary>
        public AiMetadata Ai { get; set; } = new();
    }

    /// <summary>
    /// Yeniden kullanılabilir normalizasyon/transform kuralı. Import ve dedup
    /// sırasında dış değerleri kanonik biçime getirmek için kullanılır
    /// (ör. "16GB" → "16", "grey" → "gray", büyük/küçük harf düzeltme).
    /// AttributeMapping/OptionMapping bu kurallara ID ile bağlanabilir.
    /// </summary>
    public class NormalizationRule : AttributeEntityBase
    {
        /// <summary>Tekil kod (ör. "strip_gb_suffix").</summary>
        public string CanonicalCode { get; set; } = string.Empty;

        /// <summary>İnsan-okur ad.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Kural türü.</summary>
        public NormalizationRuleType RuleType { get; set; } = NormalizationRuleType.Lowercase;

        /// <summary>Desen/kaynak (regex, aranan değer vb.).</summary>
        public string? Pattern { get; set; }

        /// <summary>Değiştirilecek hedef (regex replace / map hedefi).</summary>
        public string? Replacement { get; set; }

        /// <summary>Uygulama sırası (küçük = önce).</summary>
        public int Priority { get; set; }

        /// <summary>Yalnızca belirli veri tipine uygulanır (null = tümü).</summary>
        public AttributeDataType? AppliesToDataType { get; set; }

        /// <summary>Açıklama.</summary>
        public string? Description { get; set; }

        /// <summary>Kural aktif mi?</summary>
        public bool IsActive { get; set; } = true;
    }
}
```

## Dosya: _Attribute.Templates.cs
```csharp
using System;

namespace data._Attribute
{
    /// <summary>
    /// Kategori sınıfına atanabilen, yeniden kullanılabilir ATTRIBUTE KÜMESİ şablonu
    /// (ör. "Laptop" template'i: RAM, CPU, GPU, SSD, Ekran...). 50.000 kategori için
    /// AI her seferinde sıfırdan üretmesin diye Gaming/Business/Ultrabook gibi benzer
    /// kategoriler AYNI template'i paylaşır.
    ///
    /// Önemli: Bu, mevcut data._Product.AttributeTemplates ile KARIŞTIRILMAMALIDIR.
    /// Oradaki şablon ürün-özellik şablonudur; buradaki ise kategori → attribute-kümesi
    /// katalog şablonudur (farklı namespace, farklı sorumluluk).
    ///
    /// Immutable evrim: Template değişince yeni sürüm oluşturmak için
    /// <see cref="Version"/> + <see cref="SupersededByTemplateId"/> kullanılır;
    /// geçmiş ürün verisi sessizce değişmez.
    /// </summary>
    public class AttributeTemplate : AttributeEntityBase
    {
        /// <summary>Dilden bağımsız tekil kod (ör. "laptop_v1").</summary>
        public string CanonicalCode { get; set; } = string.Empty;

        /// <summary>Varsayılan (Türkçe) görünen ad (ör. "Laptop Şablonu").</summary>
        public string CanonicalName { get; set; } = string.Empty;

        /// <summary>Açıklama (opsiyonel).</summary>
        public string? Description { get; set; }

        /// <summary>Sürüm numarası (immutable evrim izleme).</summary>
        public int Version { get; set; } = 1;

        /// <summary>Bu template'i devralan yeni sürüm (varsa).</summary>
        public Guid? SupersededByTemplateId { get; set; }

        /// <summary>Template aktif mi?</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>Yapay zekâ izlenebilirlik + manuel koruma bilgisi.</summary>
        public AiMetadata Ai { get; set; } = new();
    }

    /// <summary><see cref="AttributeTemplate"/> için dile özgü metinler.</summary>
    public class AttributeTemplateTranslation : AttributeEntityBase
    {
        /// <summary>Bağlı template.</summary>
        public Guid AttributeTemplateId { get; set; }

        /// <summary>Dil.</summary>
        public Language Language { get; set; }

        /// <summary>Bu dildeki template adı.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Bu dildeki açıklama.</summary>
        public string? Description { get; set; }

        /// <summary>Kaynak.</summary>
        public ContentSource Source { get; set; } = ContentSource.Manual;

        /// <summary>Manuel düzenleme kilidi.</summary>
        public bool IsManuallyEdited { get; set; }
    }

    /// <summary>
    /// Template ↔ Attribute bağıntısı: bir template'in içerdiği attribute'lar ve
    /// bu template kapsamındaki override'ları.
    /// (AttributeTemplateId, AttributeDefinitionId) benzersizdir.
    /// </summary>
    public class TemplateAttribute : AttributeEntityBase
    {
        /// <summary>Bağlı template.</summary>
        public Guid AttributeTemplateId { get; set; }

        /// <summary>Bağlı attribute.</summary>
        public Guid AttributeDefinitionId { get; set; }

        /// <summary>Template içindeki sıralama.</summary>
        public int DisplayOrder { get; set; }

        /// <summary>Zorunluluk override'ı (null = attribute varsayılanını kullan).</summary>
        public bool? IsRequiredOverride { get; set; }

        /// <summary>Varyant override'ı (null = attribute varsayılanını kullan).</summary>
        public bool? IsVariantOverride { get; set; }

        /// <summary>Filtrelenebilirlik override'ı (null = varsayılan).</summary>
        public bool? IsFilterableOverride { get; set; }

        /// <summary>Karşılaştırılabilirlik override'ı (null = varsayılan).</summary>
        public bool? IsComparableOverride { get; set; }

        /// <summary>Ürün sayfası görünürlüğü override'ı (null = varsayılan).</summary>
        public bool? IsVisibleOverride { get; set; }

        /// <summary>Görsel grup override'ı (null = attribute'ın kendi grubu).</summary>
        public Guid? AttributeGroupOverrideId { get; set; }

        /// <summary>Yapay zekâ izlenebilirlik + manuel koruma bilgisi.</summary>
        public AiMetadata Ai { get; set; } = new();
    }

    /// <summary>
    /// Template ↔ Category bağıntısı: bir template'in hangi kategoriler tarafından
    /// kullanıldığı. Not: CategoryId, ürün kategorisi CategoriesProduct (Guid PK) tablosuna işaret eder.
    /// (AttributeTemplateId, CategoryId) benzersizdir.
    /// </summary>
    public class TemplateCategory : AttributeEntityBase
    {
        /// <summary>Bağlı template.</summary>
        public Guid AttributeTemplateId { get; set; }

        /// <summary>Bağlı kategori (CategoriesProduct.Id — Guid).</summary>
        public Guid CategoryId { get; set; }

        /// <summary>Bu kategori için birincil template mi?</summary>
        public bool IsPrimary { get; set; } = true;

        /// <summary>Kaynak (özellikle AI önerisi için).</summary>
        public ContentSource Source { get; set; } = ContentSource.Manual;

        /// <summary>Yapay zekâ izlenebilirlik + manuel koruma bilgisi.</summary>
        public AiMetadata Ai { get; set; } = new();
    }
}
```

## Dosya: _Attribute.Units.cs
```csharp
using System;

namespace data._Attribute
{
    /// <summary>
    /// Birim grubu / ölçüm ailesi (ör. Ağırlık, Uzunluk, Veri, Bellek, Ekran).
    /// Aynı gruptaki tüm birimler ortak bir baz birime dönüştürülebilir; böylece
    /// normalize edilmiş değer saklanıp doğru karşılaştırma/filtreleme yapılır.
    /// </summary>
    public class UnitGroup : AttributeEntityBase
    {
        /// <summary>Dilden bağımsız tekil kod (ör. "weight", "data_storage").</summary>
        public string CanonicalCode { get; set; } = string.Empty;

        /// <summary>Varsayılan (Türkçe) görünen ad.</summary>
        public string CanonicalName { get; set; } = string.Empty;

        /// <summary>Fiziksel boyut / ölçüm ailesi.</summary>
        public UnitDimension Dimension { get; set; } = UnitDimension.Custom;

        /// <summary>
        /// Grubun baz/temel birimi (normalize saklama için). Ör: Ağırlık → gram.
        /// (Dairesel FK olduğundan silme davranışı NoAction'dır.)
        /// </summary>
        public Guid? BaseUnitId { get; set; }

        /// <summary>Sıralama önceliği.</summary>
        public int DisplayOrder { get; set; }

        /// <summary>Grup aktif mi?</summary>
        public bool IsActive { get; set; } = true;
    }

    /// <summary><see cref="UnitGroup"/> için dile özgü metinler.</summary>
    public class UnitGroupTranslation : AttributeEntityBase
    {
        /// <summary>Bağlı birim grubu.</summary>
        public Guid UnitGroupId { get; set; }

        /// <summary>Dil.</summary>
        public Language Language { get; set; }

        /// <summary>Bu dildeki grup adı.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Kaynak.</summary>
        public ContentSource Source { get; set; } = ContentSource.Manual;

        /// <summary>Manuel düzenleme kilidi.</summary>
        public bool IsManuallyEdited { get; set; }
    }

    /// <summary>
    /// Tek bir ölçü birimi (ör. kg, g, mm, cm, GB, MB, °C).
    /// Baz birime dönüşüm için çarpan ve (gerekirse) ofset taşır.
    /// </summary>
    public class Unit : AttributeEntityBase
    {
        /// <summary>Bağlı birim grubu.</summary>
        public Guid UnitGroupId { get; set; }

        /// <summary>Grup içinde tekil kod (ör. "kg", "gb").</summary>
        public string CanonicalCode { get; set; } = string.Empty;

        /// <summary>Görünen sembol (ör. "kg", "GB", "cm").</summary>
        public string Symbol { get; set; } = string.Empty;

        /// <summary>Varsayılan (Türkçe) görünen ad (ör. "Kilogram").</summary>
        public string CanonicalName { get; set; } = string.Empty;

        /// <summary>
        /// Baz birime çevirme çarpanı. Normalize değer = (değer * Çarpan) + Ofset.
        /// Ör: gram baz iken kg için çarpan 1000.
        /// </summary>
        public decimal ConversionFactorToBase { get; set; } = 1m;

        /// <summary>
        /// Baz birime çevirmede ofset (çoğunlukla 0; sıcaklık gibi durumlarda gerekir).
        /// </summary>
        public decimal ConversionOffset { get; set; } = 0m;

        /// <summary>Bu birim grubun baz birimi mi?</summary>
        public bool IsBaseUnit { get; set; }

        /// <summary>Sıralama önceliği.</summary>
        public int DisplayOrder { get; set; }

        /// <summary>Birim aktif mi?</summary>
        public bool IsActive { get; set; } = true;
    }

    /// <summary><see cref="Unit"/> için dile özgü metinler.</summary>
    public class UnitTranslation : AttributeEntityBase
    {
        /// <summary>Bağlı birim.</summary>
        public Guid UnitId { get; set; }

        /// <summary>Dil.</summary>
        public Language Language { get; set; }

        /// <summary>Bu dildeki birim adı (tekil, ör. "Kilogram").</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Bu dildeki çoğul ad (opsiyonel, ör. "Kilograms").</summary>
        public string? PluralName { get; set; }

        /// <summary>Kaynak.</summary>
        public ContentSource Source { get; set; } = ContentSource.Manual;

        /// <summary>Manuel düzenleme kilidi.</summary>
        public bool IsManuallyEdited { get; set; }
    }
}
```


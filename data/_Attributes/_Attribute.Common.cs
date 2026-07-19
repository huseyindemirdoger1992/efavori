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

using System;

namespace data._Products
{
    // ════════════════════════════════════════════════════════════════════════
    //  efavori — Ürün Yorum / Puanlama & Soru-Cevap Sistemi (Review & QA V1)
    //  Enum kataloğu — dilden bağımsız, tinyint (byte) olarak saklanan sabitler.
    //  Not: Enum değerlerine ASLA arada değer eklemeyin; yalnızca SONA ekleyin.
    // ════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Kullanıcı üretimi içeriğin moderasyon durumu.
    /// Vitrinde yalnızca Approved / AutoApproved kayıtlar gösterilir.
    ///
    /// PLATFORM GENELİNDE ORTAK ENUM'DIR (§23): ürün yorumu/sorusu ve sosyal içerik
    /// (gönderi, yorum, medya) aynı durum makinesini kullanır; moderasyon paneli
    /// tek bir kuyruk mantığıyla çalışır.
    /// </summary>
    public enum ModerationStatus : byte
    {
        /// <summary>Gönderildi, moderasyon bekliyor.</summary>
        Pending = 1,
        /// <summary>Onaylandı, yayında.</summary>
        Approved = 2,
        /// <summary>Reddedildi (küfür/spam/politika ihlali).</summary>
        Rejected = 3,
        /// <summary>Otomatik/manuel spam işaretli.</summary>
        Spam = 4,
        /// <summary>Kullanıcı tarafından geri çekildi.</summary>
        Withdrawn = 5,
        /// <summary>Kullanıcı şikâyeti sonrası incelemede gizlendi.</summary>
        UnderReview = 6,

        // ── Sosyal içerik moderasyonu için eklenenler (§23) ──────────────────
        /// <summary>Vitrinde gizlendi ama silinmedi (yazarı görebilir).</summary>
        Hidden = 7,
        /// <summary>Moderatör tarafından kaldırıldı.</summary>
        Removed = 8,
        /// <summary>Yapay zekâ ön denetiminden geçti, insan onayı beklemiyor.</summary>
        AutoApproved = 9,
        /// <summary>Yapay zekâ riskli buldu, insan incelemesi kuyruğunda.</summary>
        AutoFlagged = 10,
        /// <summary>Yaş/hassasiyet kısıtı uygulandı (bulanıklaştırılarak gösterilir).</summary>
        AgeRestricted = 11
    }

    /// <summary>
    /// Bir yoruma/cevaba yapılan oy türü (fayda oyu). Amazon "Was this helpful?" karşılığı.
    /// </summary>
    public enum VoteType : byte
    {
        /// <summary>Faydalı buldu.</summary>
        Helpful = 1,
        /// <summary>Faydalı bulmadı.</summary>
        NotHelpful = 2
    }

    /// <summary>
    /// Bir kullanıcı üretimi içeriğin şikâyet (report) gerekçesi.
    ///
    /// PLATFORM GENELİNDE ORTAK ENUM'DIR (§22): ürün yorumları/soruları
    /// (ProductReviewReports, ProductQuestionReports) ve sosyal içerik
    /// (data._Shares.ContentReports — gönderi, yorum, kullanıcı, mağaza, mesaj)
    /// AYNI gerekçe sözlüğünü kullanır. Böylece moderasyon raporları tek bir
    /// kavram kümesi üzerinden üretilebilir ve ekip iki farklı sözlük öğrenmez.
    ///
    /// Değerler 1-5 mevcut sürümden korunmuştur; sosyal ağ için gereken gerekçeler
    /// SONA eklenmiştir (mevcut veriler bozulmaz).
    /// </summary>
    public enum ReportReason : byte
    {
        /// <summary>Spam / reklam.</summary>
        Spam = 1,
        /// <summary>Küfür / uygunsuz dil.</summary>
        Offensive = 2,
        /// <summary>Konu dışı / alakasız.</summary>
        OffTopic = 3,
        /// <summary>Yanıltıcı / sahte bilgi.</summary>
        Misinformation = 4,
        /// <summary>Kişisel bilgi ifşası.</summary>
        PrivacyViolation = 5,

        // ── Sosyal ağ için eklenen gerekçeler ────────────────────────────────
        /// <summary>Taciz / zorbalık — belirli bir kişiyi hedef alan saldırı.</summary>
        Harassment = 6,
        /// <summary>Nefret söylemi — korunan bir gruba yönelik saldırı.</summary>
        HateSpeech = 7,
        /// <summary>Şiddet içerikli veya şiddete teşvik eden içerik.</summary>
        Violence = 8,
        /// <summary>Cinsel içerik / müstehcenlik.</summary>
        SexualContent = 9,
        /// <summary>Çocuk istismarı şüphesi — EN YÜKSEK ÖNCELİKLE işlenir.</summary>
        ChildSafety = 10,
        /// <summary>Kendine zarar verme veya intihar içeriği — destek akışına yönlendirilir.</summary>
        SelfHarm = 11,
        /// <summary>Sahte hesap / kimlik taklidi.</summary>
        Impersonation = 12,
        /// <summary>Dolandırıcılık / sahte satış.</summary>
        Fraud = 13,
        /// <summary>Telif hakkı veya marka ihlali.</summary>
        IntellectualProperty = 14,
        /// <summary>Yasa dışı ürün veya hizmet satışı.</summary>
        IllegalGoods = 15,
        /// <summary>Sahte/ücretli yorum şüphesi.</summary>
        FakeReview = 16,

        /// <summary>Diğer (Description alanı doldurulur).</summary>
        Other = 100
    }

    /// <summary>
    /// Şikâyetin işlenme durumu. Ürün ve sosyal içerik şikâyetlerinde ORTAK kullanılır (§22).
    /// </summary>
    public enum ReportStatus : byte
    {
        /// <summary>İncelenmeyi bekliyor.</summary>
        Open = 1,
        /// <summary>İncelendi, işlem yapıldı (içerik kaldırıldı/gizlendi).</summary>
        ActionTaken = 2,
        /// <summary>İncelendi, geçersiz bulundu (reddedildi).</summary>
        Dismissed = 3,

        // ── Sosyal ağ moderasyon kuyruğu için eklenenler ─────────────────────
        /// <summary>Bir moderatör kuyruktan aldı ve inceliyor (lease deseni).</summary>
        UnderReview = 4,
        /// <summary>Üst seviye incelemeye (yasal/güven ekibi) yükseltildi.</summary>
        Escalated = 5,
        /// <summary>Aynı içerik için açılmış başka bir şikâyetle birleştirildi.</summary>
        Duplicate = 6,
        /// <summary>Otomatik sistem tarafından kapatıldı (içerik zaten kaldırılmıştı).</summary>
        AutoResolved = 7
    }
}

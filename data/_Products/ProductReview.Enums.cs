using System;

namespace data._Products
{
    // ════════════════════════════════════════════════════════════════════════
    //  efavori — Ürün Yorum / Puanlama & Soru-Cevap Sistemi (Review & QA V1)
    //  Enum kataloğu — dilden bağımsız, tinyint (byte) olarak saklanan sabitler.
    //  Not: Enum değerlerine ASLA arada değer eklemeyin; yalnızca SONA ekleyin.
    // ════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Kullanıcı üretimi içeriğin (yorum / cevap / soru) moderasyon durumu.
    /// Vitrinde yalnızca Approved kayıtlar gösterilir.
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
        UnderReview = 6
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

    /// <summary>Bir kullanıcı üretimi içeriğin şikâyet (report) gerekçesi.</summary>
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
        /// <summary>Diğer (Description alanı doldurulur).</summary>
        Other = 100
    }

    /// <summary>Şikâyetin işlenme durumu.</summary>
    public enum ReportStatus : byte
    {
        /// <summary>İncelenmeyi bekliyor.</summary>
        Open = 1,
        /// <summary>İncelendi, işlem yapıldı (içerik kaldırıldı/gizlendi).</summary>
        ActionTaken = 2,
        /// <summary>İncelendi, geçersiz bulundu (reddedildi).</summary>
        Dismissed = 3
    }
}

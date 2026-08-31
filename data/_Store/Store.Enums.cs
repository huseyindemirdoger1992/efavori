using System;

namespace data._Store
{
    // ════════════════════════════════════════════════════════════════════════
    //  efavori — Mağaza Modülü (Store V2)
    //  Enum kataloğu — tinyint (byte) olarak saklanır.
    //  KURAL: Değerlerin ARASINA ekleme yapılmaz; yalnızca SONA eklenir.
    // ════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Mağazanın yaşam döngüsü durumu.
    ///
    /// Eski modelde durum İKİ AYRI boolean ile tutuluyordu
    /// (<c>IsActiveStateAdmin</c> + <c>IsActiveStateVendor</c>). Bu tasarımda dört
    /// kombinasyon oluşuyor ve "başvuru bekliyor", "reddedildi", "askıya alındı",
    /// "satıcı tatilde" durumları birbirinden ayırt edilemiyordu.
    ///
    /// Yeni modelde yönetim kararı bu enum'da, satıcının kendi geçici kapatması ise
    /// ayrı ve dar kapsamlı <c>IsTemporarilyClosed</c> alanındadır — iki farklı
    /// karar mercii, iki farklı alan.
    /// </summary>
    public enum StoreStatus : byte
    {
        /// <summary>Taslak — satıcı bilgileri doldurmaya devam ediyor, başvuru yapılmadı.</summary>
        Draft = 1,

        /// <summary>Başvuru yapıldı, yönetim onayı ve belge incelemesi bekleniyor.</summary>
        PendingApproval = 2,

        /// <summary>Onaylandı ve yayında — ürün satabilir.</summary>
        Active = 3,

        /// <summary>Yönetim tarafından geçici olarak askıya alındı; ürünleri vitrinden kalkar.</summary>
        Suspended = 4,

        /// <summary>Başvuru reddedildi.</summary>
        Rejected = 5,

        /// <summary>Satıcı mağazayı kalıcı olarak kapattı.</summary>
        ClosedByOwner = 6,

        /// <summary>Yönetim tarafından kalıcı olarak kapatıldı.</summary>
        ClosedByAdmin = 7
    }

    /// <summary>
    /// Mağaza doğrulama belgesinin türü.
    ///
    /// Eski modelde 15 ayrı <c>Guid?</c> kolonu vardı (CertificateOfIncorporation,
    /// TaxRegistration, SignatureCircular...). Bu tasarım:
    ///  • yeni bir belge türü eklendiğinde ŞEMA DEĞİŞİKLİĞİ gerektiriyordu,
    ///  • belge başına durum/son kullanma tarihi/red gerekçesi tutulamıyordu,
    ///  • aynı türden ikinci bir belge (yenileme) yüklenemiyordu.
    ///
    /// Bu yüzden 15 kolon, <c>StoreDocuments</c> tablosuna normalize edilmiştir.
    /// </summary>
    public enum StoreDocumentType : byte
    {
        /// <summary>Kuruluş belgesi / ticaret sicil tasdiknamesi.</summary>
        CertificateOfIncorporation = 1,

        /// <summary>Faaliyet belgesi.</summary>
        ActivityCertificate = 2,

        /// <summary>Vergi levhası.</summary>
        TaxRegistration = 3,

        /// <summary>Ticaret sicil gazetesi.</summary>
        TradeRegistryGazette = 4,

        /// <summary>İmza sirküleri.</summary>
        SignatureCircular = 5,

        /// <summary>Yetkili kişi kimlik belgesi.</summary>
        AuthorizedPersonId = 6,

        /// <summary>İşyeri adres kanıtı.</summary>
        ProofOfBusinessAddress = 7,

        /// <summary>Banka hesap dökümü.</summary>
        BankStatement = 8,

        /// <summary>Banka hesap doğrulama yazısı (IBAN teyidi).</summary>
        BankAccountConfirmation = 9,

        /// <summary>Marka tescil belgesi.</summary>
        TrademarkCertificate = 10,

        /// <summary>Yetki/temsil mektubu (distribütörlük).</summary>
        LetterOfAuthorization = 11,

        /// <summary>Kalite/uygunluk sertifikaları (CE, ISO, TSE).</summary>
        QualityCertificate = 12,

        /// <summary>Gümrük kayıt belgesi.</summary>
        CustomsRegistration = 13,

        /// <summary>SGK işyeri tescil belgesi.</summary>
        SocialSecurityRegistration = 14,

        /// <summary>Mülkiyet/kira sözleşmesi.</summary>
        ProofOfOwnership = 15,

        /// <summary>Diğer (Description alanı doldurulur).</summary>
        Other = 100
    }

    /// <summary>
    /// Mağaza sayfasında öne çıkarılan öğenin türü (§30).
    /// </summary>
    public enum StoreFeaturedItemType : byte
    {
        /// <summary>Öne çıkarılan ürün (Products.Id).</summary>
        Product = 1,

        /// <summary>Öne çıkarılan gönderi (Posts.Id).</summary>
        Post = 2,

        /// <summary>Öne çıkarılan kampanya (Campaigns.Id).</summary>
        Campaign = 3,

        /// <summary>Öne çıkarılan kategori (CategoriesProduct.Id).</summary>
        Category = 4,

        /// <summary>Öne çıkarılan makale (Articles.Id).</summary>
        Article = 5
    }
}

using System;

namespace data._Systems
{
    // ════════════════════════════════════════════════════════════════════════
    //  efavori — Denetim Kaydı (Audit V1) — §39
    // ════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Denetim kaydına konu olan işlem türü.
    /// </summary>
    public enum AuditAction : byte
    {
        /// <summary>Kayıt oluşturuldu.</summary>
        Created = 1,

        /// <summary>Kayıt güncellendi.</summary>
        Updated = 2,

        /// <summary>Kayıt soft-delete edildi.</summary>
        SoftDeleted = 3,

        /// <summary>Soft-delete geri alındı.</summary>
        Restored = 4,

        /// <summary>Kayıt fiziksel olarak silindi (yalnızca istisnai durumlar).</summary>
        HardDeleted = 5,

        /// <summary>Durum değişikliği (sipariş, mağaza, moderasyon).</summary>
        StatusChanged = 6,

        /// <summary>Yetki/rol değişikliği.</summary>
        PermissionChanged = 7,

        /// <summary>Hassas veri okundu (belge görüntüleme, kişisel veri erişimi).</summary>
        SensitiveDataAccessed = 8,

        /// <summary>Veri dışa aktarıldı (KVKK veri taşınabilirliği talebi).</summary>
        Exported = 9,

        /// <summary>Kimlik doğrulama olayı (giriş, çıkış, parola değişimi).</summary>
        Authentication = 10,

        /// <summary>Ödeme/finansal işlem.</summary>
        FinancialTransaction = 11,

        /// <summary>Toplu işlem (birden çok kaydı etkileyen yönetim eylemi).</summary>
        BulkOperation = 12
    }

    /// <summary>
    /// DENETİM KAYDI (§39) — kritik varlıklardaki değişikliklerin DEĞİŞMEZ izi.
    ///
    /// KAPSAM — bu tabloya her tablo yazılmaz. Yalnızca aşağıdaki kritik varlıklar
    /// izlenir; aksi hâlde tablo, uygulamanın tüm yazma trafiğini kopyalayan bir
    /// darboğaza dönüşür:
    ///   Users, UserSecurity, Store, StoreDocuments, Products, ProductPrices,
    ///   VariantWarehouseStock, Orders, SubOrders, PaymentTransactions, Refunds,
    ///   SellerLedgerEntries, SellerPayouts, ReturnRequests, Disputes,
    ///   CommissionRates, AccountPermissions.
    ///
    /// TASARIM KARARLARI:
    ///  • SALT-EKLEME (append-only): satır asla güncellenmez ve silinmez. Bu yüzden
    ///    soft delete ve RowVersion alanları YOKTUR (§40 istisnası).
    ///  • <see cref="EntityName"/> + <see cref="EntityId"/> ikilisi bilinçli olarak
    ///    polimorfiktir ve FK TANIMLANMAZ (§42 istisnası): denetim kaydı, izlediği
    ///    kayıt silinse bile ayakta kalmalıdır. FK burada amacın tam tersine hizmet ederdi.
    ///  • Eski/yeni değerler JSON'dur — JSON KULLANIMI BURADA MEŞRUDUR (§46):
    ///    şeması varlıktan varlığa değişen, sorgulanmayan bir anlık görüntüdür.
    ///
    /// GİZLİLİK: <see cref="OldValuesJson"/> ve <see cref="NewValuesJson"/> alanlarına
    /// parola hash'i, MFA gizli anahtarı, ödeme token'ı ve tam kart numarası gibi
    /// sırlar ASLA YAZILMAZ. Denetim yazıcısı bu alanları maskeleyen bir kara liste
    /// uygulamak ZORUNDADIR.
    /// </summary>
    public class AuditLogs
    {
        /// <summary>
        /// Birincil anahtar. Sıralı GUID (sequential/COMB) üretilmesi önerilir —
        /// yüksek hacimli ekleme yapılan tabloda sayfa bölünmesini azaltır.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>İzlenen varlığın tablo/entity adı ("Orders", "Store", "Users").</summary>
        public string EntityName { get; set; } = string.Empty;

        /// <summary>İzlenen kaydın birincil anahtarı (Guid PK'li varlıklar için).</summary>
        public Guid? EntityId { get; set; }

        /// <summary>İzlenen kaydın birincil anahtarı (int PK'li varlıklar için — Locations vb.).</summary>
        public int? EntityIntId { get; set; }

        /// <summary>Uygulanan işlem.</summary>
        public AuditAction Action { get; set; }

        /// <summary>İşlemi yapan kullanıcı (Users.Id). Sistem/arka plan işlerinde null.</summary>
        public Guid? UserId { get; set; }

        /// <summary>İşlemin yapıldığı mağaza bağlamı (Store.Id) — satıcı işlemlerinde.</summary>
        public Guid? StoreId { get; set; }

        /// <summary>
        /// İşlemi yapan sistem bileşeni ("OrderStatusWorker", "PriceSyncJob").
        /// UserId null olduğunda sorumluluğu belirler.
        /// </summary>
        public string? SystemActor { get; set; }

        /// <summary>Değişiklikten ÖNCEKİ değerler (JSON). Created işleminde null.</summary>
        public string? OldValuesJson { get; set; }

        /// <summary>Değişiklikten SONRAKİ değerler (JSON). Delete işleminde null.</summary>
        public string? NewValuesJson { get; set; }

        /// <summary>Değişen alan adları (JSON dizi) — hızlı filtreleme için.</summary>
        public string? ChangedFieldsJson { get; set; }

        /// <summary>İşlemin gerekçesi (yönetim işlemlerinde zorunlu tutulabilir).</summary>
        public string? Reason { get; set; }

        /// <summary>İstek IP adresi.</summary>
        public string? IpAddress { get; set; }

        /// <summary>İstek User-Agent bilgisi.</summary>
        public string? UserAgent { get; set; }

        /// <summary>İşlemin yapıldığı URL yolu.</summary>
        public string? RequestPath { get; set; }

        /// <summary>Dağıtık izleme kimliği — uygulama loglarıyla korelasyon için.</summary>
        public string? TraceId { get; set; }

        /// <summary>
        /// Toplu işlemlerde aynı gruba ait satırları birbirine bağlayan kimlik
        /// ("120 ürünün fiyatı tek işlemde güncellendi").
        /// </summary>
        public Guid? BatchId { get; set; }

        /// <summary>İşlemin gerçekleştiği an (UTC).</summary>
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}

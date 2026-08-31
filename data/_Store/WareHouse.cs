using data.Owned;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace data._Store
{
    /// <summary>
    /// Satıcıya ait depo/warehouse kaydını temsil eder.
    /// Stok, sevkiyat ve mal kabul operasyonlarının yönetimi için kullanılır.
    /// </summary>
    public class WareHouse
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // Hangi kullanıcıya ait
        public Guid? UserId { get; set; }

        // Hangi mağazaya ait
        public Guid? StoreId { get; set; }

        // Depo tanımlayıcıları
        public string? Name { get; set; } = string.Empty;
        public string? WarehouseCode { get; set; } = string.Empty; // Örn: "WH-IST-001"
        public string? Description { get; set; } = string.Empty;

        // Depo tipi: 1=Ana Depo, 2=Dağıtım Merkezi, 3=Mağaza Arkası, 4=Soğuk Hava, 5=Konsinye Depo ... gibi...
        public int? WarehouseType { get; set; } = 1;

        // ==========================================
        // DURUM KONTROLLERİ
        // ==========================================

        // Operasyonel durumlar
        public bool? IsReceiveAllowed { get; set; } = true;   // Mal kabul aktif mi?
        public bool? IsShippingAllowed { get; set; } = true;  // Sevkiyat aktif mi?
        public bool? IsReturnAllowed { get; set; } = true;    // İade kabul aktif mi?

        // ==========================================
        // KAPASİTE ve ÖZELLİKLER
        // ==========================================

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalAreaM2 { get; set; }      // Toplam alan (m²)

        [Column(TypeName = "decimal(18,2)")]
        public decimal? AvailableAreaM2 { get; set; }  // Kullanılabilir alan (m²)

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalVolumeM3 { get; set; }    // Toplam hacim (m³)

        public int? MaxShelfCount { get; set; }        // Maksimum raf sayısı
        public int? MaxPalletCount { get; set; }       // Maksimum palet sayısı

        // Depo özellikleri
        public bool? IsClimateControlled { get; set; } = false;  // İklim kontrollü mü?
        public bool? IsHazardousMaterial { get; set; } = false;  // Tehlikeli madde depolama izni?
        public bool? Is24HourOpen { get; set; } = false;         // 7/24 operasyon?

        // ==========================================
        // OWNED ENTITY'LER
        // ==========================================

        public AddressInfo? AddressInfo { get; set; } = new();
        public WorkingHours? WorkingHours { get; set; } = new();
        public ContactInformation? ContactInformation { get; set; } = new();

        // ==========================================
        // MEDYA ve BELGELER (data._Galleries.Media.Id referansları)
        //
        // §72 KURALI: Fiziksel dosya yolu/URL'i BURADA TUTULMAZ. Eski
        // ProfileCoverGallery owned tipi (ProfileImagePath / CoverImagePath)
        // KALDIRILMIŞTIR; yerine merkezî medya deposuna FK veren iki alan gelmiştir.
        // ==========================================

        public Guid? LogoMediaId { get; set; }              // Depo logosu / görseli
        public Guid? CoverMediaId { get; set; }             // Depo kapak görseli

        public Guid? WarehouseLayoutPlan { get; set; }      // Depo yerleşim planı
        public Guid? FireSafetyCertificate { get; set; }    // Yangın güvenlik belgesi
        public Guid? OperatingLicense { get; set; }         // İşletme izni
        public Guid? InsuranceDocument { get; set; }        // Depo sigorta belgesi
        public Guid? LeaseAgreement { get; set; }           // Kira sözleşmesi / tapu

        // ==========================================
        // TEMEL METADATA
        // ==========================================

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public Meta? Meta { get; set; } = new();
        public IsDeleted? IsDeleted { get; set; } = new();
    }
}

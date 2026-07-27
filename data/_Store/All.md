## Dosya: Store.cs
```csharp
﻿using data.Owned;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security;
namespace data._Store
{
    // <summary>
    // Satıcının (Vendor/Seller) açtığı mağazayı temsil eder.
    // Çoklu satıcılı (Marketplace) yapı için kullanılır.
    // </summary>
    public class Store
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? UserId { get; set; } //
        public string? Name { get; set; } = string.Empty; //
        public string? Description { get; set; } = string.Empty; //

        // Admin Kontrolleri
        public bool? IsActiveStateAdmin { get; set; } = false; //
        public DateTime? IsActiveDateAdmin { get; set; } = new DateTime(); //


        // Satıcı kontrolleri
        public bool? IsActiveStateVendor { get; set; } = true; //
        public DateTime? IsActiveDateVendor { get; set; } = DateTime.Now; //

        public ContactInformation? ContactInformation { get; set; } = new(); 
        public ProfileCoverGallery? ProfileCoverGallery { get; set; } = new();
        public AddressInfo? AddressInfo { get; set; } = new(); 
        public WorkingHours? WorkingHours { get; set; } = new(); 

        // Medya Slotları - Belge GUID Tanımlamaları
        public Guid? CertificateOfIncorporation { get; set; }
        public Guid? ActivityCertificate { get; set; }
        public Guid? TaxRegistration { get; set; }
        public Guid? TradeRegistryGazette { get; set; }
        public Guid? SignatureCircular { get; set; }
        public Guid? AuthorizedPersonId { get; set; }
        public Guid? ProofOfBusinessAddress { get; set; }
        public Guid? BankStatement { get; set; }
        public Guid? BankAccountConfirmation { get; set; }
        public Guid? TrademarkCertificate { get; set; }
        public Guid? LetterOfAuthorization { get; set; }
        public Guid? QualityCertificates { get; set; }
        public Guid? CustomsRegistration { get; set; }
        public Guid? SocialSecurityRegistration { get; set; }
        public Guid? ProofOfOwnership { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public Meta? Meta { get; set; } = new();
        public InteractionCounts? Interaction { get; set; } = new();
        public IsDeleted? IsDeleted { get; set; } = new();
    }
}
```

## Dosya: StoreBlockingInfos.cs
```csharp
﻿using data.Owned;
using System;
using System.Collections.Generic;
using System.Text;

namespace data._Store
{
    public class StoreBlockingInfos
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? UserId { get; set; } // Mağazayı açan kullanıcı (satıcı) ID'si
        public Guid? AdminId { get; set; } // Onay işlemini yürüten yönetici ID'si
        public Guid StoreId { get; set; } // Hangi mağazanın engellendiği/onaylanmadığı ID bilgisi

        public string BlockInfoDescription { get; set; } // Engelleme veya onaylanmama nedeni açıklaması

        public bool IsSucces { get; set; } // Engelleme veya onaylanmama sebebinin çözülüp çözülmediği bilgisi
        public DateTime AtDateTime { get; set; } // Engelleme veya onaylanmama işleminin gerçekleştiği tarih ve saat bilgisi
        public IsDeleted? IsDeleted { get; set; } = new();
    }
}
```

## Dosya: StoreIntegration.cs
```csharp
﻿using data.Owned;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace data._Store
{
    public class StoreIntegration
    {
        [Key]
        public Guid Id { get; set; }
        public Guid StoreId { get; set; }
        public Guid UserId { get; set; }

        // Entegratör adı (Örn: "Parasut", "Logo", "Uyumsoft")
        public string Provider { get; set; }

        // --- Kimlik Doğrulama Bilgileri (Hepsi şifrelenerek saklanmalı) ---

        public string EncryptedApiKey { get; set; }

        public string EncryptedApiSecret { get; set; }

        // Bazı entegratörler (Örn: Logo) için zorunludur
        public string EncryptedUserName { get; set; }

        public string EncryptedPassword { get; set; }

        // Şube kodu veya firma ID gibi özel değerler için
        public string CompanyCode { get; set; }

        // --- Operasyonel Alanlar ---

        public bool IsDefault { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public IsDeleted? IsDeleted { get; set; } = new();

    }
}
```

## Dosya: WareHouse.cs
```csharp
﻿using data.Owned;
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
        public ProfileCoverGallery? ProfileCoverGallery { get; set; } = new();

        // ==========================================
        // MEDYA ve BELGELER (GUID Referansları)
        // ==========================================

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
}```


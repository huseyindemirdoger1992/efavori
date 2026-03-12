using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security;
namespace data
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
        public bool? AdminPermissionPending { get; set; } = true; //
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

        public IsDeleted? IsDeleted { get; set; } = new(); 
    }
}

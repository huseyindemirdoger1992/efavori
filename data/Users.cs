using data._Shared;
using System.ComponentModel.DataAnnotations;

namespace data
{
    public class Users
    {
        // === Temel Kimlik Bilgileri ===
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid(); // Birincil anahtar, otomatik artan benzersiz
        public Guid? WorkstationEmployeeGroupId { get; set; } // Bağlı olduğu İş istasyonu + 
        public bool? UserIsEmployee { get; set; } // Kullanıcı Çalışan Mı + 
        // Şifrele ***-***-***
        public string? UserSponsorEmail { get; set; } // Kullanıcının Sponsor Email Adresi + 
        // Şifrele ***-***-***
        public string? FirstName { get; set; } // Kullanıcının adı + 
        // Şifrele ***-***-***
        public string? LastName { get; set; } // Kullanıcının soyadı + 
        // Şifrele ***-***-***
        public string? Password { get; set; } // Şifrenin hashlenmiş (şifrelenmiş) hali + 

        // === Tarih Bilgileri ===
        public DateTime? RegistrationDate { get; set; } // Sisteme ilk kayıt olduğu tarih ve saat + 
        public bool AccountActivationMailStatu { get; set; } // Hesap aktivasyon durumu +
        public int AccountActivationMailCode { get; set; } // Hesap aktivasyon kodu +
        public int AccountPasswordResetMailCode { get; set; } // Hesap şifre sıfırlama kodu +
        public DateTime? AccountActivationMailDeadline { get; set; } // Hesap aktivasyon geçerlilik süresi +
        public DateTime? DateOfBirth { get; set; } // Doğum Tarihi + 

        // === Kullanıcı Tercihleri ===
        // Şifrele ***-***-***
        public string? Language { get; set; } = "en"; // Arayüz dili tercihi (Varsayılan: İngilizce) (en,tr,az,de,es,fr,hi,pt,ru,zh seçenekleri olabilir) + 
        // Şifrele ***-***-***
        public string? Currency { get; set; } = "USD"; // Kullanıcının tercih ettiği para birimi (USD, EUR, TRY, AZN seçenekleri olabilir) + 
        // Şifrele ***-***-***
        public string? UsersType { get; set; } = "Customer"; // Kullanıcı Tipi (Customer, SuperAdmin seçenekleri olabilir) + 
        // Şifrele ***-***-***

        // === Durum ve Yetki Bilgileri ===
        public bool? IsActive { get; set; } = true; // Kullanıcı hesabı aktif mi dondurulmuş mu?
        public bool? IsActiveVendorStatu { get; set; } = false; // Mağaza açma yetkisi var mı?
        public bool? TermsOfUse { get; set; } // Kullanım koşullarını ve gizlilik sözleşmesini kabul etti mi?
        //public bool? IsDeleted { get; set; } = false; // Veritabanından silmek yerine "silindi" işaretlemek için (Soft Delete)

        // === Profil Bilgileri ===
        // Şifrele ***-***-***
        public string? BackgroundImagePath { get; set; } // Web site arka plan resmi +

        // === İlişkili Tablolar ===
        public ContactInformation? ContactInformation { get; set; }
        public ProfileCoverGallery? ProfileCoverGallery { get; set; }
        public IsPrivateOrPublic? IsPrivateOrPublic { get; set; }
        public UserRolesAccessPermissions? UserRolesAccessPermissions { get; set; }
        public IsDeleted? IsDeleted { get; set; }
    }
}

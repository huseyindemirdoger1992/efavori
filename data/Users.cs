using data._Shared;
using System.ComponentModel.DataAnnotations;

namespace data
{
    public class Users
    {
        // === Temel Kimlik Bilgileri ===
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid(); // Birincil anahtar, otomatik artan benzersiz
        public string FirstName { get; set; } // Kullanıcının adı (Zorunlu, max 50 karakter)
        public string LastName { get; set; } // Kullanıcının soyadı (Zorunlu, max 50 karakter)
        public string Password { get; set; } // Şifrenin hashlenmiş (şifrelenmiş) hali

        // === Tarih Bilgileri ===
        public DateTime RegistrationDate { get; set; } // Sisteme ilk kayıt olduğu tarih ve saat
        public DateTime DateOfBirth { get; set; } // Doğum Tarihi

        // === Kullanıcı Tercihleri ===
        public string Language { get; set; } = "en"; // Arayüz dili tercihi (Varsayılan: İngilizce)
        public string Currency { get; set; } = "USD"; // Kullanıcının tercih ettiği para birimi
        public string? UsersType { get; set; } = "Customer"; // Kullanıcı Tipi
        public string? HeaderMenuType { get; set; } = "Customer"; // Kullanıcı Tipi Header Menüsü

        // === Durum ve Yetki Bilgileri ===
        public bool IsActive { get; set; } = true; // Kullanıcı hesabı aktif mi dondurulmuş mu?
        public bool IsActiveVendorStatu { get; set; } = false; // Mağaza açma yetkisi var mı?
        public bool IsDeleted { get; set; } = false; // Veritabanından silmek yerine "silindi" işaretlemek için (Soft Delete)
        public bool TermsOfUse { get; set; } // Kullanım koşullarını ve gizlilik sözleşmesini kabul etti mi?

        // === Profil Bilgileri ===
        public string? BackgroundImagePath { get; set; } // Profil arka plan resmi

        // === İlişkili Tablolar ===
        public ContactInformation ContactInformation { get; set; }
        public ProfileCoverGallery ProfileCoverGallery { get; set; }
        public AddressInfo AddressInfo { get; set; }
        public IsPrivateOrPublic IsPrivateOrPublic { get; set; }
    }
}

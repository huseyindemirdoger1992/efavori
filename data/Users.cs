using System.ComponentModel.DataAnnotations;

namespace data
{
    public class Users
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid(); // Birincil anahtar, otomatik artan benzersiz

        public string FirstName { get; set; } // Kullanıcının adı (Zorunlu, max 50 karakter) 
        public string LastName { get; set; } // Kullanıcının soyadı (Zorunlu, max 50 karakter) 
        public string Email { get; set; } // İletişim ve giriş için e-posta adresi (Zorunlu) 
        public bool EmailConfirmed { get; set; } // Kullanıcı e-postasını link üzerinden doğruladı mı?
        public string Password { get; set; } // Şifrenin hashlenmiş (şifrelenmiş) hali



























        public string? CountryPhoneCode { get; set; } // Ülke telefon kodu (örn: 90) +
        public string? PhoneNumber { get; set; } // Kargo ve güvenlik doğrulaması için telefon numarası +

        public bool PhoneNumberConfirmed { get; set; } // Telefon numarası SMS ile doğrulandı mı?

        public string? ImagePath { get; set; } // Profil fotoğrafının sunucudaki veya CDN'deki yolu +

        public int CountryId { get; set; } // Kullanıcının bağlı olduğu ülkenin ID'si +

        public string Language { get; set; } = "en"; // Arayüz dili tercihi (Varsayılan: Türkçe) +

        public string Currency { get; set; } = "USD"; // Kullanıcının tercih +

        public int UsersRolesId { get; set; } // Kullanıcının yetki grubu (Admin, User, Vendor vb.) +

        public bool TermsOfUse { get; set; } // Kullanım koşullarını ve gizlilik sözleşmesini kabul etti mi? 

        public bool IsActiveSideBarMenu { get; set; } // Kullanıcı panelinde yan menü açık mı kapalı mı?
        public string? HeaderMenuType { get; set; } = "Customer"; // Kullanıcı Tipi Header Menüsü

        public DateTime RegistrationDate { get; set; } // Sisteme ilk kayıt olduğu tarih ve saat +
        public DateTime DateOfBirth { get; set; } // Doğum Tarihi +


        // Satıcı (Vendor) Bilgileri
        public bool IsActiveVendorStatu { get; set; } = false; // Mağaza açma yetkisi var mı?

        public bool IsActive { get; set; } = true; // Kullanıcı hesabı genel olarak aktif mi dondurulmuş mu?

        public bool IsDeleted { get; set; } = false; // Veritabanından silmek yerine "silindi" işaretlemek için (Soft Delete)

        // Gizlilik/Görünürlük Durumu
        public bool? IsProfilePublic { get; set; } // Diğer Kullanıcılar Benim Profilimi Görüntüleyebilsin mi?

        public bool? IsAllowFriendRequest { get; set; } // Diğer kullanıcılar arkadaş olarak ekleyebilsin mi?

        public bool? IsAllowPhoneNumberView { get; set; } // Diğer kullanıcılar telefon numaramı görebilsin mi?

        public bool? IsAllowEmailView { get; set; } // Diğer kullanıcılar e-posta adresimi görebilsin mi?

        public bool? IsAllowPostsView { get; set; } // Diğer kullanıcılar paylaşımlarımı görebilsin mi?

        public bool? IsAllowComments { get; set; } // Yorum izni

        public bool? IsShowLastSeen { get; set; } // Son görülme

        public bool? IsAllowTagging { get; set; } // Etiketlenme izni

        public bool? IsEmailNotificationEnabled { get; set; } // E-posta bildirimleri

        // Arka Plana Atılmış Resim Yolu
        public string? BackgroundImagePath { get; set; } // Profil arka plan resmi

    }
}

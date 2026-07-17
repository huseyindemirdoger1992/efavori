## Dosya: ChatMessage.cs
```csharp
﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.ConstrainedExecution;
using System.Text;

namespace data._Users
{
    public class ChatMessage
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public Guid SenderId { get; set; } // Gönderen kişinin kullanıcı ID'si
        [Required]
        public Guid ReceiverId { get; set; } // Alıcı kişinin kullanıcı ID'si
        public string Content { get; set; } // Mesaj içeriği (Dosya gönderimi olmadığı için sadece string)
        public DateTime Timestamp { get; set; } = DateTime.UtcNow; // Mesajın gönderilme zamanı
        public bool IsRead { get; set; } = false; // Mesajın okunup okunmadığı bilgisi (WhatsApp'taki mavi tık mantığı)
        public bool IsDelivered { get; set; } = false; // Mesajın alıcıya ulaşıp ulaşmadığı bilgisi (Çift tık mantığı)
        public bool IsDeletedBySender { get; set; } = false; // Opsiyonel: Mesajın silinip silinmediği(tek taraftan)
        public bool IsDeletedBySenderAndReceiver { get; set; } = false; // Opsiyonel: Mesajın silinip silinmediği(Her iki taraftan)
    }
}
```

## Dosya: EmailHistory.cs
```csharp
﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace data._Users
{
    public class EmailHistory
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FromWhom { get; set; }
        public string ToWho { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Attachments { get; set; }
        public string TraceId { get; set; }
        public DateTime SentDate { get; set; } = DateTime.UtcNow;
    }
}
```

## Dosya: LoginTry.cs
```csharp
﻿using System.ComponentModel.DataAnnotations;

namespace data._Users
{
    public class LoginTry
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid? UsersId { get; set; } // Kullanıcı ID'si

        public DateTime? AttemptDate { get; set; } = DateTime.UtcNow; // Deneme tarihi

        public bool? IsSuccessful { get; set; } // true = başarılı, false = başarısız

        public string? IPAddress { get; set; } // IP adresi

        public string? UserAgent { get; set; } // Tarayıcı User-Agent bilgisi

        public string? Platform { get; set; } // Platform bilgisi (örneğin: Web, Mobile)

        public string? Browser { get; set; } // Tarayıcı bilgisi

        // <summary>İşlemin yapıldığı URL yolu.</summary>
        public string? RequestPath { get; set; }
    }
}
```

## Dosya: UserAddress.cs
```csharp
﻿using data.Owned;
using System;
using System.Collections.Generic;
using System.Text;

namespace data._Users
{
    public class UserAddress
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; } // Hangi kullanıcıya ait olduğu
        public string AddressName { get; set; } // Örn: "Ev Adresim", "İş Adresim"
        public string AddressType { get; set; } // Kurumsal veya bireysel adres türü

        public string DeliveryReceiverFirstName { get; set; } // Alıcı adı
        public string DeliveryReceiverLastName { get; set; } // Alıcı soyadı
        public string DeliveryReceiverEmail { get; set; } // Alıcı e-posta adresi
        public string DeliveryReceiverPhoneCountryCode { get; set; } // Alıcı telefon ülke kodu (Örn: +90)
        public string DeliveryReceiverPhoneNumber { get; set; } // Alıcı telefon numarası

        public string Country { get; set; } // Ülke
        public string Cities { get; set; } // Şehir
        public string State { get; set; } // İlçe / Eyalet
        public string FullAddress { get; set; } // Açık adres dökümü

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow; // Kayıt oluşturulma tarihi
        public bool IsDefault { get; set; } // Kullanıcının varsayılan ödeme yöntemi mi?
        public IsDeleted? IsDeleted { get; set; }
    }
}```

## Dosya: UserPayment.cs
```csharp
﻿using data.Owned;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace data._Users
{
    public class UserPayment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; } // Hangi kullanıcıya ait olduğu

        public string CardAlias { get; set; } // Örn: "Maaş Kartım", "Şirket Kartı"
        public string CardHolderName { get; set; } // Kart üzerindeki isim ve soyisim
        public string CardNumber { get; set; } // Kart Numarası
        public string ExpirationDateMonth { get; set; } // Kartın son kullanma tarihi (ay)
        public string ExpirationDateYear { get; set; } // Kartın son kullanma tarihi (yıl)
        public string CardAssociation { get; set; } // Örn: Visa, Mastercard, Troy, Amex
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow; // Kayıt oluşturulma tarihi
        public bool IsDefault { get; set; } // Kullanıcının varsayılan ödeme yöntemi mi?
        public IsDeleted? IsDeleted { get; set; }

    }
}
```

## Dosya: Users.cs
```csharp
﻿using data.Owned;
using System.ComponentModel.DataAnnotations;

namespace data._Users
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
        public string? UsersType { get; set; } = "Customer"; // Kullanıcı Tipi (sadece "Customer", "SuperAdmin" seçenekleri olabilir) + 
        // Şifrele ***-***-***

        // === Durum ve Yetki Bilgileri ===
        public bool? IsActive { get; set; } = true; // Kullanıcı hesabı aktif mi dondurulmuş mu?
        public bool? IsActiveVendorStatu { get; set; } = false; // Mağaza açma yetkisi var mı?
        public bool? TermsOfUse { get; set; } // Kullanım koşullarını ve gizlilik sözleşmesini kabul etti mi?
        //public bool? IsDeleted { get; set; } = false; // Veritabanından silmek yerine "silindi" işaretlemek için (Soft Delete)

        // === Profil Bilgileri ===
        // Şifrele ***-***-***
        public string? BackgroundImagePath { get; set; } // Web site arka plan resmi +
        public int? LogOutTimer { get; set; } // Hareketsizlik Denetimi: Kişisel verilerinizin yetkisiz erişime karşı korunması amacıyla, sistem belirli bir süre etkileşim almadığında oturumunuzu otomatik ve güvenli bir şekilde sonlandırır. (Saniye cinsinden değer alır)

        // === İlişkili Tablolar ===
        public ContactInformation? ContactInformation { get; set; }
        public ProfileCoverGallery? ProfileCoverGallery { get; set; }
        public IsPrivateOrPublic? IsPrivateOrPublic { get; set; }
        public IsDeleted? IsDeleted { get; set; }
    }
}
```

## Dosya: UserShortcuts.cs
```csharp
﻿using data.Owned;
using System.ComponentModel.DataAnnotations;

namespace data._Users
{
    public class UserShortcuts
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; } // Kullanıcı ID'si
        // Şifrele ***-***-***
        public string ShortcutName { get; set; } // Kısayolun adı
        // Şifrele ***-***-***
        public string ShortcutUrl { get; set; } // Kısayolun URL'si
        // Şifrele ***-***-***
        public string ShortcutIcon { get; set; } // Kısayolun URL'si
        public IsDeleted? IsDeleted { get; set; }
    }
}
```


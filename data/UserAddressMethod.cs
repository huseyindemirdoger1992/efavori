using data._Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace data
{
    public class UserAddressMethod
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
}
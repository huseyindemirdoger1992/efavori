using data._Shared;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace data
{
    public class UserPaymentMethod
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

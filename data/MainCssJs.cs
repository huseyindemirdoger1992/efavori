using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace data
{
    public class MainCssJs
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid(); // Benzersiz kimlik oluşturmak için Guid kullanılır
        public string? Title { get; set; } // Başlık alanı
        public string? Description { get; set; } // Açıklama alanı
        public string? UserCodes { get; set; } // Kullanıcı kodları alanı
        public bool? IsActive { get; set; } // Aktiflik durumu alanı
        public DateTime? GetDateTime { get; set; } // Oluşturulma tarihi alanı
        public bool? IsCssOrJs { get; set; } // CSS veya JS olduğunu belirten alan (true: CSS, false: JS) select option yapısında kullanılabilir Css ise true, Js ise false
        public bool? IsDelete { get; set; } // Silinmişlik durumu alanı
    }
}

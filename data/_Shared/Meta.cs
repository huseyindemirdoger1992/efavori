using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace data._Shared
{
    [Owned]
    public class Meta
    {

        [MaxLength(70)]
        public string MetaTitle { get; set; } // Google'da görünecek başlık

        [MaxLength(160)]
        public string MetaDescription { get; set; } // Arama sonuçlarındaki açıklama

        public string FocusKeywords { get; set; } // Virgülle ayrılmış anahtar kelimeler

        public string CanonicalUrl { get; set; } // Yinelenen içerik engelleme linki

        public string OgType { get; set; } = "article"; // Open Graph tipi (Facebook/Twitter için)

        // Robot talimatları: "index, follow" vb.
        public string RobotsIndex { get; set; } = "index, follow";
    }
}

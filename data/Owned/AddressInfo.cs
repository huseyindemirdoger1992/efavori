using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace data.Owned
{
    [Owned]
    public class AddressInfo
    {
        public string? MapTitle { get; set; } // Örn: "Evim", "Merkez Ofis", "Depo", "Mağaza" gibi harita üzerinde gösterilecek başlık

        public string? Country { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public string? ZipCode { get; set; }
        public string? Address { get; set; }

        [Column(TypeName = "decimal(18,15)")]
        public decimal? Latitude { get; set; }

        [Column(TypeName = "decimal(18,15)")]
        public decimal? Longitude { get; set; }
        public string? GoogleMyBusinessAccountLink { get; set; } // Google Benim İşletmem Hesabı Linki 
    }
}

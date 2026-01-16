using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace data._Shared
{
    [Owned]
    public class AddressInfo
    {
        public string? Country { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public string? ZipCode { get; set; }
        public string? Address { get; set; }

        // Google Harita Konum Bilgisi
        public string? MapTitle { get; set; } // Örn: "Evim", "Merkez Ofis"
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? GoogleMyBusinessAccountLink { get; set; } // Google Benim İşletmem Hesabı Linki 
    }
}
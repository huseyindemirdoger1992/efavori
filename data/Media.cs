using System.ComponentModel.DataAnnotations;

namespace data
{
    public class Media
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string DosyaAdi { get; set; }
        public string DosyaURL { get; set; }
        public string DosyaUzantisi { get; set; }
        public string OrjDosyaBoyutu { get; set; }
        public string SonrakiDosyaBoyutu { get; set; }
        public string DosyaLokal { get; set; }
        public DateTime DosyaYuklenmeTarihi { get; set; }
        public bool? IsDeleted { get; set; }
        public string? MediaType { get; set; } // Images, Videos, Documents, Other

    }
}

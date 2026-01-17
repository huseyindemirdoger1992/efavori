using System.ComponentModel.DataAnnotations;

namespace data
{
    public class Languages
    {
        [Key]
        public int Id { get; set; }

        public string? NativeName { get; set; }    
        public string? Iso2 { get; set; }    
        public string? Iso3 { get; set; }    

    }
}

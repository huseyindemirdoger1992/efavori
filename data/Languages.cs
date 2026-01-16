using System.ComponentModel.DataAnnotations;

namespace data
{
    public class Languages
    {
        [Key]
        public int Id { get; set; }

        public string? Name { get; set; }    
        public string? InterFaceName { get; set; }    

    }
}

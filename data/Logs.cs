using System.ComponentModel.DataAnnotations;

namespace data
{
    public class Logs
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Exception { get; set; }
    }
}

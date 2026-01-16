using System.ComponentModel.DataAnnotations;

namespace data
{
    public class UsersRoles
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } // Örn: Customer, Seller, Admin
    }
}

using System.ComponentModel.DataAnnotations;

namespace data
{
    public class UsersRoles
    {
        [Key]
        public Guid Id { get; set; }

        // Şifrele ***-***-***
        public string Name { get; set; } // Örn: Customer, Seller, Admin
    }
}

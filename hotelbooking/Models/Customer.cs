using System.ComponentModel.DataAnnotations;

namespace hotelbooking.Models
{
    public class Customer
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string FirstName { get; set; }
        
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; }
    }
}

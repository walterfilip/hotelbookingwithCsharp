using System.ComponentModel.DataAnnotations;

namespace hotelbooking.Models
{
    public class Customer
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;
    }
}

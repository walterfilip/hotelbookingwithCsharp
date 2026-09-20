using System.ComponentModel.DataAnnotations;

namespace hotelbooking.Models
{
    public class Customer
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string Lastname { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}

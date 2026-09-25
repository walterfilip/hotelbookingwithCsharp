namespace hotelbooking.Models
{
    public class Booking
    {
        public int Id { get; set; } 

        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public int RoomId { get; set; }
        public Room? Room { get; set; } 
        public DateOnly CheckIn { get; set; }
        public DateOnly CheckOut { get; set; }

        public decimal TotalPrice { get; set; }
    }
}

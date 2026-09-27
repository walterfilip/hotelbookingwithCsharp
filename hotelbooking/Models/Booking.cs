namespace hotelbooking.Models
{

    public enum BookingStatus
    {
        Active,
        Completed,
        Cancelled
    }   
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
        public BookingStatus Status { get; set; } = BookingStatus.Active;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? CancelledAt { get; set; } 
    }
}

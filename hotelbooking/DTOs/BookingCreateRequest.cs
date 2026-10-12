namespace hotelbooking.DTOs
{
    public class BookingCreateRequest
    {
        public int CustomerId { get; set; }
        public int RoomId { get; set; }
        public DateOnly CheckIn { get; set; }
        public DateOnly CheckOut { get; set; }

    }
}

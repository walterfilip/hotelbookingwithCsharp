namespace hotelbooking.DTOs
{
    public class BookingChangeDateRequest
    {
        public DateOnly CheckIn { get; set; }
        public DateOnly CheckOut { get; set; }
    }
}

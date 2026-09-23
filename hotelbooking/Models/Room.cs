namespace hotelbooking.Models
{

    public enum RoomType
    {
        Single,
        Double
     
    }
    public class Room
    {
        public int Id { get; set; }

        public int RoomNumber { get; set; }

        public RoomType Type { get; set; }

        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}

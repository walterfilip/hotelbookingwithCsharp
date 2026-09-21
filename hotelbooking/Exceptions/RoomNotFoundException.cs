namespace hotelbooking.Exceptions
{
    public class RoomNotFoundException :Exception
    {
        public RoomNotFoundException(string message) : base(message)
        {
        }
    }
}

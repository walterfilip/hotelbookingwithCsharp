namespace hotelbooking.Exceptions
{
    public class RoomAlreadyBookedException : Exception
    {
        public RoomAlreadyBookedException(string message) : base(message)
        {
        }
    
    }
}

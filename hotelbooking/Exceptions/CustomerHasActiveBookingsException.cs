namespace hotelbooking.Exceptions
{
    public class CustomerHasActiveBookingsException :Exception
    {
        public CustomerHasActiveBookingsException(string message) : base(message)
        {
        }
    }
}

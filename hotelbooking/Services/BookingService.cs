using hotelbooking.Data;
using hotelbooking.Models;
using hotelbooking.Exceptions;

namespace hotelbooking.Services
{
    public class BookingService
    {
        private readonly AppDbContext _context;

        public BookingService(AppDbContext context)
        {
            _context = context;
        }

       public Booking CreateBooking(Booking booking)
        {
            Customer? customer = _context.Customers
                .FirstOrDefault(c => c.Id == booking.CustomerId);

            if(customer == null)
            {
                throw new CustomerNotFoundException($"Kund med id {booking.CustomerId} finns ej");
            }
            Room? room = _context.Rooms
                .FirstOrDefault(r => r.Id == booking.RoomId);

            if (room == null)
            {
                throw new RoomNotFoundException($"Rum med id {booking.RoomId} finns ej");
            }
            if (!room.IsActive)
            {
                throw new InvalidRoomException($"Rum är inte aktivt");
            }

            return booking;
        }
    }
}

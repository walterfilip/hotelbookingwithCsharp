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
        public List<Booking> GetAllBookings()
        {
            return _context.Bookings.ToList();
        }

        public Booking? GetBookingById(int id)
        {
            var booking = _context.Bookings.FirstOrDefault(b => b.Id == id);

            if (booking == null)
            {
                throw new BookingNotFoundException($"Bokning med id {id} finns ej");
            }

            return booking;
        }

        public Booking CreateBooking(Booking booking)
        {
            Customer? customer = _context.Customers
                .FirstOrDefault(c => c.Id == booking.CustomerId);

            if (customer == null)
            {
                throw new CustomerNotFoundException($"Kund med id {booking.CustomerId} finns ej");
            }

            Room? room = _context.Rooms
                .FirstOrDefault(r => r.Id == booking.RoomId);

            room = ValidateRoom(room, booking);
            ValidateDates(booking);
            ValidateRoomAvailability(booking);

            int numberOfNights = booking.CheckOut.DayNumber - booking.CheckIn.DayNumber;
            booking.TotalPrice = numberOfNights * room.Price;

            _context.Bookings.Add(booking);
            _context.SaveChanges();

            return booking;
        }

        private Room ValidateRoom(Room? room, Booking booking)
        {
            if (room == null)
            {
                throw new RoomNotFoundException($"Rum med id {booking.RoomId} finns ej");
            }
            if (!room.IsActive)
            {
                throw new InvalidRoomException($"Rum är inte aktivt");
            }
            return room;
        }

        private void ValidateDates(Booking booking)
        {
            if (booking.CheckOut <= booking.CheckIn)
            {
                throw new InvalidBookingException("Check-out datum måste vara efter check-in datum");
            }
            if (booking.CheckIn < DateOnly.FromDateTime(DateTime.Now))
            {
                throw new InvalidBookingException("Check-in datum kan inte vara i det förflutna");
            }
        }
        private void ValidateRoomAvailability(Booking booking)
        {
            bool roomIsBooked = _context.Bookings.Any(
                b => b.RoomId == booking.RoomId &&
                b.CheckIn < booking.CheckOut &&
                b.CheckOut > booking.CheckIn);
            if(roomIsBooked)
            {
                throw new RoomAlreadyBookedException($"Rum med id {booking.RoomId} är redan bokat för de valda datumen");
            }
         
        }
    }
}

using hotelbooking.Data;
using hotelbooking.Models;
using hotelbooking.Exceptions;
using hotelbooking.DTOs;
using Microsoft.EntityFrameworkCore;


namespace hotelbooking.Services
{
    public class BookingService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<BookingService> _logger;

        public BookingService(AppDbContext context, ILogger<BookingService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public List<Booking> GetAllBookings()
        {
            return _context.Bookings.Include(b => b.Room).ToList();
            
        }

        public Booking GetBookingById(int id)
        {
            var booking = _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefault(b => b.Id == id);

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

            booking.CustomerFirstNameSnapshot = customer.FirstName;
            booking.CustomerLastNameSnapshot = customer.LastName;
            booking.CustomerEmailSnapshot = customer.Email;

            booking.TotalPrice = CalculateTotalPrice(booking.RoomId, booking.CheckIn, booking.CheckOut);

            _context.Bookings.Add(booking);
            _context.SaveChanges();

            return booking;
        }
        public Booking CancelBooking(int id)
        {
            Booking? booking = _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefault(b => b.Id == id);

            if (booking == null) 
            {
                throw new BookingNotFoundException($"Bokning med id {id} finns ej");
            }
            if(booking.Status == BookingStatus.Cancelled)
            {
                throw new InvalidBookingException($"Bokning med id {id} är redan avbokad");
            }
            if (booking.Status == BookingStatus.Completed)
            {
                throw new InvalidBookingException($"Bokning med id {id} är redan slutförd och kan inte avbokas");
            }
            if (BookingHasStarted(booking))
            {
                throw new InvalidBookingException($"Bokning med id {id} har redan börjat och kan inte avbokas");
            }
          
            booking.Status = BookingStatus.Cancelled;
            booking.CancelledAt = DateTimeOffset.UtcNow;

            _context.SaveChanges();

            return booking;
        }
        public Booking UpdateBooking(int id, BookingChangeDateRequest updatedBooking)
        {
            var existingBooking = _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefault(b => b.Id == id);

            if (existingBooking == null)
            {
                throw new BookingNotFoundException($"Bokning med id {id} finns ej");
            }
            if(existingBooking.Status == BookingStatus.Cancelled)
            {
                throw new InvalidBookingException($"Bokning med id {id} är avbokad och kan inte ändras");
            }
            if(existingBooking.Status == BookingStatus.Completed)
            {
                throw new InvalidBookingException($"Bokning med id {id} är slutförd och kan inte ändras");
            }
            if (BookingHasStarted(existingBooking))
            {
                throw new InvalidBookingException($"Bokning med id {existingBooking.Id} har redan börjat och kan inte ändras");
            }

            var bookingToValidate = new Booking
            {
                Id = existingBooking.Id,
                RoomId = existingBooking.RoomId,
                CheckIn = updatedBooking.CheckIn,
                CheckOut = updatedBooking.CheckOut,
                Status = existingBooking.Status
            };


            ValidateDates(bookingToValidate);
            ValidateRoomAvailabilityIgnoreCurrentBooking(bookingToValidate);


            existingBooking.CheckIn = updatedBooking.CheckIn;
            existingBooking.CheckOut = updatedBooking.CheckOut;
            existingBooking.TotalPrice = CalculateTotalPrice(existingBooking.RoomId,
                existingBooking.CheckIn, existingBooking.CheckOut);

            _context.SaveChanges();

            return existingBooking;
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
                b.Status == BookingStatus.Active &&
                b.CheckIn < booking.CheckOut &&
                b.CheckOut > booking.CheckIn);

            if(roomIsBooked)
            {
                throw new RoomAlreadyBookedException($"Rum med id {booking.RoomId} är redan bokat för de valda datumen");
            }
         
        }
        private void ValidateRoomAvailabilityIgnoreCurrentBooking(Booking booking)
        {
            bool roomIsBooked = _context.Bookings.Any(
                b => b.Id != booking.Id &&
                b.Status == BookingStatus.Active &&
                b.RoomId == booking.RoomId &&
                b.CheckIn < booking.CheckOut &&
                b.CheckOut > booking.CheckIn);

            if (roomIsBooked)
            {
                throw new RoomAlreadyBookedException($"Rum med id {booking.RoomId} är redan bokat för de valda datumen");
            }

        }
        private bool BookingHasStarted(Booking booking) 
        {
            return booking.CheckIn <= DateOnly.FromDateTime(DateTime.Now);        
        }

        public void UpdateCompletedBookings()
        {
            var bookings = _context.Bookings
                .Where(b => b.Status == BookingStatus.Active)
                .ToList();

            foreach (var booking in bookings)
            {
                if (booking.CheckOut < DateOnly.FromDateTime(DateTime.Now))
                {
                    booking.Status = BookingStatus.Completed;
                    _logger.LogInformation("Bokning med id {Id} har markerats som slutförd", booking.Id);

                }
              
            }
            _context.SaveChanges();
        }

        private decimal CalculateTotalPrice(int roomId, DateOnly checkIn, DateOnly checkOut)
        {
            Room? room = _context.Rooms.FirstOrDefault(r => r.Id == roomId);

            if (room == null)
            {
                throw new RoomNotFoundException($"Rum med id {roomId} finns ej");
            }

            int numberOfNights = checkOut.DayNumber - checkIn.DayNumber;

            return numberOfNights * room.Price;        
        }
                
    }
}

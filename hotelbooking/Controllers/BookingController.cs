using hotelbooking.Services;
using hotelbooking.Models;
using Microsoft.AspNetCore.Mvc;

namespace hotelbooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly BookingService _bookingService;


        public BookingController(BookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        public ActionResult<List<Booking>> GetAllBookings()
        {
            var bookings = _bookingService.GetAllBookings();

            return Ok(bookings);
        }

        [HttpGet("{id}")]
        public ActionResult<Booking> GetBooking(int id)
        {
            var booking = _bookingService.GetBookingById(id);
          
            return Ok(booking);
        }

        [HttpPost]
        public ActionResult<Booking> CreateBooking(Booking booking)
        {
            var newBooking = _bookingService.CreateBooking(booking);

            return CreatedAtAction(nameof(GetBooking), new { id = newBooking.Id }, newBooking);
        }


    }
}

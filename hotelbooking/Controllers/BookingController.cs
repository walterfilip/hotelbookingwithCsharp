using hotelbooking.Services;
using hotelbooking.Models;
using hotelbooking.DTOs;
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
        public ActionResult<List<BookingResponseDto>> GetAllBookings()
        {
            var bookings = _bookingService.GetAllBookings();
            var dtos = bookings.Select(ToDto).ToList();

            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public ActionResult<BookingResponseDto> GetBooking(int id)
        {
            var booking = _bookingService.GetBookingById(id);

            return Ok(ToDto(booking));
        }

        [HttpPost]
        public ActionResult<BookingResponseDto> CreateBooking(Booking booking)
        {
            var newBooking = _bookingService.CreateBooking(booking);
            var dto = ToDto(newBooking);

            return CreatedAtAction(nameof(GetBooking), new { id = dto.Id }, dto);
        }
        [HttpPut("{id}/cancel")]
        public ActionResult<BookingResponseDto> CancelBooking(int id)
        {
            var cancelledBooking = _bookingService.CancelBooking(id);

            return Ok(ToDto(cancelledBooking));
        }
        [HttpPut("{id}")]
        public ActionResult<BookingResponseDto> UpdateBooking(int id, BookingChangeDateRequest updateBooking)
        {
            var booking = _bookingService.UpdateBooking(id, updateBooking);

            return Ok(ToDto(booking));
        }
        private BookingResponseDto ToDto(Booking booking)
        {
            return new BookingResponseDto
            {
                Id = booking.Id,
                CustomerId = booking.CustomerId,
                CustomerFirstNameSnapshot = booking.CustomerFirstNameSnapshot,
                CustomerLastNameSnapshot = booking.CustomerLastNameSnapshot,
                CustomerEmailSnapshot = booking.CustomerEmailSnapshot,
                RoomId = booking.RoomId,
                RoomNumber = booking.Room?.RoomNumber ?? 0,
                RoomType = booking.Room?.Type.ToString() ?? string.Empty,
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                TotalPrice = booking.TotalPrice,
                Status = booking.Status,
                CreatedAt = booking.CreatedAt,
                CancelledAt = booking.CancelledAt
            };
        }
    }
}

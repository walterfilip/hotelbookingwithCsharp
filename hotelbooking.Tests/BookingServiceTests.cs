using hotelbooking.Data;
using hotelbooking.Models;
using hotelbooking.Services;
using hotelbooking.Exceptions;

using hotelbooking.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;

namespace hotelbooking.Tests
{
    public class BookingServiceTests
    {

        private readonly AppDbContext _context;
        private readonly BookingService _service;

        public BookingServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

            _context = new AppDbContext(options);
            var logger = NullLogger<BookingService>.Instance;
            _service = new BookingService(_context, logger);
        }

        [Fact]
        public void CreateBookingShouldReturnBookingWhenCustomerExists()
        {
            var customer = new Customer
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };
            var room = new Room
            {
                RoomNumber = 101,
                Type = RoomType.Single,
                Price = 1000,
                Description = "Test rum"
            };

            _context.Rooms.Add(room);
            _context.Customers.Add(customer);
            _context.SaveChanges();

            var booking = new Booking
            {
                CustomerId = customer.Id,
                RoomId = room.Id,
                CheckIn = DateOnly.FromDateTime(DateTime.Now),
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            };

            var result = _service.CreateBooking(booking);

            Assert.Equal(booking, result);
            Assert.NotNull(result);

        }
        [Fact]
        public void CreateBookingShouldThrowExceptionWhenCustomerDoesNotExist()
        {
            var room = new Room
            {
                RoomNumber = 101,
                Type = RoomType.Single,
                Price = 1000,
                Description = "Test rum"
            };
            _context.Rooms.Add(room);
            _context.SaveChanges();

            var booking = new Booking
            {
                CustomerId = 999,
                RoomId = room.Id
            };
            var exception = Assert.Throws<CustomerNotFoundException>(
                () => _service.CreateBooking(booking));

            Assert.Equal($"Kund med id {booking.CustomerId} finns ej", exception.Message);
        }

        [Fact]
        public void CreateBookingShouldThrowExceptionWhenRoomDoesNotExist()
        {
            var customer = new Customer
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };
            _context.Customers.Add(customer);
            _context.SaveChanges();

            var booking = new Booking
            {
                CustomerId = customer.Id,
                RoomId = 999
            };
            var exception = Assert.Throws<RoomNotFoundException>(
                () => _service.CreateBooking(booking));

            Assert.Equal($"Rum med id {booking.RoomId} finns ej", exception.Message);
        }
        [Fact]
        public void CreateBookingShouldThrowExceptionWhenRoomIsNotActive()
        {
            var customer = new Customer
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };
            var room = new Room
            {
                RoomNumber = 101,
                Type = RoomType.Single,
                Price = 1000,
                Description = "Test rum",
                IsActive = false
            };
            _context.Customers.Add(customer);
            _context.Rooms.Add(room);
            _context.SaveChanges();

            var booking = new Booking
            {
                CustomerId = customer.Id,
                RoomId = room.Id
            };

            var exception = Assert.Throws<InvalidRoomException>(
                () => _service.CreateBooking(booking));

            Assert.Equal($"Rum är inte aktivt", exception.Message);
        }
        [Fact]
        public void CreateBookingShouldThrowExceptionWhenCheckOutIsBeforeCheckIn()
        {
            var customer = new Customer
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };

            var room = new Room
            {
                RoomNumber = 101,
                Type = RoomType.Single,
                Price = 1000,
                Description = "Test rum",
                IsActive = true
            };

            _context.Customers.Add(customer);
            _context.Rooms.Add(room);
            _context.SaveChanges();

            var booking = new Booking
            {
                CustomerId = customer.Id,
                RoomId = room.Id,
                CheckIn = new DateOnly(2024, 6, 1),
                CheckOut = new DateOnly(2024, 5, 1)
            };

            var exception = Assert.Throws<InvalidBookingException>(
                () => _service.CreateBooking(booking));

            Assert.Equal("Check-out datum måste vara efter check-in datum", exception.Message);
        }
        [Fact]
        public void CreateBookingShouldThrowExceptionWhenCheckInIsInThePast()
        {
            var customer = new Customer
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };

            var room = new Room
            {
                RoomNumber = 101,
                Type = RoomType.Single,
                Price = 1000,
                Description = "Test rum",
                IsActive = true
            };

            _context.Customers.Add(customer);
            _context.Rooms.Add(room);
            _context.SaveChanges();

            var booking = new Booking
            {
                CustomerId = customer.Id,
                RoomId = room.Id,
                CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
            };

            var exception = Assert.Throws<InvalidBookingException>(
                () => _service.CreateBooking(booking));

            Assert.Equal("Check-in datum kan inte vara i det förflutna", exception.Message);
        }
        [Fact]
        public void CreateBookingShouldCalculateTotalPriceCorrectly()
        {
            var customer = new Customer
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };

            var room = new Room
            {
                RoomNumber = 101,
                Type = RoomType.Single,
                Price = 1000,
                Description = "Test rum",
                IsActive = true
            };

            _context.Customers.Add(customer);
            _context.Rooms.Add(room);
            _context.SaveChanges();

            var booking = new Booking
            {
                CustomerId = customer.Id,
                RoomId = room.Id,
                CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(3))
            };

            var result = _service.CreateBooking(booking);


            Assert.Equal(2000m, result.TotalPrice);
        }
        [Fact]
        public void CreateBookingShouldThrowExceptionWhenRoomIsNull()
        {
            var customer = new Customer
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };

            _context.Customers.Add(customer);
            _context.SaveChanges();

            var booking = new Booking
            {
                CustomerId = customer.Id,
                RoomId = 999,
                CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(3))
            };

            var exception = Assert.Throws<RoomNotFoundException>(
                () => _service.CreateBooking(booking));

            Assert.Equal($"Rum med id {booking.RoomId} finns ej", exception.Message);
        }
        [Fact]
        public void CreateBookingShouldSaveBookingToDatabase()
        {
            var customer = new Customer
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };

            _context.Customers.Add(customer);
            _context.SaveChanges();

            var room = new Room
            {
                RoomNumber = 101,
                Type = RoomType.Single,
                Price = 1000,
                Description = "Test rum",
                IsActive = true
            };

            _context.Rooms.Add(room);
            _context.SaveChanges();

            var booking = new Booking
            {
                CustomerId = customer.Id,
                RoomId = room.Id,
                CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(3))
            };

            var result = _service.CreateBooking(booking);
            var savedBooking = _context.Bookings.FirstOrDefault(b => b.Id == result.Id);

            Assert.NotNull(result);
            Assert.NotNull(savedBooking);
            Assert.Equal(result.Id, savedBooking.Id);
            Assert.Equal(result.CustomerId, savedBooking.CustomerId);
            Assert.Equal(result.RoomId, savedBooking.RoomId);
            Assert.Equal(result.CheckIn, savedBooking.CheckIn);
            Assert.Equal(result.CheckOut, savedBooking.CheckOut);
            Assert.Equal(result.TotalPrice, savedBooking.TotalPrice);
        }

        [Fact]
        public void CreateBookingShouldThrowExceptionWhenRoomIsAlreadyBooked()
        {
            var customer = new Customer
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };

            var room = new Room
            {
                RoomNumber = 101,
                Type = RoomType.Single,
                Price = 1000,
                Description = "Test rum",
                IsActive = true
            };
            _context.Customers.Add(customer);
            _context.Rooms.Add(room);
            _context.SaveChanges();

            var existingBooking = new Booking
            {
                CustomerId = customer.Id,
                RoomId = room.Id,
                CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(3))
            };
            _context.Bookings.Add(existingBooking);
            _context.SaveChanges();

            var newBooking = new Booking
            {
                CustomerId = customer.Id,
                RoomId = room.Id,
                CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(2)), // Overlaps with existing booking
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(4))
            };

            var exception = Assert.Throws<RoomAlreadyBookedException>(
                () => _service.CreateBooking(newBooking));

            Assert.Equal($"Rum med id {newBooking.RoomId} är redan bokat för de valda datumen", exception.Message);
        }
        [Fact]
        public void CreateBookingShouldBeAbleToBookRoomWithoutOverlap()
        {
            var customer = new Customer
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };
            _context.Customers.Add(customer);
            _context.SaveChanges();

            var room = new Room
            {
                RoomNumber = 101,
                Type = RoomType.Single,
                Price = 1000,
                Description = "Test rum",
                IsActive = true
            };
            var firstBooking = new Booking
            {
                CustomerId = customer.Id,
                RoomId = room.Id,
                CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(3))
            };
            _context.Rooms.Add(room);
            _context.Bookings.Add(firstBooking);
            _context.SaveChanges();

            var secondBooking = new Booking
            {
                CustomerId = customer.Id,
                RoomId = room.Id,
                CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(3)), // Starts after the first booking ends
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(5))
            };

            var result = _service.CreateBooking(secondBooking);
            Assert.NotNull(result);

            var savedBooking = _context.Bookings.FirstOrDefault(b => b.Id == result.Id);
            Assert.NotNull(savedBooking);

            Assert.Equal(secondBooking.CustomerId, savedBooking.CustomerId);
            Assert.Equal(secondBooking.RoomId, savedBooking.RoomId);
            Assert.Equal(secondBooking.CheckIn, savedBooking.CheckIn);
            Assert.Equal(secondBooking.CheckOut, savedBooking.CheckOut);
        }
        [Fact]
        public void CreateBookingShouldThrowExceptionWhenRoomBookingOverlaps()
        {
            var customer = new Customer
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };
            _context.Customers.Add(customer);
            _context.SaveChanges();

            var room = new Room
            {
                RoomNumber = 101,
                Type = RoomType.Single,
                Price = 1000,
                Description = "Test rum",
                IsActive = true
            };
            _context.Rooms.Add(room);
            _context.SaveChanges();

            var firstBooking = new Booking
            {
                CustomerId = customer.Id,
                RoomId = room.Id,
                CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(3))
            };
            _context.Bookings.Add(firstBooking);
            _context.SaveChanges();

            var newBooking = new Booking
            {
                CustomerId = customer.Id,
                RoomId = room.Id,
                CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(2)), // Overlaps with existing booking
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(4))
            };

            var exception = Assert.Throws<RoomAlreadyBookedException>(
                () => _service.CreateBooking(newBooking));

            Assert.Equal($"Rum med id {newBooking.RoomId} är redan bokat för de valda datumen", exception.Message);
        }

        [Fact]
        public void CancelBookingShouldSetStatusToCancelled()
        {
            var room = new Room
            {
                RoomNumber = 101,
                Type = RoomType.Single,
                Price = 1000,
                Description = "Test rum",
                IsActive = true
            };
            _context.Rooms.Add(room);
            _context.SaveChanges();
            var booking = new Booking
            {
                CustomerId = 1,
                RoomId = room.Id,
                CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
                TotalPrice = 1500,
                Status = BookingStatus.Active
            };
            _context.Bookings.Add(booking);
            _context.SaveChanges();

            var cancelledBooking = _service.CancelBooking(booking.Id);
            Assert.Equal(BookingStatus.Cancelled, cancelledBooking.Status);

        }
        [Fact]
        public void CancelBookingShouldThrowExceptionWhenBookingDoesNotExist()
        {
            var exception = Assert.Throws<BookingNotFoundException>(
                () => _service.CancelBooking(999));

            Assert.Equal($"Bokning med id 999 finns ej", exception.Message);
        }
        [Fact]
        public void CancelBookingShouldSetCancelledAtToCurrentTime()
        {
            var room = new Room
            {
                RoomNumber = 101,
                Type = RoomType.Single,
                Price = 1000,
                Description = "Test rum",
                IsActive = true
            };
            _context.Rooms.Add(room);
            _context.SaveChanges();

            var booking = new Booking
            {
                CustomerId = 1,
                RoomId = room.Id,
                CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
                TotalPrice = 1500,
                Status = BookingStatus.Active
            };

            _context.Bookings.Add(booking);
            _context.SaveChanges();

            var cancelledBooking = _service.CancelBooking(booking.Id);

            Assert.NotNull(cancelledBooking.CancelledAt);
            Assert.True(cancelledBooking.CancelledAt <= DateTimeOffset.UtcNow);
            Assert.Equal(BookingStatus.Cancelled, cancelledBooking.Status);
        }
        [Fact]
        public void CancelBookingShouldSaveChangesToDatabase()
        {
            var room = new Room
            {
                RoomNumber = 101,
                Type = RoomType.Single,
                Price = 1000,
                Description = "Test rum",
                IsActive = true
            };
            _context.Rooms.Add(room);
            _context.SaveChanges();

            var booking = new Booking
            {
                CustomerId = 1,
                RoomId = room.Id,
                CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
                TotalPrice = 1500,
                Status = BookingStatus.Active
            };
            _context.Bookings.Add(booking);
            _context.SaveChanges();

            var cancelledBooking = _service.CancelBooking(booking.Id);
            var savedBooking = _context.Bookings.FirstOrDefault(b => b.Id == booking.Id);

            Assert.NotNull(savedBooking);
            Assert.Equal(BookingStatus.Cancelled, savedBooking.Status);
            Assert.NotNull(savedBooking.CancelledAt);
        }
        [Fact]
        public void CancelBookingShouldThrowExceptionWhenBookingIsAlreadyCancelled()
        {
            var room = new Room
            {
                RoomNumber = 101,
                Type = RoomType.Single,
                Price = 1000,
                Description = "Test rum",
                IsActive = true
            };
            _context.Rooms.Add(room);
            _context.SaveChanges();

            var booking = new Booking
            {
                CustomerId = 1,
                RoomId = room.Id,
                CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
                TotalPrice = 1500,
                Status = BookingStatus.Cancelled,
                CancelledAt = DateTimeOffset.UtcNow
            };
            _context.Bookings.Add(booking);
            _context.SaveChanges();

            var exception = Assert.Throws<InvalidBookingException>(
                () => _service.CancelBooking(booking.Id));

            Assert.Equal($"Bokning med id {booking.Id} är redan avbokad", exception.Message);
        }
        [Fact]
        public void CancelBookingShouldThorwExceptionWhenBookingIsAlreadyCompleted()
        {
            var room = new Room
            {
                RoomNumber = 101,
                Type = RoomType.Single,
                Price = 1000,
                Description = "Test rum",
                IsActive = true
            };
            _context.Rooms.Add(room);
            _context.SaveChanges();

            var booking = new Booking
            {
                CustomerId = 1,
                RoomId = room.Id,
                CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(-3)),
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                TotalPrice = 1500,
                Status = BookingStatus.Completed
            };
            _context.Bookings.Add(booking);
            _context.SaveChanges();

            var exception = Assert.Throws<InvalidBookingException>(
                () => _service.CancelBooking(booking.Id));

            Assert.Equal($"Bokning med id {booking.Id} är redan slutförd och kan inte avbokas", exception.Message);
        }
        [Fact]
        public void CancelBookingShouldThrowExceptionWhenBookingAlreadyStarted()
        {
            var room = new Room
            {
                RoomNumber = 101,
                Type = RoomType.Single,
                Price = 1000,
                Description = "Test rum",
                IsActive = true
            };
            _context.Rooms.Add(room);
            _context.SaveChanges();

            var booking = new Booking
            {
                CustomerId = 1,
                RoomId = room.Id,
                CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
                TotalPrice = 1500,
                Status = BookingStatus.Active
            };

            _context.Bookings.Add(booking);
            _context.SaveChanges();

            var exception = Assert.Throws<InvalidBookingException>(
                () => _service.CancelBooking(booking.Id));

            Assert.Equal($"Bokning med id {booking.Id} har redan börjat och kan inte avbokas", exception.Message);
        }
        [Fact]
        public void UpdateCompletedBookingsShouldSetCompletedWhenCheckOutHasPassed()
        {
            var booking = new Booking
            {
                CustomerId = 1,
                RoomId = 1,
                CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(-3)),
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                TotalPrice = 1500,
                Status = BookingStatus.Active
            };
            _context.Bookings.Add(booking);
            _context.SaveChanges();

            _service.UpdateCompletedBookings();

            var updatedBooking = _context.Bookings.FirstOrDefault(b => b.Id == booking.Id);

            Assert.NotNull(updatedBooking);
            Assert.Equal(BookingStatus.Completed, updatedBooking.Status);
        }

        [Fact]
        public void UpdateBookingShouldChangeCheckInAndCheckOutDates()
        {
            var room = new Room
            {
                RoomNumber = 101,
                Type = RoomType.Single,
                Price = 1000,
                Description = "Test rum",
                IsActive = true
            };
            _context.Rooms.Add(room);
            _context.SaveChanges();

            var booking = new Booking
            {
                CustomerId = 1,
                RoomId = room.Id,
                CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
                TotalPrice = 1500,
                Status = BookingStatus.Active
            };

            _context.Bookings.Add(booking);
            _context.SaveChanges();

            var changeRequest = new BookingChangeDateRequest
            {
                CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(5))
            };

            _service.UpdateBooking(booking.Id, changeRequest);

            var savedBooking = _context.Bookings.FirstOrDefault(b => b.Id == booking.Id);

            Assert.NotNull(savedBooking);
            Assert.Equal(changeRequest.CheckIn, savedBooking.CheckIn);
            Assert.Equal(changeRequest.CheckOut, savedBooking.CheckOut);
            Assert.Equal(3000m, savedBooking.TotalPrice);
        }
        [Fact]
        public void UpdateBookingShouldThrowExceptionWhenDatesAreInvalid()
        {
            var room = new Room
            {
                RoomNumber = 101,
                Type = RoomType.Single,
                Price = 1000,
                Description = "Good nice room",
                IsActive = true
            };
            _context.Rooms.Add(room);
            _context.SaveChanges();

            var booking = new Booking
            {
                CustomerId = 1,
                RoomId = room.Id,
                CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
                TotalPrice = 2000,
                Status = BookingStatus.Active
            };

            _context.Bookings.Add(booking);
            _context.SaveChanges();

            var changeRequest = new BookingChangeDateRequest
            {
                CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(5)),
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
            };

            var exception = Assert.Throws<InvalidBookingException>(
                () => _service.UpdateBooking(booking.Id, changeRequest));
            Assert.Equal("Check-out datum måste vara efter check-in datum", exception.Message);

        }
        [Fact]
        public void UpdateBookingShouldThrowExceptionWhenNewDatesAreAlreadyBooked()
        {
            var room = new Room
            {
                RoomNumber = 101,
                Type = RoomType.Single,
                Price = 1000,
                Description = "Test rum",
                IsActive = true
            };

            _context.Rooms.Add(room);
            _context.SaveChanges();

            var booking = new Booking
            {
                CustomerId = 1,
                RoomId = room.Id,
                CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
                TotalPrice = 2000,
                Status = BookingStatus.Active
            };

            var otherBooking = new Booking
            {
                CustomerId = 1,
                RoomId = room.Id,
                CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(5)),
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(7)),
                TotalPrice = 2000,
                Status = BookingStatus.Active
            };

            _context.Bookings.AddRange(booking, otherBooking);
            _context.SaveChanges();

            var changeRequest = new BookingChangeDateRequest
            {
                CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(5)),
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(7))
            };

            var exception = Assert.Throws<RoomAlreadyBookedException>(
                () => _service.UpdateBooking(booking.Id, changeRequest));

            Assert.Equal(
                $"Rum med id {room.Id} är redan bokat för de valda datumen",
                exception.Message);
        }
        [Fact]
        public void UpdateBookingShouldNotChangeBookingWhenDatesAreInvalid()
        {
            var room = new Room
            {
                RoomNumber = 101,
                Type = RoomType.Single,
                Price = 1000,
                Description = "Test rum",
                IsActive = true
            };

            _context.Rooms.Add(room);
            _context.SaveChanges();

            var booking = new Booking
            {
                CustomerId = 1,
                RoomId = room.Id,
                CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
                TotalPrice = 2000,
                Status = BookingStatus.Active
            };

            _context.Bookings.Add(booking);
            _context.SaveChanges();

            var originalCheckIn = booking.CheckIn;
            var originalCheckOut = booking.CheckOut;

            var changeRequest = new BookingChangeDateRequest
            {
                CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(5)),
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(3))
            };

            Assert.Throws<InvalidBookingException>(() =>
                _service.UpdateBooking(booking.Id, changeRequest));

            var savedBooking = _context.Bookings
                .FirstOrDefault(b => b.Id == booking.Id);

            Assert.NotNull(savedBooking);
            Assert.Equal(originalCheckIn, savedBooking.CheckIn);
            Assert.Equal(originalCheckOut, savedBooking.CheckOut);
        }
        [Fact]
        public void UpdateBookingShouldThrowExceptionWhenBookingAlreadyStarted()
        {
            var room = new Room
            {
                RoomNumber = 101,
                Type = RoomType.Single,
                Price = 1000,
                Description = "Test rum",
                IsActive = true
            };

            _context.Rooms.Add(room);
            _context.SaveChanges();
            var booking = new Booking
            {
                CustomerId = 1,
                RoomId = room.Id,
                CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
                TotalPrice = 2000,
                Status = BookingStatus.Active
            };

            _context.Bookings.Add(booking);
            _context.SaveChanges();

            var changeRequest = new BookingChangeDateRequest
            {
                CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(5))
            };

            var exception = Assert.Throws<InvalidBookingException>(
                () => _service.UpdateBooking(booking.Id, changeRequest));

            Assert.Equal($"Bokning med id {booking.Id} har redan börjat och kan inte ändras", exception.Message);
        }
        [Fact]
        public void UpdateBookingShouldThrowExceptionWhenBookingDoesNotExist()
        {
            var changeRequest = new BookingChangeDateRequest
            {
                CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(5))
            };

            var exception = Assert.Throws<BookingNotFoundException>(
                () => _service.UpdateBooking(999, changeRequest));

            Assert.Equal($"Bokning med id 999 finns ej", exception.Message);
        }
        [Fact]
        public void UpdateBookingShouldThrowExceptionWhenBookingIsCancelled()
        {
            var room = new Room
            {
                RoomNumber = 101,
                Type = RoomType.Single,
                Price = 1000,
                Description = "Test rum",
                IsActive = true
            };

            _context.Rooms.Add(room);
            _context.SaveChanges();

            var booking = new Booking
            {
                CustomerId = 1,
                RoomId = room.Id,
                CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
                TotalPrice = 2000,
                Status = BookingStatus.Cancelled
            };

            _context.Bookings.Add(booking);
            _context.SaveChanges();

            var changeRequest = new BookingChangeDateRequest
            {
                CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(4)),
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(6))
            };

            var exception = Assert.Throws<InvalidBookingException>(
                () => _service.UpdateBooking(booking.Id, changeRequest));

            Assert.Equal($"Bokning med id {booking.Id} är avbokad och kan inte ändras", exception.Message);
        }
        [Fact]
        public void UpdateBookingShouldThrowExceptionWhenBookingIsCompleted()
        {
            var room = new Room
            {
                RoomNumber = 101,
                Type = RoomType.Single,
                Price = 1000,
                Description = "Test rum",
                IsActive = true
            };

            _context.Rooms.Add(room);
            _context.SaveChanges();

            var booking = new Booking
            {
                CustomerId = 1,
                RoomId = room.Id,
                CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(-3)),
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                TotalPrice = 2000,
                Status = BookingStatus.Completed
            };

            _context.Bookings.Add(booking);
            _context.SaveChanges();

            var changeRequest = new BookingChangeDateRequest
            {
                CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(4)),
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(6))
            };
            var exception = Assert.Throws<InvalidBookingException>(
                () => _service.UpdateBooking(booking.Id, changeRequest));

            Assert.Equal($"Bokning med id {booking.Id} är slutförd och kan inte ändras", exception.Message);
        }
        [Fact]
        public void CanceledBookingShouldAllowRoomToBeBookedAfterPreviousBookingWasCancelled()
        {

            var room = new Room
            {
                RoomNumber = 101,
                Type = RoomType.Single,
                Price = 1000,
                Description = "Test rum",
                IsActive = true
            };


            _context.Rooms.Add(room);
            _context.SaveChanges();

            var booking = new Booking
            {
                CustomerId = 1,
                RoomId = room.Id,
                CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
                TotalPrice = 2000,
                Status = BookingStatus.Cancelled
            };

            _context.Bookings.Add(booking);
            _context.SaveChanges();

            var newBooking = new Booking
            {
                CustomerId = 2,
                RoomId = room.Id,
                CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
                TotalPrice = 2000,
                Status = BookingStatus.Active

            };

            _context.Bookings.Add(newBooking);
            _context.SaveChanges();

            var savedBooking = _context.Bookings.FirstOrDefault(b => b.Id == newBooking.Id);

            Assert.NotNull(savedBooking);
            Assert.Equal(BookingStatus.Active, savedBooking.Status);
        }
        [Fact]
        public void CreateBookingShouldSaveCustomerSnapshot()
        {
            var room = new Room
            {
                RoomNumber = 101,
                Type = RoomType.Single,
                Price = 1000,
                Description = "Test rum",
                IsActive = true

            };
            var customer = new Customer
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };

            _context.Rooms.Add(room);
            _context.Customers.Add(customer);
            _context.SaveChanges();

            var booking = new Booking
            {
                CustomerId = customer.Id,
                RoomId = room.Id,
                CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
                TotalPrice = 2000,
                Status = BookingStatus.Active
            };

            var savedBooking = _service.CreateBooking(booking);

            Assert.NotNull(savedBooking);
            Assert.Equal("John", savedBooking.CustomerFirstNameSnapshot);
            Assert.Equal("Doe", savedBooking.CustomerLastNameSnapshot);
            Assert.Equal("john.doe@example.com", savedBooking.CustomerEmailSnapshot);
        }
        [Fact]
        public void CreateBookingShouldKeepCustomerSnapshotWhenCustomerIsUpdated()
        {
            var room = new Room
            {
                RoomNumber = 101,
                Type = RoomType.Single,
                Price = 1000,
                Description = "Test rum",
                IsActive = true
            };

            var customer = new Customer
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };

            _context.Rooms.Add(room);
            _context.Customers.Add(customer);
            _context.SaveChanges();

            var booking = new Booking
            {
                CustomerId = customer.Id,
                RoomId = room.Id,
                CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(3))
            };

            var savedBooking = _service.CreateBooking(booking);

            customer.FirstName = "Jane";
            customer.LastName = "Smith";
            customer.Email = "jane.smith@example.com";

            _context.SaveChanges();

            var updatedBooking = _context.Bookings
                .First(b => b.Id == savedBooking.Id);

            Assert.Equal("John", updatedBooking.CustomerFirstNameSnapshot);
            Assert.Equal("Doe", updatedBooking.CustomerLastNameSnapshot);
            Assert.Equal("john.doe@example.com", updatedBooking.CustomerEmailSnapshot);
        }
        [Fact]
        public void DeleteCustomerShouldKeepBookingWithCustomerSnapshot()
        {
            var room = new Room
            {
                RoomNumber = 101,
                Type = RoomType.Single,
                Price = 1000,
                Description = "Test rum",
                IsActive = true
            };

            var customer = new Customer
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };

            _context.Rooms.Add(room);
            _context.Customers.Add(customer);
            _context.SaveChanges();

            var booking = new Booking
            {
                CustomerId = customer.Id,
                RoomId = room.Id,
                CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(3))
            };

            var savedBooking = _service.CreateBooking(booking);

            _context.Customers.Remove(customer);

            _context.SaveChanges();

            var deletedCustomer = _context.Customers
                .FirstOrDefault(c => c.Id == customer.Id);

            var savedBookingAfterDelete = _context.Bookings
                .FirstOrDefault(b => b.Id == savedBooking.Id);

            Assert.Null(deletedCustomer);
            Assert.NotNull(savedBookingAfterDelete);
            Assert.Null(savedBookingAfterDelete.CustomerId);

            Assert.Equal("John", savedBookingAfterDelete.CustomerFirstNameSnapshot);
            Assert.Equal("Doe", savedBookingAfterDelete.CustomerLastNameSnapshot);
            Assert.Equal("john.doe@example.com", savedBookingAfterDelete.CustomerEmailSnapshot);
        }
    }

}
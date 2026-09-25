using hotelbooking.Data;
using hotelbooking.Models;
using hotelbooking.Services;
using hotelbooking.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

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
            _service = new BookingService(_context);
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
                CustomerId = 999, // Assuming customer with ID 999 does not exist
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
                RoomId = 999 // Assuming room with ID 999 does not exist
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
                IsActive = false // Set to false to test the exception
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
                CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)), // Set to yesterday
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(1)) // Set to tomorrow
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
                RoomId = 999, // Non-existent room
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
        public void CreateBookingShouldThrowExceptionWhenRoomIsNullInValidateRoom()
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
                RoomId = 999 // Non-existent room
            };
            var exception = Assert.Throws<RoomNotFoundException>(
                () => _service.CreateBooking(booking));
            Assert.Equal($"Rum med id {booking.RoomId} finns ej", exception.Message);
        }
    }
}
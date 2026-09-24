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
                RoomId = 1, // Assuming room with ID 1 exists
              
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
    }
}

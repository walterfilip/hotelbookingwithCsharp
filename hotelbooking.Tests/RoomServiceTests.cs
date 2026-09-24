using System;
using System.Collections.Generic;
using System.Text;
using hotelbooking.Data;
using hotelbooking.Models;
using hotelbooking.Services;
using hotelbooking.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace hotelbooking.Tests
{
    public class RoomServiceTests
    {

        private readonly AppDbContext _context;

        private readonly RoomService _service;

        public RoomServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                 .UseInMemoryDatabase(Guid.NewGuid().ToString())
                 .Options;

            _context = new AppDbContext(options);

            using var loggerFactory = LoggerFactory.Create(builder => { });
            var logger = loggerFactory.CreateLogger<RoomService>();

            _service = new RoomService(_context, logger);
        }
        [Fact]
        public void GetRoomShouldReturnRoom()
        {
            var room = new Room
            {
                RoomNumber = 101,
                Type = RoomType.Single,
                Price = 100.0m,
                Description = "A cozy single room."
            };
            _context.Rooms.Add(room);
            _context.SaveChanges();

            var result = _service.GetRoom(room.Id);

            Assert.NotNull(result);
            Assert.Equal(101, result.RoomNumber);
            Assert.Equal(RoomType.Single, result.Type);
        }
        [Fact]
        public void GetRoomShouldThrowExceptionWhenRoomDoesNotExist()
        {
            var exception = Assert.Throws<RoomNotFoundException>(() => _service.GetRoom(999)); // Assuming 999 is an ID that does not exist


            Assert.Equal("Rummet med id 999 hittades inte", exception.Message);
        }

        [Fact]
        public void GetAllRoomsShouldReturnAllRooms()
        {
            var room1 = new Room
            {
                RoomNumber = 101,
                Type = RoomType.Single,
                Price = 100.0m,
                Description = "A cozy single room."
            };
            var room2 = new Room
            {
                RoomNumber = 102,
                Type = RoomType.Double,
                Price = 150.0m,
                Description = "A spacious double room."
            };

            _context.Rooms.AddRange(room1, room2);
            _context.SaveChanges();

            var result = _service.GetAllRooms();
            Assert.Equal(2, result.Count);

        }
        [Fact]
        public void GetAllRoomsShouldReturnEmptyListWhenNoRoomsExist()
        {
            var result = _service.GetAllRooms();
            Assert.Empty(result);
        }
        [Fact]
        public void CreateRoomShouldReturnRoom()
        {
            var room = new Room
            {
                RoomNumber = 103,
                Type = RoomType.Single,
                Price = 120.0m,
                Description = "A comfortable single room."
            };

            var result = _service.CreateRoom(room);

            Assert.NotNull(result);
            Assert.Equal(103, result.RoomNumber);
            Assert.Equal(RoomType.Single, result.Type);
        }
        [Fact]
        public void CreateRoomShouldThrowExceptionWhenPriceIsInvalid()
        {
            var room = new Room
            {
                RoomNumber = 104,
                Type = RoomType.Double,
                Price = -50.0m,
                Description = "An invalid room."
            };
            var exception = Assert.Throws<InvalidRoomException>(() => _service.CreateRoom(room));
            Assert.Equal("Pris måste vara större än 0", exception.Message);
        }
        [Fact]
        public void CreateRoomShouldThrowExceptionWhenTypeIsInvalid()
        {
            var room = new Room
            {
                RoomNumber = 105,
                Type = (RoomType)999, // Invalid type
                Price = 200.0m,
                Description = "An invalid room."
            };
            var exception = Assert.Throws<InvalidRoomException>(() => _service.CreateRoom(room));
            Assert.Equal("Ogiltig rumstyp", exception.Message);
        }
        [Fact]
        public void CreateRoomShouldThrowExceptionWhenRoomNumberIsInvalid()
        {
            var room = new Room
            {
                RoomNumber = 0,
                Type = RoomType.Single,
                Price = 100.0m,
                Description = "An invalid room."
            };
            var exception = Assert.Throws<InvalidRoomException>(() => _service.CreateRoom(room));

            Assert.Equal("Rumsnummer måste vara större än 0", exception.Message);
        }
        [Fact]
        public void CreateRoomShouldThrowExceptionWhenDescriptionIsMissing()
        {
            var room = new Room
            {
                RoomNumber = 106,
                Type = RoomType.Single,
                Price = 100.0m,
                Description = ""
            };
            var exception = Assert.Throws<InvalidRoomException>(() => _service.CreateRoom(room));
            Assert.Equal("Beskrivning måste anges", exception.Message);
        }
        [Fact]
        public void UpdateRoomShouldUpdateRoom()
        {
            var room = new Room
            {
                RoomNumber = 107,
                Type = RoomType.Single,
                Price = 100.0m,
                Description = "A single room."
            };

            _context.Rooms.Add(room);
            _context.SaveChanges();

            var updatedRoom = new Room
            {
                RoomNumber = 108,
                Type = RoomType.Double,
                Price = 150.0m,
                Description = "A double room."
            };

            var result = _service.UpdateRoom(room.Id, updatedRoom);

            Assert.NotNull(result);
            Assert.Equal(108, result.RoomNumber);
            Assert.Equal(updatedRoom.Price, result.Price);
            Assert.Equal(RoomType.Double, result.Type);
            Assert.Equal("A double room.", result.Description);
        }
        [Fact]
        public void UpdateRoomShouldThrowExceptionWhenRoomDoesNotExist()
        {
            var updatedRoom = new Room
            {
                RoomNumber = 109,
                Type = RoomType.Single,
                Price = 100.0m,
                Description = "A single room."
            };
            var exception = Assert.Throws<RoomNotFoundException>(() => _service.UpdateRoom(999, updatedRoom)); // Assuming 999 is an ID that does not exist
            Assert.Equal("Rummet med id 999 hittades inte", exception.Message);
        }

        [Fact]
        public void UpdateRoomShouldThrowExceptionWhenPriceIsInvalid()
        {
            var room = new Room
            {
                RoomNumber = 110,
                Type = RoomType.Single,
                Price = 100.0m,
                Description = "A single room."
            };

            _context.Rooms.Add(room);
            _context.SaveChanges();

            var updatedRoom = new Room
            {
                RoomNumber = 111,
                Type = RoomType.Double,
                Price = -150.0m, // Invalid price
                Description = "A double room."
            };

            var exception = Assert.Throws<InvalidRoomException>(() => _service.UpdateRoom(room.Id, updatedRoom));

            Assert.Equal("Pris måste vara större än 0", exception.Message);
        }
        [Fact]
        public void UpdateRoomShouldThrowExceptionWhenTypeIsInvalid()
        {
            var room = new Room
            {
                RoomNumber = 112,
                Type = RoomType.Single,
                Price = 100.0m,
                Description = "A single room."
            };

            _context.Rooms.Add(room);
            _context.SaveChanges();

            var updatedRoom = new Room
            {
                RoomNumber = 113,
                Type = (RoomType)999, // Invalid type
                Price = 150.0m,
                Description = "A double room."
            };

            var exception = Assert.Throws<InvalidRoomException>(() => _service.UpdateRoom(room.Id, updatedRoom));

            Assert.Equal("Ogiltig rumstyp", exception.Message);
        }
        [Fact]
        public void UpdateRoomShouldThrowExceptionWhenRoomNumberIsInvalid()
        {
            var room = new Room
            {
                RoomNumber = 114,
                Type = RoomType.Single,
                Price = 100.0m,
                Description = "A single room."
            };

            _context.Rooms.Add(room);
            _context.SaveChanges();

            var updatedRoom = new Room
            {
                RoomNumber = 0, // Invalid room number
                Type = RoomType.Double,
                Price = 150.0m,
                Description = "A double room."
            };

            var exception = Assert.Throws<InvalidRoomException>(() => _service.UpdateRoom(room.Id, updatedRoom));

            Assert.Equal("Rumsnummer måste vara större än 0", exception.Message);
        }
        [Fact]
        public void UpdateRoomShouldThrowExceptionWhenDescriptionIsMissing()
        {
            var room = new Room
            {
                RoomNumber = 115,
                Type = RoomType.Single,
                Price = 100.0m,
                Description = "A single room."
            };

            _context.Rooms.Add(room);
            _context.SaveChanges();

            var updatedRoom = new Room
            {
                RoomNumber = 116,
                Type = RoomType.Double,
                Price = 150.0m,
                Description = "" // Missing description
            };

            var exception = Assert.Throws<InvalidRoomException>(() => _service.UpdateRoom(room.Id, updatedRoom));

            Assert.Equal("Beskrivning måste anges", exception.Message);
        }
        [Fact]
        public void DeleteRoomShouldSetRoomNotActive()
        {
            var room = new Room
            {
                RoomNumber = 117,
                Type = RoomType.Single,
                Price = 100.0m,
                Description = "A single room."
            };

            _context.Rooms.Add(room);
            _context.SaveChanges();

            var deleted =_service.DeleteRoom(room.Id);
            Assert.True(deleted);

            var deletedRoom = _context.Rooms.FirstOrDefault(r => r.Id == room.Id);
            Assert.NotNull(deletedRoom);
            Assert.False(deletedRoom.IsActive);
        }
        [Fact]
        public void DeleteRoomShouldReturnFalseWhenRoomDoesNotExist()
        {
            var deleted = _service.DeleteRoom(999); // Assuming 999 is an ID that does not exist
            Assert.False(deleted);
        }
    }
}

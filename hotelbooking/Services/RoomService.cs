using hotelbooking.Data;
using hotelbooking.Models;
using hotelbooking.Exceptions;

namespace hotelbooking.Services
{
    public class RoomService
    {

        private readonly AppDbContext _context;

        private readonly ILogger<RoomService> _logger;

        public RoomService(AppDbContext context, ILogger<RoomService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public Room GetRoom(int id)
        {
            Room? room = _context.Rooms.FirstOrDefault(r => r.Id == id);
            if (room == null)
            {
                _logger.LogWarning("Rummet med id {Id} hittades inte", id);
                throw new RoomNotFoundException($"Rummet med id {id} hittades inte");
            }
            return room;
        }
        public List<Room> GetAllRooms()
        {
            return _context.Rooms.ToList();
        }

        public Room CreateRoom(Room room)
        {
            ValidateRoom(room);

            _context.Rooms.Add(room);
            _context.SaveChanges();

            _logger.LogInformation("Rummet med id {Id} skapades", room.Id);
            return room;
        }
        public Room UpdateRoom(int id, Room room)
        {
            Room? existingRoom = _context.Rooms.FirstOrDefault(r => r.Id == id);
            if(existingRoom == null)
            {
                _logger.LogWarning("Rummet med id {Id} hittades inte", id);
                throw new RoomNotFoundException($"Rummet med id {id} hittades inte");
            }

            ValidateRoom(room);

            existingRoom.Price = room.Price;
            existingRoom.Type = room.Type;
            existingRoom.RoomNumber = room.RoomNumber;
            existingRoom.Description = room.Description;

            _context.SaveChanges();
            _logger.LogInformation("Rummet med id {Id} uppdaterades", existingRoom.Id);

          return existingRoom;           
        }
        public bool DeleteRoom(int id)
        {
            Room? room = _context.Rooms.FirstOrDefault(r => r.Id == id);
            if (room == null)
            {
                _logger.LogWarning("Rummet med id {Id} hittades inte",id);
                return false;
            }
            room.IsActive = false;
            _context.SaveChanges();

            _logger.LogInformation("Rummet med id {Id} har blivit inaktiverat", id);

            return true;
        }
        private void ValidateRoom(Room room)
        {
            if (room.Price <= 0)
            {
                _logger.LogWarning("Försök att spara rum med ogiltigt pris: {Price}", room.Price);
                throw new InvalidRoomException("Pris måste vara större än 0");
            }
            if (!Enum.IsDefined(room.Type))
            {
                _logger.LogWarning("Försök att spara rum med ogiltig typ: {Type}", room.Type);
                throw new InvalidRoomException("Ogiltig rumstyp");
            }
            if (room.RoomNumber <= 0)
            {
                _logger.LogWarning("Försök att spara rum med ogiltigt rumsnummer: {RoomNumber}", room.RoomNumber);
                throw new InvalidRoomException("Rumsnummer måste vara större än 0");
            }
            if (string.IsNullOrWhiteSpace(room.Description))
            {
                _logger.LogWarning("Försök att spara rum utan beskrivning");
                throw new InvalidRoomException("Beskrivning måste anges");
            }   
        }

    }
}

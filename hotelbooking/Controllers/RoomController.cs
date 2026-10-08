using hotelbooking.DTOs;
using hotelbooking.Models;
using hotelbooking.Services;
using Microsoft.AspNetCore.Mvc;

namespace hotelbooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly RoomService _roomService;

        public RoomController(RoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet]
        public ActionResult<List<RoomResponseDto>> GetAllRooms()
        {
            var rooms = _roomService.GetAllRooms();
            return Ok(rooms.Select(ToDto).ToList());
        }
        [HttpGet("{id}")]
        public ActionResult<RoomResponseDto> GetRoom(int id)
        {
            var room = _roomService.GetRoom(id);
           
            return Ok(ToDto(room));
        }
        [HttpPost]
        public ActionResult<RoomResponseDto> CreateRoom(Room room)
        {
            var createdRoom = _roomService.CreateRoom(room);
            var dto = ToDto(createdRoom);

            return CreatedAtAction(nameof(GetRoom), new { id = dto.Id }, dto);
        }
        [HttpPut("{id}")]
        public ActionResult<RoomResponseDto> UpdateRoom(int id, Room room)
        {
            var updateRoom = _roomService.UpdateRoom(id, room);
            return Ok(ToDto(updateRoom));
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteRoom(int id)
        {
            bool deleted = _roomService.DeleteRoom(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }

        private RoomResponseDto ToDto(Room room)
        {
            return new RoomResponseDto
            {
                Id = room.Id,
                RoomNumber = room.RoomNumber,
                Type = room.Type,
                Price = room.Price,
                Description = room.Description,
                IsActive = room.IsActive
            };
        }
    }
}

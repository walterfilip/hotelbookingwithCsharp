using Microsoft.AspNetCore.Mvc;
using hotelbooking.Services;
using hotelbooking.Models;

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
        public List<Room> GetAllRooms()
        {
            return _roomService.GetAllRooms();
        }
        [HttpGet("{id}")]
        public ActionResult<Room> GetRoom(int id)
        {
            Room room = _roomService.GetRoom(id);
           
            return Ok(room);
        }
        [HttpPost]
        public ActionResult<Room> CreateRoom(Room room)
        {
            Room createdRoom = _roomService.CreateRoom(room);          

            return CreatedAtAction(nameof(GetRoom), new { id = createdRoom.Id }, createdRoom);
        }
        [HttpPut("{id}")]
        public ActionResult<Room> UpdateRoom(int id, Room room)
        {
            Room updateRoom = _roomService.UpdateRoom(id, room);
            return Ok(updateRoom);
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
    }
}

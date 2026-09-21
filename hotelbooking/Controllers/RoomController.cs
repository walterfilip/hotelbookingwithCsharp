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

    }
}

using Microsoft.AspNetCore.Mvc;
using Marconnes.ConsoleApp;

namespace Marconnes.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelRoomController : ControllerBase
    {
        private readonly DAL _dal;

        public HotelRoomController(DAL dal)
        {
            _dal = dal;
        }

        // 1. GET ALL
        [HttpGet("all")]
        public IActionResult GetAll()
        {
            var rooms = _dal.GetAllRooms();
            return Ok(rooms);
        }

        // 2. GET BY ROOM
        [HttpGet("search/{RoomNumber}")]
        public IActionResult GetById(int RoomNumber)
        {
            var room = _dal.GetRoomById(RoomNumber);
            if (room == null)
            {
                return NotFound();
            }
            return Ok(room);
        }

        // 3. CREATE
        [HttpPost("add/{room}")]
        public IActionResult Create(HotelRoom room)
        {
            _dal.AddHotelRoom(room);
            return Ok("Hotelkamer succesvol toegevoegd!");
        }

        // 4. UPDATE
        [HttpPut("update/{RoomNumber}")]
        public IActionResult Update(int RoomNumber, HotelRoom room)
        {
            if (RoomNumber != room.RoomNumber)
            {
                return BadRequest("Kamernummer matcht niet");
            }

            _dal.UpdateRoom(room);
            return NoContent();
        }

        // 5. DELETE
        [HttpDelete("delete/{RoomNumber}")]
        public IActionResult Delete(int RoomNumber)
        {
            _dal.DeleteRoom(RoomNumber);
            return NoContent();
        }
    }
}
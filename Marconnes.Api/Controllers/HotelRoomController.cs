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
        [HttpGet]
        public IActionResult GetAll()
        {
            var rooms = _dal.GetAllRooms();
            return Ok(rooms);
        }

        // 2. GET BY ID
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var room = _dal.GetRoomById(id);
            if (room == null)
            {
                return NotFound();
            }
            return Ok(room);
        }

        // 3. CREATE
        [HttpPost]
        public IActionResult Create(HotelRoom room)
        {
            _dal.AddHotelRoom(room);
            return Ok("Hotelkamer succesvol toegevoegd!");
        }

        // 4. UPDATE
        [HttpPut("{id}")]
        public IActionResult Update(int id, HotelRoom room)
        {
            if (id != room.RoomID)
            {
                return BadRequest("ID matcht niet");
            }

            _dal.UpdateRoom(room);
            return NoContent();
        }

        // 5. DELETE
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _dal.DeleteRoom(id);
            return NoContent();
        }
    }
}
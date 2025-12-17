using Microsoft.AspNetCore.Mvc;
using Marconnes.ConsoleApp;

namespace Marconnes.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CampingPlaceController : ControllerBase
    {
        private readonly DAL _dal;

        public CampingPlaceController(DAL dal)
        {
            _dal = dal;
        }

        // 1. GET ALL
        [HttpGet]
        public IActionResult GetAll()
        {
            var Places = _dal.GetAllPlaces();
            return Ok(Places);
        }

        // 2. GET BY ID
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var Place = _dal.GetPlaceById(id);
            if (Place == null)
            {
                return NotFound();
            }
            return Ok(Place);
        }

        // 3. CREATE
        [HttpPost]
        public IActionResult Create(CampingPlace Place)
        {
            _dal.AddCampingPlace(Place);
            return Ok("Campingplek succesvol toegevoegd!");
        }

        // 4. UPDATE
        [HttpPut("{id}")]
        public IActionResult Update(int id, CampingPlace Place)
        {
            if (id != Place.PlaceID)
            {
                return BadRequest("ID matcht niet");
            }

            _dal.UpdatePlace(Place);
            return NoContent();
        }

        // 5. DELETE
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _dal.DeletePlace(id);
            return NoContent();
        }
    }
}
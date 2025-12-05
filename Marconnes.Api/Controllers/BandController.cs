using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Marconnes.Api.Controllers
{
    using Marconnes.ConsoleApp;
    using Microsoft.AspNetCore.Mvc;

    namespace BandAPI.Controllers
    {
        [ApiController]
        [Route("[controller]")]
        public class BandsController : Controller
        {
            private readonly DAL dal;

            public BandsController(DAL _dal)
            {
                dal = _dal;
            }

            [HttpGet]
            public IActionResult GetAll()
            {
                var bands = dal.GetAllBands();
                return Ok(bands);
            }

            [HttpGet("{id}")]
            public IActionResult GetById(int id)
            {
                var band = dal.GetBandById(id);
                if (band == null) return NotFound();
                return Ok(band);
            }

            [HttpPost]
            public IActionResult Create(Band band)
            {
                dal.AddBand(band);
                return CreatedAtAction(nameof(GetById), new { id = band.Id }, band);
            }

            [HttpPut("{id}")]
            public IActionResult Update(int id, Band band)
            {
                if (id != band.Id) return BadRequest();
                dal.UpdateBand(band);
                return NoContent();
            }

            [HttpDelete("{id}")]
            public IActionResult Delete(int id)
            {
                dal.DeleteBand(id);
                return NoContent();
            }
        }
    }


}

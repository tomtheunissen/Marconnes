using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CampingEF.Models;

namespace CampingEF.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly MarconnesDbContext _context;

        public DataController(MarconnesDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CampingPlace>>> GetAlles()
        {
            return await _context.CampingPlaces.ToListAsync();
        }

        [HttpGet("zoek/{zoekterm}")]
        public async Task<ActionResult<IEnumerable<CampingPlace>>> ZoekOpNummer(string zoekterm)
        {
            var resultaat = await _context.CampingPlaces
                .Where(c => c.PlaceNumber.Contains(zoekterm))
                .ToListAsync();

            return resultaat;
        }

        [HttpPost]
        public async Task<ActionResult<CampingPlace>> Toevoegen(CampingPlace nieuwePlek)
        {
            nieuwePlek.PlaceId = 0;

            _context.CampingPlaces.Add(nieuwePlek);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAlles), new { id = nieuwePlek.PlaceId }, nieuwePlek);
        }
    }
}
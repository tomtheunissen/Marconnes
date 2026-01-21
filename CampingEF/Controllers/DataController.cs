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

        // 4. WIJZIGEN (Update)
        [HttpPut("{id}")]
        public async Task<IActionResult> Wijzigen(int id, CampingPlace gewijzigdePlek)
        {
            if (id != gewijzigdePlek.PlaceId)
            {
                return BadRequest("Het ID in de URL matcht niet met het ID in de data.");
            }

            _context.Entry(gewijzigdePlek).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                bool bestaatNog = _context.CampingPlaces.Any(e => e.PlaceId == id);

                if (!bestaatNog)
                {
                    return NotFound();
                }
                else
                {
                    throw; 
                }
            }

            return NoContent();
        }

        // 5. OPHALEN OP ID (Details)
        [HttpGet("{id}")]
        public async Task<ActionResult<CampingPlace>> GetOpId(int id)
        {
            var plek = await _context.CampingPlaces.FindAsync(id);

            if (plek == null)
            {
                return NotFound();
            }

            return plek;
        }

        // 6. ZOEKEN OP PLEKNUMMER (Tekst, bijv. "A12" of "B01")
        [HttpGet("zoek/{plekNummer}")]
        public async Task<ActionResult<IEnumerable<CampingPlace>>> ZoekOpNummer(string plekNummer)
        {
            var resultaten = await _context.CampingPlaces
                .Where(p => p.PlaceNumber.Contains(plekNummer))
                .ToListAsync();

            if (resultaten == null || resultaten.Count == 0)
            {
                return NotFound("Geen plekken gevonden met dit nummer.");
            }

            return resultaten;
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
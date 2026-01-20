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
        // De url wordt: PUT /api/Data/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Wijzigen(int id, CampingPlace gewijzigdePlek)
        {
            // Check 1: Komt het ID in de URL overeen met het ID in de data?
            // Let op: PlaceId met kleine 'd' (zoals in jouw model)
            if (id != gewijzigdePlek.PlaceId)
            {
                return BadRequest("Het ID in de URL matcht niet met het ID in de data.");
            }

            // Vertel EF Core dat dit object is aangepast
            _context.Entry(gewijzigdePlek).State = EntityState.Modified;

            try
            {
                // Probeer op te slaan
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Check of de plek nog wel bestaat in de database
                bool bestaatNog = _context.CampingPlaces.Any(e => e.PlaceId == id);

                if (!bestaatNog)
                {
                    return NotFound();
                }
                else
                {
                    throw; // Er was een andere onbekende fout
                }
            }

            // 204 No Content is de standaard response voor een gelukte update
            return NoContent();
        }

        // 5. OPHALEN OP ID (Details)
        [HttpGet("{id}")]
        public async Task<ActionResult<CampingPlace>> GetOpId(int id)
        {
            // FindAsync is geoptimaliseerd om heel snel te zoeken op Primary Key
            var plek = await _context.CampingPlaces.FindAsync(id);

            // Check of er iets gevonden is
            if (plek == null)
            {
                return NotFound(); // Geeft een 404 error als het ID niet bestaat
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
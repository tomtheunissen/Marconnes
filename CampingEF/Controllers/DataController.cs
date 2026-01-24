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
            return await _context.CampingPlaces
    .Include(c => c.Reserveringens)
    .ThenInclude(r => r.Gebruiker)
    .ToListAsync();
        }

        // 4. WIJZIGEN (Update)
        [HttpPut("{id}")]
        public async Task<IActionResult> Wijzigen(int id, CampingPlace gewijzigdePlek)
        {
            if (id != gewijzigdePlek.PlaceNumber)
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
                bool bestaatNog = _context.CampingPlaces.Any(e => e.PlaceNumber == id);

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
                .Where(p => p.PlaceNumber.ToString().Contains(plekNummer))
                .ToListAsync();

            if (resultaten == null || resultaten.Count == 0)
            {
                return NotFound("Geen plekken gevonden met dit nummer.");
            }

            return resultaten;
        }

        [HttpPost("campingplek")]
        public async Task<ActionResult<CampingPlace>> Toevoegen(CampingPlace nieuwePlek)
        {
            nieuwePlek.PlaceNumber = 0;

            _context.CampingPlaces.Add(nieuwePlek);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAlles), new { id = nieuwePlek.PlaceNumber }, nieuwePlek);
        }

        [HttpGet("reserveringen")]
        public async Task<ActionResult<IEnumerable<Reserveringen>>> GetReserveringen()
        {
            return await _context.Reserveringens
                .Include(r => r.AccomodatieNavigation)
                .Include(r => r.Gebruiker)
                .ToListAsync();
        }

        // In Camping API -> Controllers -> ReserveringenController.cs
        [HttpPost("add_reserveringen")]
        public async Task<ActionResult<Reserveringen>> PostReservering(Reserveringen reservering)
        {
            // Voeg toe aan database
            _context.Reserveringens.Add(reservering);
            await _context.SaveChangesAsync();

            // Geef terug wat er is aangemaakt (standaard REST-regel)
            return CreatedAtAction(nameof(GetReserveringen), new { reserveringId = reservering.ReserveringId }, reservering);
        }

        [HttpDelete("delete_reserveringen/{ReserveringId}")]
        public async Task<IActionResult> DeleteReservering(int ReserveringId)
        {
            // 1. Zoek de reservering op basis van het ID
            var reservering = await _context.Reserveringens.FindAsync(ReserveringId);

            // 2. Als hij niet bestaat, geef NotFound terug
            if (reservering == null)
            {
                return NotFound("Reservering niet gevonden.");
            }

            // 3. Verwijder uit de database en sla op
            _context.Reserveringens.Remove(reservering);
            await _context.SaveChangesAsync();

            // 4. Geef NoContent (204) terug, dit is standaard voor een succesvolle delete
            return NoContent();
        }
    }
}
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

        // ==========================================
        // CAMPING PLACES
        // ==========================================

        [HttpGet("all_Camping")]
        public async Task<ActionResult<IEnumerable<CampingPlace>>> GetAlles()
        {
            return await _context.CampingPlaces
                .Include(c => c.Reserveringens)
                .ThenInclude(r => r.Gebruiker)
                .ToListAsync();
        }

        [HttpGet("zoek/{PlekNummer:int}")]
        public async Task<ActionResult<IEnumerable<CampingPlace>>> ZoekCampingPlek(int PlekNummer)
        {
            var resultaat = await _context.CampingPlaces
                                          .Include(c => c.Reserveringens)
                                          .ThenInclude(r => r.Gebruiker)
                                          .Where(c => c.PlaceNumber == PlekNummer)
                                          .ToListAsync();

            if (resultaat == null || resultaat.Count == 0)
            {
                return NotFound($"Geen campingplek gevonden met nummer {PlekNummer}");
            }

            return Ok(resultaat);
        }

        [HttpPut("update/{PlekNummer}")]
        public async Task<IActionResult> Wijzigen(int PlekNummer, CampingPlace gewijzigdePlek)
        {
            if (PlekNummer != gewijzigdePlek.PlaceNumber)
                return BadRequest("ID matcht niet.");

            _context.Entry(gewijzigdePlek).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.CampingPlaces.Any(e => e.PlaceNumber == PlekNummer)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        [HttpPost("add/camping")]
        public async Task<ActionResult<CampingPlace>> Toevoegen(CampingPlace nieuwePlek)
        {
            nieuwePlek.PlaceNumber = 0;
            _context.CampingPlaces.Add(nieuwePlek);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAlles), new { id = nieuwePlek.PlaceNumber }, nieuwePlek);
        }

        // ==========================================
        // RESERVERINGEN
        // ==========================================

        [HttpGet("all_reserveringen")]
        public async Task<ActionResult<IEnumerable<Reserveringen>>> GetReserveringen()
        {
            return await _context.Reserveringens
                .Include(r => r.AccomodatieNavigation)
                .Include(r => r.Gebruiker)
                .ToListAsync();
        }

        [HttpPost("add/reservering")]
        public async Task<ActionResult<Reserveringen>> PostReservering(Reserveringen reservering)
        {
            _context.Reserveringens.Add(reservering);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetReserveringen), new { reserveringId = reservering.ReserveringId }, reservering);
        }

        [HttpDelete("delete_reserveringen/{ReserveringId}")]
        public async Task<IActionResult> DeleteReservering(int ReserveringId)
        {
            var reservering = await _context.Reserveringens.FindAsync(ReserveringId);
            if (reservering == null) return NotFound("Reservering niet gevonden.");

            _context.Reserveringens.Remove(reservering);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ==========================================
        // GEBRUIKERS
        // ==========================================

        [HttpGet("all_gebruikers")]
        public async Task<ActionResult<IEnumerable<Gebruiker>>> GetAllUsers()
        {
            return await _context.Gebruikers
                .Include(u => u.Reserveringens)
                    .ThenInclude(r => r.AccomodatieNavigation)
                .ToListAsync();
        }

        [HttpGet("zoek/{naamGebruiker}")]
        public async Task<ActionResult<IEnumerable<Gebruiker>>> ZoekOpNaam(string naamGebruiker)
        {
            var users = await _context.Gebruikers
                .Include(u => u.Reserveringens)
                .ThenInclude(r => r.AccomodatieNavigation)
                .Where(u => u.Naam.Contains(naamGebruiker))
                .ToListAsync();

            if (users == null || !users.Any()) return NotFound("Geen gebruikers gevonden.");

            return users;
        }

        [HttpPost("add/gebruiker")]
        public async Task<ActionResult<Gebruiker>> Toevoegen(Gebruiker nieuweUser)
        {
            _context.Gebruikers.Add(nieuweUser);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(ZoekOpNaam), new { gebruiker = nieuweUser.Naam }, nieuweUser);
        }
    }
}
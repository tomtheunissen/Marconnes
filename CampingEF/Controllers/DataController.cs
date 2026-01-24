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

        [HttpGet("all/{CampingPlace}")]
        public async Task<ActionResult<IEnumerable<CampingPlace>>> GetAlles()
        {
            return await _context.CampingPlaces
    .Include(c => c.Reserveringens)
    .ThenInclude(r => r.Gebruiker)
    .ToListAsync();
        }

        // 4. WIJZIGEN (Update)
        [HttpPut("update/{PlekNummer}")]
        public async Task<IActionResult> Wijzigen(int PlekNummer, CampingPlace gewijzigdePlek)
        {
            if (PlekNummer != gewijzigdePlek.PlaceNumber)
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
                bool bestaatNog = _context.CampingPlaces.Any(e => e.PlaceNumber == PlekNummer);

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

        // 6. ZOEKEN OP PLEKNUMMER (Tekst, bijv. "A12" of "B01")
        [HttpGet("zoek/{PlekNummer}")]
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

        [HttpPost("update/{PlekNummer}")]
        public async Task<ActionResult<CampingPlace>> Toevoegen(CampingPlace nieuwePlek)
        {
            nieuwePlek.PlaceNumber = 0;

            _context.CampingPlaces.Add(nieuwePlek);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAlles), new { id = nieuwePlek.PlaceNumber }, nieuwePlek);
        }

        [HttpGet("all/{Reserveringen}")]
        public async Task<ActionResult<IEnumerable<Reserveringen>>> GetReserveringen()
        {
            return await _context.Reserveringens
                .Include(r => r.AccomodatieNavigation)
                .Include(r => r.Gebruiker)
                .ToListAsync();
        }

        // In Camping API -> Controllers -> ReserveringenController.cs
        [HttpPost("add/{Reserveringen}")]
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

        [HttpGet("gebruikers")]
        public async Task<ActionResult<IEnumerable<Gebruiker>>> GetAllUsers()
        {
            return await _context.Gebruikers
                // We laden direct alle gekoppelde data mee
                .Include(u => u.Reserveringens)
                    .ThenInclude(r => r.AccomodatieNavigation)
                .ToListAsync();
        }

        // 1. ZOEKEN OP NAAM (Aangepast)
        [HttpGet("zoek-gebruiker/{naamGebruiker}")]
        public async Task<ActionResult<IEnumerable<Gebruiker>>> ZoekOpNaam(string naamGebruiker)
        {
            // WIJZIGING: Alleen zoeken in de kolom 'Naam'
            var users = await _context.Gebruikers
                .Include(u => u.Reserveringens)
                .ThenInclude(r => r.AccomodatieNavigation)

                .Where(u => u.Naam.Contains(naamGebruiker))
                .ToListAsync();

            if (users == null || !users.Any())
            {
                return NotFound("Geen gebruikers gevonden met deze naam.");
            }

            return users;
        }

        // 2. TOEVOEGEN (Aangepast)
        [HttpPost("add/{gebruiker}")]
        public async Task<ActionResult<Gebruiker>> Toevoegen(Gebruiker nieuweUser)
        {
            _context.Gebruikers.Add(nieuweUser);
            await _context.SaveChangesAsync();

            // WIJZIGING: Verwijzing naar 'nieuweUser.Naam'
            return CreatedAtAction(nameof(ZoekOpNaam), new { gebruiker = nieuweUser.Naam }, nieuweUser);
        }
    }
}
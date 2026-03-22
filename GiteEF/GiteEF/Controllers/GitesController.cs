using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GiteEF.Models;

namespace GiteEF.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GitesController : ControllerBase
    {
        private readonly MarconnesDbContext _context;

        public GitesController(MarconnesDbContext context)
        {
            _context = context;
        }

        // GET: api/Gites
        [HttpGet("all_gites")]
        public async Task<ActionResult<IEnumerable<Gite>>> GetGites()
        {
            return await _context.Gites.ToListAsync();
        }

        // GET: api/Gites/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Gite>> GetGite(int id)
        {
            var gite = await _context.Gites.FindAsync(id);

            if (gite == null)
            {
                return NotFound();
            }

            return gite;
        }

        // PUT: api/Gites/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGite(int id, Gite gite)
        {
            if (id != gite.GiteNumber)
            {
                return BadRequest();
            }

            _context.Entry(gite).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GiteExists(id))
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

        // POST: api/Gites
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Gite>> PostGite(Gite gite)
        {
            _context.Gites.Add(gite);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGite", new { id = gite.GiteNumber }, gite);
        }

        // DELETE: api/Gites/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGite(int id)
        {
            var gite = await _context.Gites.FindAsync(id);
            if (gite == null)
            {
                return NotFound();
            }

            _context.Gites.Remove(gite);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GiteExists(int id)
        {
            return _context.Gites.Any(e => e.GiteNumber == id);
        }
    }
}

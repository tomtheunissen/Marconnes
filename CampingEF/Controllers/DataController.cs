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
    }
}
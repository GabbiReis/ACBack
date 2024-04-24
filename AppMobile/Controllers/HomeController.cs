using AppMobile.AppMobile.Dados;
using AppMobile.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;

namespace AppMobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConvitesController : ControllerBase
    {
        private readonly AppMobileContext _context;

        public ConvitesController(AppMobileContext context)
        {
            _context = context;
        }

        // GET: api/Convites
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Convite>>> GetConvites()
        {
            return await _context.Convites.ToListAsync();
        }

        // GET: api/Convites/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Convite>> GetConvite(int id)
        {
            var convite = await _context.Convites.FindAsync(id);

            if (convite == null)
            {
                return NotFound();
            }

            return convite;
        }

        // POST: api/Convites
        [HttpPost]
        public async Task<ActionResult<Convite>> PostConvite(Convite convite)
        {
            _context.Convites.Add(convite);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetConvite), new { id = convite.ID }, convite);
        }

        // DELETE: api/Convites/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConvite(int id)
        {
            var convite = await _context.Convites.FindAsync(id);
            if (convite == null)
            {
                return NotFound();
            }

            _context.Convites.Remove(convite);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ConviteExists(int id)
        {
            return _context.Convites.Any(e => e.ID == id);
        }
    }
}

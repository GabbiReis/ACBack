using AppMobile.AppMobile.Dados;
using AppMobile.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;

namespace AppMobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SorteiosController : ControllerBase
    {
        private readonly AppMobileContext _context;

        public SorteiosController(AppMobileContext context)
        {
            _context = context;
        }

        // GET: api/Sorteios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sorteios>>> GetSorteios()
        {
            return await _context.Sorteios.ToListAsync();
        }

        // GET: api/Sorteios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Sorteios>> GetSorteio(int id)
        {
            var sorteio = await _context.Sorteios.FindAsync(id);

            if (sorteio == null)
            {
                return NotFound();
            }

            return sorteio;
        }

        // POST: api/Sorteios
        [HttpPost]
        public async Task<ActionResult<Sorteios>> PostSorteio(Sorteios sorteio)
        {
            _context.Sorteios.Add(sorteio);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSorteio", new { id = sorteio.ID }, sorteio);
        }
    }
}

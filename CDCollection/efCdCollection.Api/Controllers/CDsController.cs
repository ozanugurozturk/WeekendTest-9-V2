using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using efCdCollection.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace efCdCollection.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CDsController : ControllerBase //Check all response types according to requested style
    {
        private readonly CdCollectionDbContext _context;

        public CDsController(CdCollectionDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CD>>> GetCDs([FromQuery] string genre ="")
        {
            IQueryable<CD> cds = _context.CDs;
            
            if (!string.IsNullOrEmpty(genre))
            {
                cds = cds.Where(c => c.Genre.Name.ToLower() == genre.ToLower());
            }

            return Ok(await _context.CDs.ToListAsync()); // bad request??
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CD>> GetOneCD(int id)
        {
            var cd = await _context.CDs.Include(c => c.Genre).FirstOrDefaultAsync(c => c.Id == id); //another one???

            if (cd == null) return NotFound();

            return Ok(cd); //bad response ???
        }

        [HttpPut("{id}/artist")]
        public async Task<ActionResult> UpdateArtist(int id, [FromBody] string artistName)
        {
            var cd = await _context.CDs.FindAsync(id);
            if (cd == null) return NotFound();

            cd.ArtistName = artistName;
            await _context.SaveChangesAsync();

            return Ok(); // ok or nocontent?
        }

        [HttpPut("{id}/artist")]
        public async Task<ActionResult> UpdateGenre(int id, [FromBody] string genreName) //??? genre.name?
        {
            var cd = await _context.CDs.FindAsync(id);
            if (cd == null) return NotFound();

            var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Name.ToLower() == genreName.ToLower());
            if (genre == null)
            {
                genre = new Genre {Name = genreName};
                _context.Genres.Add(genre);
                await _context.SaveChangesAsync();
            }

            cd.Genre = genre;
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult<CD>> PostCD(CD cd)
        {
            _context.CDs.Add(cd);
            await _context.SaveChangesAsync(); 
            return CreatedAtAction(nameof(GetOneCD), new {id = cd.Id}, cd);
        }

        [HttpDelete]
        public async Task<ActionResult<CD>> DeleteCD(int id)
        {
            var cd = await _context.CDs.FindAsync(id); 
            if (cd == null) return NotFound();

            _context.CDs.Remove(cd);
            await _context.SaveChangesAsync();
            return Ok();
        }

    }
}
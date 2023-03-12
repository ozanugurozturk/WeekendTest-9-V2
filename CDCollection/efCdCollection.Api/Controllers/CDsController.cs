using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using efCdCollection.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bogus;

namespace efCdCollection.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CDsController : ControllerBase
    {
        private readonly CdCollectionDbContext _context;

        public CDsController(CdCollectionDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CD>>> GetCDs([FromQuery] string? genre ="")
        {
            IQueryable<CD> cds = _context.CDs;
            
            if (string.IsNullOrEmpty(genre)) // 1. durum icin don
            {
                return Ok(cds.Include(c => c.Genre));
            }
            cds = cds.Where(c => c.Genre.Name.ToLower() == genre.ToLower());

            var resultList = await cds.Include(c => c.Genre)
                                    .Select(c=>new CDDto {
                                        Id = c.Id,
                                        Name = c.Name,
                                        Artist = c.ArtistName,
                                        Genre = c.Genre.Name
                                    }).ToListAsync();

            return Ok(resultList);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CD>> GetOneCD(int id)
        {
            var cd = await _context.CDs.Include(c => c.Genre).FirstOrDefaultAsync(c => c.Id == id);

            if (cd == null) return NotFound();

            return Ok(cd);
        }

        [HttpPut("{id}/artist")]
        public async Task<ActionResult> UpdateArtist(int id, [FromBody] string artistName)
        {
            var cd = await _context.CDs.FindAsync(id);
            if (cd == null) return NotFound();

            cd.ArtistName = artistName;
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id}/genre")]
        public async Task<ActionResult> UpdateGenre(int id, [FromBody] string genreName)
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

        [HttpPost]
        [Route("api/SeedData")]
        public async Task<ActionResult> SeedData()
        {
            var genreFaker = new Faker<Genre>()
                            .RuleFor(g => g.Id, f => ((short)f.IndexFaker))
                            .RuleFor(g => g.Name, f => f.Company.CompanyName());

            var genres = genreFaker.Generate(7);

            await _context.Genres.AddRangeAsync(genres);
            await _context.SaveChangesAsync();

            var cdFaker = new Faker<CD>()
                            .RuleFor(c => c.Id, f => f.IndexFaker)
                            .RuleFor(c => c.Name, f => f.Lorem.Sentence())
                            .RuleFor(c => c.ArtistName, f => f.Name.FirstName())
                            .RuleFor(c => c.Description, f => f.Lorem.Paragraph())
                            .RuleFor(c => c.PurchaseDate, f => f.Date.Past(2))
                            .RuleFor(c => c.Genre, f => f.PickRandom(genres));
            
            var cds = cdFaker.Generate(25);

            await _context.CDs.AddRangeAsync(cds);
            await _context.SaveChangesAsync();
            
            return Ok("Data seeded!!!");
        }

    }

    public class CDDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Artist { get; set; }
        public string? Genre { get; set; }
    }
}
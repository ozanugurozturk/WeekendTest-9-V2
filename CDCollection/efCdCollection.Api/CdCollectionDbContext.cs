using Microsoft.EntityFrameworkCore;
using efCdCollection.Api.Models;

namespace efCdCollection.Api
{
    public class CdCollectionDbContext : DbContext
    {
        public CdCollectionDbContext(DbContextOptions<CdCollectionDbContext> options) : base(options)
        {
        }

        public DbSet<CD> CDs { get; set; }
        public DbSet<Genre> Genres { get; set; }
    }
}
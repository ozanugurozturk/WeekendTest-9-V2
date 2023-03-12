using Microsoft.EntityFrameworkCore;
using efCdCollection.Api.Models;

namespace efCdCollection.Api
{
    public class CdCollectionDbContext : DbContext
    {
        public CdCollectionDbContext(DbContextOptions<CdCollectionDbContext> options) : base(options)
        {
        }

        public virtual DbSet<CD> CDs { get; set; }
        public virtual DbSet<Genre> Genres { get; set; }
    }
}
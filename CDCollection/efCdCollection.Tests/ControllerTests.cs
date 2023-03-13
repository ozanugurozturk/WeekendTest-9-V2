using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using efCdCollection.Api.Controllers;
using efCdCollection.Api.Models;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.EntityFrameworkCore;
using efCdCollection.Api;
using System.Linq;
using FluentAssertions;
using Microsoft.Data.Sqlite;


namespace efCdCollection.Tests
{
    public class SqliteControllerTest : ControllerTests
{
    public SqliteControllerTest() 
        : base(new DbContextOptionsBuilder<CdCollectionDbContext>()
            .UseSqlite("Filename=Test.db")
            .Options)
    {
    }
}

    public abstract class ControllerTests
    {
        protected ControllerTests(DbContextOptions<CdCollectionDbContext> contextOptions)
        {
            ContextOptions = contextOptions;
            Seed();
        }

        protected DbContextOptions<CdCollectionDbContext> ContextOptions { get; }
        private void Seed()
        {
            using (var context = new CdCollectionDbContext(ContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                var cd1 = new CD
                {
                    Name = "cd1",
                    ArtistName = "artist1",
                    Description = "This is the description for CD1",
                    PurchaseDate = DateTime.Now,
                    Genre = new Genre { Name = "Rock" }
                };

                var cd2 = new CD
                {
                    Name = "cd2",
                    ArtistName = "artist2",
                    Description = "This is the description for CD2",
                    PurchaseDate = DateTime.Now,
                    Genre = new Genre { Name = "Pop" }
                };

                var cd3 = new CD
                {
                    Name = "cd3",
                    ArtistName = "artist3",
                    Description = "This is the description for CD3",
                    PurchaseDate = DateTime.Now,
                    Genre = new Genre { Name = "Hip-hop" }
                };

                context.AddRange(cd1, cd2, cd3);
                context.SaveChanges();
            }
        }

        [Fact]
        public void GetCDs_WithoutGenre_ReturnsAllCDs()
        {
            using (var context = new CdCollectionDbContext(ContextOptions))
            {
                var controller = new CDsController(context);
                var cds = controller.GetCDs();
                Assert.Equal(3, cds.Result.Value.Count());
                Assert.Equal("cd1", cds.Result.Value.ToList()[0].Name);
                Assert.Equal("artist2", cds.Result.Value.ToList()[1].ArtistName);
                Assert.Equal("This is the description for CD3", cds.Result.Value.ToList()[2].Description);
                Assert.Equal("Pop", cds.Result.Value.ToList()[1].Genre.Name);
            }
        }
    }
}
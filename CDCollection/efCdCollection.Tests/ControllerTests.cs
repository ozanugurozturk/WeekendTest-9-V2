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
        public void Seed()
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
        [Fact]
        public async Task GetOneCD_WithExistingCDId_ReturnsCD()
        {
            using (var context = new CdCollectionDbContext(ContextOptions))
            {
                var controller = new CDsController(context);
                var result = await controller.GetOneCD(1);

                Assert.IsType<OkObjectResult>(result.Result);
                var cd = Assert.IsType<CD>((result.Result as OkObjectResult).Value);
                Assert.Equal("cd1", cd.Name);
            }
        }
        [Fact]
        public async Task GetOneCD_WithNonExistingCDId_ReturnsNotFound()
        {
            using (var context = new CdCollectionDbContext(ContextOptions))
            {
                var controller = new CDsController(context);
                var result = await controller.GetOneCD(99);

                Assert.IsType<NotFoundResult>(result.Result);
            }
        }
        [Fact]
        public async Task UpdateArtist_WithExistingCDId_ReturnsOk()
        {
            using (var context = new CdCollectionDbContext(ContextOptions))
            {
                var controller = new CDsController(context);
                var result = await controller.UpdateArtist(1, "Artist A");

                Assert.IsType<OkResult>(result);
                var cd = await context.CDs.FindAsync(1);
                Assert.Equal("Artist A", cd.ArtistName);
            }
        }
        [Fact]
        public async Task UpdateArtist_WithNonExistingCDId_ReturnsNotFound()
        {
            using (var context = new CdCollectionDbContext(ContextOptions))
            {
                var controller = new CDsController(context);
                var result = await controller.UpdateArtist(99, "Artist B");

                Assert.IsType<NotFoundResult>(result);
            }
        }
        [Fact]
        public async Task UpdateGenre_WithExistingCDIdAndExistingGenreName_ReturnsOk()
        {
            using (var context = new CdCollectionDbContext(ContextOptions))
            {
                var controller = new CDsController(context);
                var result = await controller.UpdateGenre(1, "rock");

                Assert.IsType<OkResult>(result);
                var cd = await context.CDs.FindAsync(1);
                Assert.Equal("rock", cd.Genre.Name.ToLower());
            }
        }
        [Fact]
        public async Task UpdateGenre_WithExistingCDIdAndNonExistingGenreName_ReturnsOk()
        {
            using (var context = new CdCollectionDbContext(ContextOptions))
            {
                var controller = new CDsController(context);
                var result = await controller.UpdateGenre(1, "country");

                Assert.IsType<OkResult>(result);
                var cd = await context.CDs.FindAsync(1);
                Assert.Equal("country", cd.Genre.Name.ToLower());
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using efCdCollection.Api.Controllers;
using efCdCollection.Api.Models;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.EntityFrameworkCore;
using efCdCollection.Api;
using System.Linq;
using FluentAssertions;

namespace efCdCollection.Tests;

public class UnitTests
{
    private readonly CDsController _controller;
    private readonly Mock<CdCollectionDbContext> _mockDbContext;

    public UnitTests()
    {
        _mockDbContext = new Mock<CdCollectionDbContext>(new DbContextOptions<CdCollectionDbContext>());
        _controller = new CDsController(_mockDbContext.Object);
    }

    private static DbSet<T> MockDbSet<T>(List<T> list) where T : class
    {
        var mock = new Mock<DbSet<T>>();
        mock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(list.AsQueryable().Provider);
        mock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(list.AsQueryable().Expression);
        mock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(list.AsQueryable().ElementType);
        mock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(list.GetEnumerator());
        mock.Setup(m => m.Add(It.IsAny<T>())).Callback<T>((s) => list.Add(s));
        mock.Setup(m => m.Remove(It.IsAny<T>())).Callback<T>((s) => list.Remove(s));
        return mock.Object;
    }

    [Fact]
    public async Task GetOneCD_WithExistingCd_ReturnsOk()
    {
        // Arrange
        var expectedCd = new CD
        {
            Id = 1,
            Name = "CD1",
            ArtistName = "Artist1",
            Genre = new Genre { Id = 1, Name = "Rock" }
        };

        _mockDbContext.Setup(x => x.CDs).Returns(MockDbSet(new List<CD> { expectedCd }));

        // Act
        var actionResult = await _controller.GetOneCD(1);
        var result = actionResult.Result as OkObjectResult;
        var actualCd = result?.Value as CD;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(expectedCd, actualCd);
    }

    [Fact]
    public async Task GetOneCD_WithNonExistingCd_ReturnsNotFound()
    {
        // Arrange
        _mockDbContext.Setup(x => x.CDs).Returns(MockDbSet(new List<CD>()));

        // Act
        var actionResult = await _controller.GetOneCD(1);
        var result = actionResult.Result as NotFoundResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
    }
}
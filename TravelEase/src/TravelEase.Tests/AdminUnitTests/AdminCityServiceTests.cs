using Microsoft.EntityFrameworkCore;
using TravelEase.TravelEase.Application.DTOs.Admin;
using TravelEase.TravelEase.Domain.Entities;
using TravelEase.TravelEase.Infrastructure.Data;
using Xunit;

namespace TravelEase.TravelEase.Tests.AdminUnitTests;

public class AdminCityServiceTests
{
    private TravelEaseDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<TravelEaseDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_City_{Guid.NewGuid()}")
            .Options;

        return new TravelEaseDbContext(options);
    }


    [Fact]
    public async Task CreateAsync_ShouldAddCity()
    {
        var db = GetInMemoryDbContext();
        var service = new AdminCityService(db);

        var dto = new AdminCityDto { Name = "Rome", Country = "Italy", PostOffice = "00100" };
        var result = await service.CreateAsync(dto);

        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("Rome", db.Cities.First().Name);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnCities()
    {
        var db = GetInMemoryDbContext();
        db.Cities.Add(new City { Name = "Paris", Country = "France", PostOffice = "75000" });
        db.SaveChanges();

        var service = new AdminCityService(db);
        var result = await service.GetAllAsync();

        Assert.Equal(1, result.Count);
        Assert.Contains(result, c => c.Name == "Paris" && c.Country == "France");
        Assert.Equal("Paris", result[0].Name);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCity()
    {
        var db = GetInMemoryDbContext();
        db.Cities.Add(new City { Id = 1, Name = "Berlin", Country = "Germany", PostOffice = "10115" });
        db.SaveChanges();

        var service = new AdminCityService(db);
        var result = await service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Berlin", result.Name);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateCity()
    {
        var db = GetInMemoryDbContext();
        db.Cities.Add(new City { Id = 1, Name = "Old", Country = "Oldland", PostOffice = "0000" });
        db.SaveChanges();

        var service = new AdminCityService(db);
        var dto = new AdminCityDto { Name = "New", Country = "Newland", PostOffice = "9999" };

        var result = await service.UpdateAsync(1, dto);

        Assert.NotNull(result);
        Assert.Equal("New", db.Cities.First().Name);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveCity()
    {
        var db = GetInMemoryDbContext();
        db.Cities.Add(new City { Id = 1, Name = "ToDelete", Country = "X", PostOffice = "000" });
        db.SaveChanges();

        var service = new AdminCityService(db);
        var result = await service.DeleteAsync(1);

        Assert.True(result);
        Assert.Empty(db.Cities);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ShouldReturnNull()
    {
        var db = GetInMemoryDbContext();
        var service = new AdminCityService(db);
        var dto = new AdminCityDto { Name = "Ghost", Country = "Nowhere", PostOffice = "000" };

        var result = await service.UpdateAsync(999, dto);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse()
    {
        var db = GetInMemoryDbContext();
        var service = new AdminCityService(db);

        var result = await service.DeleteAsync(999);

        Assert.False(result);
    }
    [Fact]
    public async Task GetAllAsync_ShouldBeFast()
    {
        await using var db = GetInMemoryDbContext();
        for (int i = 0; i < 1000; i++)
            db.Cities.Add(new City { Name = $"City{i}", Country = "Test", PostOffice = "12345" });
        db.SaveChanges();

        var service = new AdminCityService(db);

        var sw = System.Diagnostics.Stopwatch.StartNew();
        var result = await service.GetAllAsync();
        sw.Stop();

        Assert.Equal(1000, result.Count);
        Assert.True(sw.ElapsedMilliseconds < 500); 
    }

}
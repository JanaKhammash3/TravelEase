using Microsoft.EntityFrameworkCore;
using TravelEase.TravelEase.Application.DTOs.Admin;
using TravelEase.TravelEase.Domain.Entities;
using TravelEase.TravelEase.Infrastructure.Data;
using TravelEase.TravelEase.Infrastructure.Services.Admin;
using Xunit;

namespace TravelEase.TravelEase.Tests.AdminUnitTests;

public class AdminHotelServiceTests
{
    private TravelEaseDbContext GetInMemoryDbContext_Hotel()
    {
        var options = new DbContextOptionsBuilder<TravelEaseDbContext>()
            .UseInMemoryDatabase(databaseName: $"HotelTestDb_{Guid.NewGuid()}")
            .Options;
        return new TravelEaseDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddHotel()
    {
        await using var db = GetInMemoryDbContext_Hotel();
        db.Cities.Add(new City { Id = 1, Name = "Tokyo", Country = "Japan", PostOffice = "100-0001" });
        db.SaveChanges();

        var service = new AdminHotelService(db);
        var dto = new AdminHotelDto
        {
            Name = "Shinagawa Inn",
            Owner = "Sato",
            StarRating = 4,
            CityName = "Tokyo",
            Location = "Tokyo Center",          // Required
            Amenities = "WiFi, AC"              // Required
        };

        var result = await service.CreateAsync(dto);

        Assert.NotNull(result);
        Assert.Equal("Shinagawa Inn", db.Hotels.First().Name);
    }


    [Fact]
    public async Task GetAllAsync_ShouldReturnHotels()
    {
        await using var db = GetInMemoryDbContext_Hotel();
        db.Cities.Add(new City { Id = 1, Name = "Osaka", Country = "Japan", PostOffice = "530-0001" });
        db.Hotels.Add(new Hotel { Name = "Osaka Grand", CityId = 1, Owner = "Tanaka", Location = "Osaka Street", Amenities = "Spa", StarRating = 5 });
        db.SaveChanges();

        var service = new AdminHotelService(db);
        var result = await service.GetAllAsync();

        Assert.Single(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnHotel()
    {
        await using var db = GetInMemoryDbContext_Hotel();
        var city = new City { Id = 1, Name = "Dubai", Country = "UAE", PostOffice = "00000" };
        db.Cities.Add(city);
        db.Hotels.Add(new Hotel { Id = 1, Name = "Burj", CityId = 1, Owner = "Al Maktoum", Location = "Downtown", Amenities = "Pool, Gym", StarRating = 5 });
        db.SaveChanges();

        var service = new AdminHotelService(db);
        var result = await service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Burj", result.Name);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateHotel()
    {
        await using var db = GetInMemoryDbContext_Hotel();
        db.Cities.Add(new City { Id = 1, Name = "Tokyo", Country = "Japan", PostOffice = "105-0000" });
        db.Hotels.Add(new Hotel { Id = 1, Name = "Old Hotel", Owner = "Mr. A", CityId = 1, StarRating = 3, Location = "Old Street", Amenities = "WiFi" });
        db.SaveChanges();

        var service = new AdminHotelService(db);
        var dto = new AdminHotelDto { Name = "New Hotel", Owner = "Mr. B", StarRating = 5, CityName = "Tokyo", Location = "New Location", Amenities = "All" };

        var result = await service.UpdateAsync(1, dto);

        Assert.Equal("New Hotel", db.Hotels.First().Name);
        Assert.Equal(5, db.Hotels.First().StarRating);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveHotel()
    {
        await using var db = GetInMemoryDbContext_Hotel();
        db.Cities.Add(new City { Id = 1, Name = "NYC", Country = "USA", PostOffice = "10001" });
        db.Hotels.Add(new Hotel { Id = 1, Name = "ToRemove", CityId = 1, Owner = "XYZ", Location = "5th Ave", Amenities = "TV" });
        db.SaveChanges();

        var service = new AdminHotelService(db);
        var result = await service.DeleteAsync(1);

        Assert.True(result);
        Assert.Empty(db.Hotels);
    }

    [Fact]
    public async Task GetAllAsync_WithNoMatches_ShouldReturnEmpty()
    {
        await using var db = GetInMemoryDbContext_Hotel();
        db.Cities.Add(new City { Id = 1, Name = "Exist", Country = "Nowhere", PostOffice = "0000" });
        db.Hotels.Add(new Hotel { Name = "Actual", CityId = 1, Owner = "Real", Location = "Exist St", Amenities = "TV" });
        db.SaveChanges();

        var service = new AdminHotelService(db);
        var result = await service.GetAllAsync(name: "Ghost");

        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateAsync_WithMaxStarRating_ShouldWork()
    {
        await using var db = GetInMemoryDbContext_Hotel();
        db.Cities.Add(new City { Id = 1, Name = "Starsville", Country = "Skyland", PostOffice = "9999" });
        db.SaveChanges();

        var service = new AdminHotelService(db);
        var dto = new AdminHotelDto
        {
            Name = "MaxStar",
            CityName = "Starsville",
            Owner = "Top",
            StarRating = 5,
            Location = "Peak",                 // Required
            Amenities = "Everything"          // Required
        };

        var result = await service.CreateAsync(dto);

        Assert.Equal(5, db.Hotels.First().StarRating);
    }


    [Fact]
    public async Task GetAllAsync_WithEmptyDb_ShouldReturnEmpty()
    {
        await using var db = GetInMemoryDbContext_Hotel();
        var service = new AdminHotelService(db);

        var result = await service.GetAllAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldHandleLargeDatasetQuickly()
    {
        await using var db = GetInMemoryDbContext_Hotel();
        db.Cities.Add(new City { Id = 1, Name = "BulkCity", Country = "Speedland", PostOffice = "1111" });

        for (int i = 0; i < 1000; i++)
            db.Hotels.Add(new Hotel { Name = $"Hotel{i}", CityId = 1, StarRating = 3, Owner = "BulkOwner", Location = "Zone", Amenities = "AC" });

        db.SaveChanges();

        var service = new AdminHotelService(db);
        var sw = System.Diagnostics.Stopwatch.StartNew();

        var result = await service.GetAllAsync();

        sw.Stop();
        Assert.Equal(1000, result.Count);
        Assert.True(sw.ElapsedMilliseconds < 500);
    }

    [Fact]
    public async Task UpdateAsync_InvalidId_ShouldReturnNull()
    {
        await using var db = GetInMemoryDbContext_Hotel();
        db.Cities.Add(new City { Id = 1, Name = "TestCity", Country = "Land", PostOffice = "8888" });
        db.SaveChanges();

        var service = new AdminHotelService(db);
        var result = await service.UpdateAsync(999, new AdminHotelDto
        {
            Name = "X",
            Owner = "Y",
            StarRating = 2,
            CityName = "TestCity",
            Location = "Void",
            Amenities = "None"
        });

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_InvalidId_ShouldReturnFalse()
    {
        await using var db = GetInMemoryDbContext_Hotel();
        var service = new AdminHotelService(db);

        var result = await service.DeleteAsync(999);

        Assert.False(result);
    }
}

using Microsoft.EntityFrameworkCore;
using TravelEase.TravelEase.Application.DTOs.Admin;
using TravelEase.TravelEase.Domain.Entities;
using TravelEase.TravelEase.Infrastructure.Data;
using Xunit;

namespace TravelEase.TravelEase.Tests.AdminUnitTests;

public class AdminRoomServiceTests
{
    private TravelEaseDbContext GetInMemoryDbContext_Room()
    {
        var options = new DbContextOptionsBuilder<TravelEaseDbContext>()
            .UseInMemoryDatabase(databaseName: $"RoomDb_{Guid.NewGuid()}")
            .Options;
        return new TravelEaseDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddRoom()
    {
        await using var db = GetInMemoryDbContext_Room();
        db.Hotels.Add(new Hotel { Id = 1, Name = "Hilton", Owner = "Hilton Corp", Location = "City Center", Amenities = "WiFi, Gym" });
        db.SaveChanges();

        var service = new AdminRoomService(db);
        var dto = new AdminRoomDto
        {
            Number = "101",
            CapacityAdults = 2,
            CapacityChildren = 1,
            HotelName = "Hilton"
        };

        var result = await service.CreateAsync(dto);

        Assert.NotNull(result);
        Assert.Equal("101", db.Rooms.First().Number);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnRooms()
    {
        await using var db = GetInMemoryDbContext_Room();
        var hotel = new Hotel { Id = 1, Name = "Sheraton", Owner = "Sheraton Group", Location = "Downtown", Amenities = "Pool, Restaurant" };
        db.Hotels.Add(hotel);
        db.Rooms.Add(new Room { Number = "202", HotelId = 1, CapacityAdults = 2, CapacityChildren = 0 });
        db.SaveChanges();

        var service = new AdminRoomService(db);
        var result = await service.GetAllAsync();

        Assert.Single(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnRoom()
    {
        await using var db = GetInMemoryDbContext_Room();
        db.Hotels.Add(new Hotel { Id = 1, Name = "Westin", Owner = "Westin Inc", Location = "Beachfront", Amenities = "Spa, Bar" });
        db.Rooms.Add(new Room { Id = 1, Number = "501", HotelId = 1, CapacityAdults = 2, CapacityChildren = 1 });
        db.SaveChanges();

        var service = new AdminRoomService(db);
        var result = await service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("501", result.Number);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateRoom()
    {
        await using var db = GetInMemoryDbContext_Room();
        db.Hotels.Add(new Hotel { Id = 1, Name = "Marriott", Owner = "Marriott Int.", Location = "Main Blvd", Amenities = "Breakfast, Parking" });
        db.Rooms.Add(new Room { Id = 1, Number = "201", HotelId = 1, CapacityAdults = 1, CapacityChildren = 0 });
        db.SaveChanges();

        var service = new AdminRoomService(db);
        var dto = new AdminRoomDto
        {
            Number = "202",
            CapacityAdults = 3,
            CapacityChildren = 2,
            HotelName = "Marriott"
        };

        var result = await service.UpdateAsync(1, dto);

        Assert.Equal("202", db.Rooms.First().Number);
        Assert.Equal(3, db.Rooms.First().CapacityAdults);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveRoom()
    {
        await using var db = GetInMemoryDbContext_Room();
        db.Hotels.Add(new Hotel { Id = 1, Name = "TestHotel", Owner = "TestOwner", Location = "Anywhere", Amenities = "Nothing" });
        db.Rooms.Add(new Room { Id = 1, Number = "404", HotelId = 1 });
        db.SaveChanges();

        var service = new AdminRoomService(db);
        var result = await service.DeleteAsync(1);

        Assert.True(result);
        Assert.Empty(db.Rooms);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ShouldReturnNull()
    {
        await using var db = GetInMemoryDbContext_Room();
        var service = new AdminRoomService(db);
        var dto = new AdminRoomDto { Number = "999", CapacityAdults = 1, CapacityChildren = 0, HotelName = "Ghost" };

        var result = await service.UpdateAsync(999, dto);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse()
    {
        await using var db = GetInMemoryDbContext_Room();
        var service = new AdminRoomService(db);

        var result = await service.DeleteAsync(999);

        Assert.False(result);
    }

    [Fact]
    public async Task CreateAsync_WithNonExistentHotel_ShouldThrow()
    {
        await using var db = GetInMemoryDbContext_Room();
        var service = new AdminRoomService(db);
        var dto = new AdminRoomDto { Number = "X999", CapacityAdults = 2, CapacityChildren = 1, HotelName = "Nowhere" };

        await Assert.ThrowsAsync<Exception>(() => service.CreateAsync(dto));
    }

    [Fact]
    public async Task GetAllAsync_WithNoMatchingHotel_ShouldReturnEmpty()
    {
        await using var db = GetInMemoryDbContext_Room();
        db.Hotels.Add(new Hotel { Id = 1, Name = "RealHotel", Owner = "RealOwner", Location = "Real Street", Amenities = "Everything" });
        db.Rooms.Add(new Room { Number = "R1", HotelId = 1, CapacityAdults = 2, CapacityChildren = 1 });
        db.SaveChanges();

        var service = new AdminRoomService(db);
        var result = await service.GetAllAsync(hotelName: "FakeHotel");

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_WithEmptyDb_ShouldReturnEmpty()
    {
        await using var db = GetInMemoryDbContext_Room(); 
        var service = new AdminRoomService(db); 

        var result = await service.GetAllAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        await using var db = GetInMemoryDbContext_Room();
        var service = new AdminRoomService(db);

        var result = await service.GetByIdAsync(999);

        Assert.Null(result);
    }
}

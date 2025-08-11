using Moq;
using TravelEase.TravelEase.Application.Features.Room;
using TravelEase.TravelEase.Application.Interfaces;
using TravelEase.TravelEase.Domain.Entities;
using TravelEase.TravelEase.Domain.Enums;
using Xunit;

namespace TravelEase.TravelEase.Tests.UserUnitTests;

public class RoomServiceTests
{
    private readonly Mock<IRoomRepository> _roomRepoMock;
    private readonly RoomService _service;

    public RoomServiceTests()
    {
        _roomRepoMock = new Mock<IRoomRepository>();
        _service = new RoomService(_roomRepoMock.Object);
    }

    [Fact(DisplayName = "Create room")]
    public async Task CreateRoomAsync_ShouldAddRoom()
    {
        var cmd = new CreateRoomCommand
        {
            Number = "101",
            CapacityAdults = 2,
            CapacityChildren = 1,
            PricePerNight = 150.00m,
            Category = RoomCategory.Deluxe, 
            HotelId = 1
        };

        _roomRepoMock.Setup(r => r.AddRoomAsync(It.IsAny<Room>())).Returns(Task.CompletedTask);

        await _service.CreateRoomAsync(cmd);

        _roomRepoMock.Verify(r => r.AddRoomAsync(It.Is<Room>(room =>
            room.Number == "101" &&
            room.CapacityAdults == 2 &&
            room.CapacityChildren == 1 &&
            room.PricePerNight == 150.00m &&
            room.Category == RoomCategory.Deluxe && 
            room.HotelId == 1
        )), Times.Once);
    }


    [Fact(DisplayName = "Get all rooms")]
    public async Task GetAllRoomsAsync_ShouldReturnList()
    {
        _roomRepoMock.Setup(r => r.GetAllRoomsAsync()).ReturnsAsync(new List<Room> { new(), new() });

        var result = await _service.GetAllRoomsAsync();

        Assert.Equal(2, result.Count);
    }

    [Fact(DisplayName = "Get room by ID")]
    public async Task GetRoomByIdAsync_ShouldReturnRoom()
    {
        var room = new Room { Id = 7, Number = "207" };
        _roomRepoMock.Setup(r => r.GetRoomByIdAsync(7)).ReturnsAsync(room);

        var result = await _service.GetRoomByIdAsync(7);

        Assert.NotNull(result);
        Assert.Equal("207", result.Number);
    }

    [Fact(DisplayName = "Delete room when found")]
    public async Task DeleteRoomAsync_ShouldDeleteRoom()
    {
        var room = new Room { Id = 3 };
        _roomRepoMock.Setup(r => r.GetRoomByIdAsync(3)).ReturnsAsync(room);

        await _service.DeleteRoomAsync(3);

        _roomRepoMock.Verify(r => r.DeleteRoomAsync(room), Times.Once);
    }

    [Fact(DisplayName = "Delete room when not found does nothing")]
    public async Task DeleteRoomAsync_ShouldNotFail_WhenRoomNotFound()
    {
        _roomRepoMock.Setup(r => r.GetRoomByIdAsync(999)).ReturnsAsync((Room)null);

        await _service.DeleteRoomAsync(999);

        _roomRepoMock.Verify(r => r.DeleteRoomAsync(It.IsAny<Room>()), Times.Never);
    }

    [Fact(DisplayName = "Update room when found")]
    public async Task UpdateRoomAsync_ShouldUpdateRoom()
    {
        var room = new Room { Id = 5 };
        var cmd = new UpdateRoomCommand
        {
            Id = 5,
            Number = "305",
            CapacityAdults = 3,
            CapacityChildren = 2,
            PricePerNight = 200,
            Category = (int)RoomCategory.Suite, // explicit cast from enum
            HotelId = 2
        };

        _roomRepoMock.Setup(r => r.GetRoomByIdAsync(5)).ReturnsAsync(room);
        _roomRepoMock.Setup(r => r.UpdateRoomAsync(room)).Returns(Task.CompletedTask);

        await _service.UpdateRoomAsync(cmd);

        Assert.Equal("305", room.Number);
        Assert.Equal(RoomCategory.Suite, room.Category);
        _roomRepoMock.Verify(r => r.UpdateRoomAsync(room), Times.Once);
    }

    [Fact(DisplayName = "Update room when not found should throw")]
    public async Task UpdateRoomAsync_ShouldThrow_WhenNotFound()
    {
        var cmd = new UpdateRoomCommand
        {
            Id = 999,
            Number = "X",
            CapacityAdults = 0,
            CapacityChildren = 0,
            PricePerNight = 0,
            Category = (int)RoomCategory.Standard,
            HotelId = 1
        };

        _roomRepoMock.Setup(r => r.GetRoomByIdAsync(999)).ReturnsAsync((Room)null);

        var ex = await Assert.ThrowsAsync<Exception>(() => _service.UpdateRoomAsync(cmd));

        Assert.Equal("Room not found", ex.Message);
    }

    [Fact(DisplayName = "Search rooms returns filtered result")]
    public async Task SearchRoomsAsync_ShouldReturnMatchingRooms()
    {
        var query = new SearchRoomsQuery { HotelId = 1, MinPrice = 100, MaxPrice = 300 };
        var resultList = new List<Room> { new Room { Id = 1 } };

        _roomRepoMock.Setup(r => r.SearchRoomsAsync(query)).ReturnsAsync(resultList);

        var result = await _service.SearchRoomsAsync(query);

        Assert.Single(result);
    }
}

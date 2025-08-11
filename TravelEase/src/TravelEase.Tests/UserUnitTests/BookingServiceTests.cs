using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using TravelEase.TravelEase.Application.DTOs;
using TravelEase.TravelEase.Application.Features.Booking;
using TravelEase.TravelEase.Application.Interfaces;
using TravelEase.TravelEase.Domain.Entities;
using Xunit;

namespace TravelEase.TravelEase.Tests.UserUnitTests;

public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _bookingRepoMock;
    private readonly BookingService _service;

    public BookingServiceTests()
    {
        _bookingRepoMock = new Mock<IBookingRepository>();
        _service = new BookingService(_bookingRepoMock.Object);
    }

    [Fact(DisplayName = "Create booking when room is available")]
    public async Task CreateAsync_ShouldCreateBooking_WhenRoomIsAvailable()
    {
        var command = new CreateBookingCommand
        {
            UserId = 1,
            RoomId = 101,
            CheckIn = DateTime.Today,
            CheckOut = DateTime.Today.AddDays(2),
            Adults = 2,
            Children = 1,
            SpecialRequests = "Sea view"
        };

        _bookingRepoMock.Setup(r => r.IsRoomAvailableAsync(command.RoomId, command.CheckIn, command.CheckOut))
            .ReturnsAsync(true);

        _bookingRepoMock.Setup(r => r.BeginSerializableTransactionAsync())
            .ReturnsAsync(Mock.Of<IDbContextTransaction>());

        _bookingRepoMock.Setup(r => r.AddAsync(It.IsAny<Booking>()))
            .Returns(Task.CompletedTask);

        await _service.CreateAsync(command);

        _bookingRepoMock.Verify(r => r.AddAsync(It.IsAny<Booking>()), Times.Once);
    }

    [Fact(DisplayName = "Create booking when room is not available should throw")]
    public async Task CreateAsync_ShouldThrow_WhenRoomIsNotAvailable()
    {
        var command = new CreateBookingCommand
        {
            RoomId = 101,
            CheckIn = DateTime.Today,
            CheckOut = DateTime.Today.AddDays(3),
            Adults = 2,
            Children = 0,
            SpecialRequests = "None"
        };

        _bookingRepoMock.Setup(r => r.IsRoomAvailableAsync(command.RoomId, command.CheckIn, command.CheckOut))
            .ReturnsAsync(false);

        var ex = await Assert.ThrowsAsync<Exception>(() => _service.CreateAsync(command));

        Assert.Contains("Room is not available", ex.Message);
    }



    [Fact(DisplayName = " Get all bookings")]
    public async Task GetAllAsync_ShouldReturnBookings()
    {
        var bookings = new List<BookingDto>
        {
            new() { Id = 1, RoomNumber = "101" },
            new() { Id = 2, RoomNumber = "102" }
        };

        _bookingRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(bookings);

        var result = await _service.GetAllAsync();

        Assert.Equal(2, result.Count);
        Assert.All(result, b => Assert.IsType<BookingDto>(b));
    }

    [Fact(DisplayName = "Get booking by ID")]
    public async Task GetByIdAsync_ShouldReturnBooking_WhenExists()
    {
        var bookingDto = new BookingDto { Id = 5, RoomNumber = "203" };

        _bookingRepoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(bookingDto);

        var result = await _service.GetByIdAsync(5);

        Assert.NotNull(result);
        Assert.Equal("203", result.RoomNumber);
    }

    [Fact(DisplayName = "Delete booking if exists")]
    public async Task DeleteAsync_ShouldCallRepo_WhenBookingExists()
    {
        var booking = new Booking { Id = 3 };
        _bookingRepoMock.Setup(r => r.GetBookingEntityByIdAsync(3)).ReturnsAsync(booking);

        await _service.DeleteAsync(3);

        _bookingRepoMock.Verify(r => r.DeleteAsync(booking), Times.Once);
    }

    [Fact(DisplayName = " Delete booking when not found should do nothing")]
    public async Task DeleteAsync_ShouldNotFail_WhenBookingDoesNotExist()
    {
        _bookingRepoMock.Setup(r => r.GetBookingEntityByIdAsync(99)).ReturnsAsync((Booking)null);

        await _service.DeleteAsync(99);

        _bookingRepoMock.Verify(r => r.DeleteAsync(It.IsAny<Booking>()), Times.Never);
    }

    [Fact(DisplayName = " Update booking when it exists")]
    public async Task UpdateAsync_ShouldUpdate_WhenBookingExists()
    {
        var existing = new Booking
        {
            Id = 1,
            CheckIn = DateTime.Today,
            CheckOut = DateTime.Today.AddDays(2),
            SpecialRequests = "Old",
            Adults = 2,
            Children = 1
        };

        var updateCmd = new UpdateBookingCommand
        {
            Id = 1,
            CheckIn = DateTime.Today.AddDays(1),
            CheckOut = DateTime.Today.AddDays(3),
            SpecialRequests = "Updated",
            Adults = 3,
            Children = 0
        };

        _bookingRepoMock.Setup(r => r.GetBookingEntityByIdAsync(1)).ReturnsAsync(existing);
        _bookingRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Booking>())).Returns(Task.CompletedTask);

        await _service.UpdateAsync(updateCmd);

        Assert.Equal(updateCmd.CheckIn, existing.CheckIn);
        Assert.Equal(updateCmd.CheckOut, existing.CheckOut);
        Assert.Equal(updateCmd.SpecialRequests, existing.SpecialRequests);
        Assert.Equal(updateCmd.Adults, existing.Adults);
        Assert.Equal(updateCmd.Children, existing.Children);
    }

    [Fact(DisplayName = " Update booking when not found should do nothing")]
    public async Task UpdateAsync_ShouldDoNothing_WhenBookingDoesNotExist()
    {
        var updateCmd = new UpdateBookingCommand
        {
            Id = 123,
            CheckIn = DateTime.Today,
            CheckOut = DateTime.Today.AddDays(1),
            SpecialRequests = "None",
            Adults = 1,
            Children = 1
        };

        _bookingRepoMock.Setup(r => r.GetBookingEntityByIdAsync(123)).ReturnsAsync((Booking)null);

        await _service.UpdateAsync(updateCmd);

        _bookingRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Booking>()), Times.Never);
    }

    [Fact(DisplayName = " Search bookings should return result")]
    public async Task SearchBookingsAsync_ShouldReturnList()
    {
        var query = new SearchBookingsQuery { UserId = 1 };

        var data = new List<BookingDto> { new() { Id = 1, UserEmail = "test@example.com" } };

        _bookingRepoMock.Setup(r => r.SearchAsync(query)).ReturnsAsync(data);

        var result = await _service.SearchBookingsAsync(query);

        Assert.Single(result);
        Assert.Equal("test@example.com", result[0].UserEmail);
    }
}

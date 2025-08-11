using Moq;
using TravelEase.TravelEase.Application.DTOs;
using TravelEase.TravelEase.Application.Features.Review;
using TravelEase.TravelEase.Application.Interfaces;
using TravelEase.TravelEase.Domain.Entities;
using Xunit;

namespace TravelEase.TravelEase.Tests.UserUnitTests;

public class ReviewServiceTests
{
    private readonly Mock<IReviewRepository> _reviewRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly ReviewService _service;

    public ReviewServiceTests()
    {
        _reviewRepoMock = new Mock<IReviewRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _service = new ReviewService(_reviewRepoMock.Object, _userRepoMock.Object);
    }

    [Fact(DisplayName = "Create review")]
    public async Task CreateReviewAsync_ShouldAddReview()
    {
        var dto = new ReviewDto
        {
            HotelId = 1,
            UserId = 2,
            Rating = 5,
            Comment = "Excellent!"
        };

        _reviewRepoMock.Setup(r => r.AddAsync(It.IsAny<Review>())).Returns(Task.CompletedTask);

        await _service.CreateReviewAsync(dto);

        _reviewRepoMock.Verify(r => r.AddAsync(It.Is<Review>(r =>
            r.HotelId == 1 && r.UserId == 2 && r.Rating == 5 && r.Comment == "Excellent!"
        )), Times.Once);
    }

    [Fact(DisplayName = "Get reviews by hotel ID")]
    public async Task GetReviewsByHotelIdAsync_ShouldReturnList()
    {
        var reviews = new List<Review>
        {
            new() { Rating = 4, Comment = "Nice!", CreatedAt = DateTime.UtcNow, User = new User { FullName = "Alice" } },
            new() { Rating = 5, Comment = "Great!", CreatedAt = DateTime.UtcNow, User = new User { FullName = "Bob" } }
        };

        _reviewRepoMock.Setup(r => r.GetByHotelIdAsync(10)).ReturnsAsync(reviews);

        var result = await _service.GetReviewsByHotelIdAsync(10);

        Assert.Equal(2, result.Count);
        Assert.Equal("Alice", result[0].UserName);
        Assert.Equal("Bob", result[1].UserName);
    }

    [Fact(DisplayName = "Delete review")]
    public async Task DeleteReviewAsync_ShouldCallRepo()
    {
        await _service.DeleteReviewAsync(99);

        _reviewRepoMock.Verify(r => r.DeleteAsync(99), Times.Once);
    }
}

using Microsoft.AspNetCore.Mvc;
using Moq;
using TravelEase.TravelEase.API.Controllers;
using TravelEase.TravelEase.API.Models;
using TravelEase.TravelEase.Application.DTOs;
using TravelEase.TravelEase.Application.Interfaces;
using Xunit;

namespace TravelEase.TravelEase.Tests.UserUnitTests;

public class PaymentControllerTests
{
    [Fact(DisplayName = "✅ CreateCheckoutSession returns Stripe session DTO")]
    public void CreateCheckoutSession_ShouldReturnValidSessionDto()
    {
        // Arrange
        var mockStripeService = new Mock<IStripeSessionService>();
        mockStripeService.Setup(s => s.CreateCheckoutSession(It.IsAny<object>()))
            .Returns(("cs_test_123", "https://stripe.com/checkout/test"));

        var controller = new PaymentController(mockStripeService.Object);

        var dto = new StripeCheckoutRequestDto
        {
            UserId = 1,
            RoomId = 101,
            CheckIn = DateTime.Today,
            CheckOut = DateTime.Today.AddDays(2),
            Adults = 2,
            Children = 1,
            SpecialRequests = "High floor",
            SuccessUrl = "https://example.com/success",
            CancelUrl = "https://example.com/cancel"
        };

        // Act
        var result = controller.CreateCheckoutSession(dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<StripeCheckoutResponseDto>(okResult.Value);

        Assert.Equal("cs_test_123", value.SessionId);
        Assert.Equal("https://stripe.com/checkout/test", value.Url);

        mockStripeService.Verify(s => s.CreateCheckoutSession(It.IsAny<object>()), Times.Once);
    }
}
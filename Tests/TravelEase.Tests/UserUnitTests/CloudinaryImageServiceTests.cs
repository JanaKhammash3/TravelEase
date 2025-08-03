using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Moq;
using TravelEase.Application.Interfaces;
using TravelEase.Infrastructure.Services;
using TravelEase.TravelEase.Application.Interfaces;

namespace TravelEase.Tests.UserUnitTests
{
    public class CloudinaryImageServiceTests
    {
        [Fact(DisplayName = "UploadImageAsync returns mocked secure URL")]
        public async Task UploadImageAsync_ShouldReturnSecureUrl()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var stream = new MemoryStream(new byte[] { 1, 2, 3 });

            fileMock.Setup(f => f.FileName).Returns("test.jpg");
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);

            var wrapperMock = new Mock<ICloudinaryWrapper>();
            wrapperMock
                .Setup(w => w.UploadImageAsync(It.IsAny<ImageUploadParams>()))
                .ReturnsAsync(new ImageUploadResult
                {
                    SecureUrl = new Uri("https://mocked.com/test.jpg")
                });

            var service = new CloudinaryImageService(wrapperMock.Object);

            // Act
            var result = await service.UploadImageAsync(fileMock.Object);

            // Assert
            Assert.Equal("https://mocked.com/test.jpg", result);
        }
    }
}
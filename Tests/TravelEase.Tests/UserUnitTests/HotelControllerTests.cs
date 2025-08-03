using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TravelEase.API.Controllers;
using TravelEase.API.Models;
using TravelEase.Application.Interfaces;

namespace TravelEase.Tests.UserUnitTests;

public class HotelControllerTests
{
    [Fact(DisplayName = "UploadImages uploads files and returns uploaded URLs")]
    public async Task UploadImages_ShouldUploadFilesAndReturnUrls()
    {
        // Arrange
        var mockHotelService = new Mock<IHotelService>();
        var mockUploader = new Mock<IImageUploader>(); // Use the interface here

        var fakeUrl = "https://cloudinary.com/hotel/test.jpg";
        mockUploader.Setup(x => x.UploadImageAsync(It.IsAny<IFormFile>()))
                    .ReturnsAsync(fakeUrl);

        var controller = new HotelController(mockHotelService.Object, null);
        var dto = new HotelImageUploadDto
        {
            HotelId = 1,
            Images = new List<IFormFile>
            {
                CreateFakeFormFile("image1.jpg"),
                CreateFakeFormFile("image2.jpg")
            }
        };

        // Act
        var result = await controller.UploadImages(dto.HotelId, dto, mockUploader.Object);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var uploadedUrlsProperty = okResult.Value!.GetType().GetProperty("UploadedUrls");
        Assert.NotNull(uploadedUrlsProperty);

        var urls = uploadedUrlsProperty!.GetValue(okResult.Value) as List<string>;
        Assert.NotNull(urls);
        Assert.Equal(2, urls.Count);
        Assert.All(urls, url => Assert.Equal(fakeUrl, url));

        mockUploader.Verify(x => x.UploadImageAsync(It.IsAny<IFormFile>()), Times.Exactly(2));
        mockHotelService.Verify(x => x.SaveHotelImageUrlsAsync(1, It.IsAny<List<string>>()), Times.Once);
    }

    private IFormFile CreateFakeFormFile(string fileName)
    {
        var bytes = Encoding.UTF8.GetBytes("fake image content");
        var stream = new MemoryStream(bytes);
        return new FormFile(stream, 0, bytes.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/jpeg"
        };
    }
}

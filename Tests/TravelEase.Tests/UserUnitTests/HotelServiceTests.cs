using Moq;
using TravelEase.TravelEase.Application.DTOs;
using TravelEase.TravelEase.Application.Features.Hotel;
using TravelEase.TravelEase.Application.Interfaces;
using TravelEase.TravelEase.Domain.Entities;

namespace TravelEase.Tests.UserUnitTests
{
    public class HotelServiceTests
    {
        private readonly Mock<IHotelRepository> _hotelRepoMock;
        private readonly HotelService _service;

        public HotelServiceTests()
        {
            _hotelRepoMock = new Mock<IHotelRepository>();
            _service = new HotelService(_hotelRepoMock.Object);
        }

        [Fact(DisplayName = "Create hotel")]
        public async Task CreateHotelAsync_ShouldAddHotel()
        {
            var cmd = new CreateHotelCommand
            {
                Name = "Hotel Lux",
                CityId = 1,
                Owner = "John Doe",
                Location = "Center",
                StarRating = 5,
                Description = "Luxury stay"
            };

            await _service.CreateHotelAsync(cmd);

            _hotelRepoMock.Verify(r => r.AddAsync(It.Is<Hotel>(h =>
                h.Name == "Hotel Lux" && h.CityId == 1 && h.Owner == "John Doe")), Times.Once);
        }

        [Fact(DisplayName = "Update hotel")]
        public async Task UpdateHotelAsync_ShouldUpdate_WhenExists()
        {
            var hotel = new Hotel { Id = 1 };
            var cmd = new UpdateHotelCommand
            {
                Id = 1,
                Name = "Updated",
                CityId = 2,
                Owner = "New Owner",
                Location = "Downtown",
                StarRating = 4,
                Description = "Updated Desc"
            };

            _hotelRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(hotel);

            await _service.UpdateHotelAsync(cmd);

            Assert.Equal("Updated", hotel.Name);
            _hotelRepoMock.Verify(r => r.UpdateAsync(hotel), Times.Once);
        }

        [Fact(DisplayName = "Delete hotel")]
        public async Task DeleteHotelAsync_ShouldDelete_WhenExists()
        {
            var hotel = new Hotel { Id = 1 };
            _hotelRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(hotel);

            await _service.DeleteHotelAsync(1);

            _hotelRepoMock.Verify(r => r.DeleteAsync(hotel), Times.Once);
        }

        [Fact(DisplayName = "Get all hotels with pagination")]
        public async Task GetAllHotelsAsync_ShouldReturnPagedList()
        {
            _hotelRepoMock.Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                          .ReturnsAsync(new List<Hotel> { new Hotel(), new Hotel() });

            var result = await _service.GetAllHotelsAsync(1, 20);

            Assert.Equal(2, result.Count);
            _hotelRepoMock.Verify(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        [Fact(DisplayName = "Get hotel by ID")]
        public async Task GetHotelByIdAsync_ShouldReturnHotel()
        {
            var hotel = new Hotel { Id = 5 };
            _hotelRepoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(hotel);

            var result = await _service.GetHotelDtoByIdAsync(5);

            Assert.NotNull(result);
        }

        [Fact(DisplayName = "Search hotels")]
        public async Task SearchHotelsAsync_ShouldReturnPagedList()
        {
            var query = new SearchHotelsQuery { Name = "test", Page = 1, PageSize = 10 };

            _hotelRepoMock.Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                          .ReturnsAsync(new List<Hotel>
                          {
                              new Hotel { Name = "Test Hotel", StarRating = 4, City = new City { Name = "Paris" }, Rooms = new List<Room> { new() { PricePerNight = 100 } } }
                          });

            var result = await _service.SearchHotelsAsync(query);

            Assert.Single(result);
            Assert.Contains("Test", result.First().Name, StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "Get featured hotels")]
        public async Task GetFeaturedHotelsAsync_ShouldReturnList()
        {
            _hotelRepoMock.Setup(r => r.GetFeaturedHotelsAsync()).ReturnsAsync(new List<Hotel> { new Hotel() });

            var result = await _service.GetFeaturedHotelsAsync();

            Assert.Single(result);
        }

        [Fact(DisplayName = "Record hotel view")]
        public async Task RecordHotelViewAsync_ShouldCallRepository()
        {
            await _service.RecordHotelViewAsync(1, 2);
            _hotelRepoMock.Verify(r => r.RecordHotelViewAsync(1, 2), Times.Once);
        }

        [Fact(DisplayName = "Get recent hotels")]
        public async Task GetRecentlyVisitedHotelsAsync_ShouldReturnRecent()
        {
            _hotelRepoMock.Setup(r => r.GetRecentlyVisitedHotelsAsync(1, 5)).ReturnsAsync(new List<HotelDto> { new() });

            var result = await _service.GetRecentlyVisitedHotelsAsync(1);

            Assert.Single(result);
        }

        [Fact(DisplayName = "Get trending cities")]
        public async Task GetTrendingCitiesAsync_ShouldReturnList()
        {
            _hotelRepoMock.Setup(r => r.GetTrendingCitiesAsync(5)).ReturnsAsync(new List<TrendingCityDto> { new() });

            var result = await _service.GetTrendingCitiesAsync();

            Assert.Single(result);
        }

        [Fact(DisplayName = "Upload hotel images")]
        public async Task UploadImagesAsync_ShouldSaveFilesAndReturnUrls()
        {
            var hotel = new Hotel { Id = 1, Images = new List<HotelImage>() };
            _hotelRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(hotel);
            _hotelRepoMock.Setup(r => r.SaveHotelImageUrlsAsync(1, It.IsAny<List<string>>())).Returns(Task.CompletedTask);

            var mockStream = new MemoryStream(new byte[] { 0x1, 0x2, 0x3 });
            var files = new List<(string FileName, Stream Content)>
            {
                ("test.jpg", mockStream)
            };

            var result = await _service.UploadImagesAsync(1, files);

            Assert.Single(result);
            _hotelRepoMock.Verify(r => r.SaveHotelImageUrlsAsync(1, It.IsAny<List<string>>()), Times.Once);
        }

        [Fact(DisplayName = "Upload hotel images throws if hotel not found")]
        public async Task UploadImagesAsync_ShouldThrowIfHotelNotFound()
        {
            _hotelRepoMock.Setup(r => r.GetByIdAsync(123)).ReturnsAsync((Hotel)null!);

            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.UploadImagesAsync(123, new List<(string, Stream)>()));

            Assert.Equal("Hotel not found", ex.Message);
        }

        [Fact(DisplayName = "Save hotel image URLs")]
        public async Task SaveHotelImageUrlsAsync_ShouldCallRepo()
        {
            var urls = new List<string> { "/img1.jpg" };
            await _service.SaveHotelImageUrlsAsync(1, urls);
            _hotelRepoMock.Verify(r => r.SaveHotelImageUrlsAsync(1, urls), Times.Once);
        }
    }
}

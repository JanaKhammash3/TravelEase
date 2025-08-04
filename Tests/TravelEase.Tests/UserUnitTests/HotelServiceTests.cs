using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelEase.TravelEase.Application.DTOs;
using TravelEase.TravelEase.Application.Features.Hotel;
using TravelEase.TravelEase.Application.Interfaces;
using TravelEase.TravelEase.Domain.Entities;
using Xunit;

namespace TravelEase.TravelEase.Tests.UserUnitTests
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

        [Fact(DisplayName = "Get all hotels returns mapped DTOs")]
        public async Task GetAllHotelsAsync_ShouldReturnHotelDtos()
        {
            // Arrange
            var hotels = new List<Hotel>
            {
                new Hotel
                {
                    Id = 1,
                    Name = "Test Hotel",
                    City = new City { Name = "Paris" },
                    StarRating = 5,
                    Rooms = new List<Room> { new Room { PricePerNight = 100 } },
                    Images = new List<HotelImage> { new HotelImage { ImageUrl = "thumb.jpg" } }
                }
            };

            _hotelRepoMock
                .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(hotels);

            // Act
            var result = await _service.GetAllHotelsAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal("Test Hotel", result.First().Name);

            _hotelRepoMock.Verify(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        [Fact(DisplayName = "Get hotel by ID returns mapped DTO")]
        public async Task GetHotelDtoByIdAsync_ShouldReturnHotelDto()
        {
            // Arrange
            var hotel = new Hotel
            {
                Id = 2,
                Name = "Sea View",
                City = new City { Name = "Dubai" },
                StarRating = 4,
                Rooms = new List<Room> { new Room { PricePerNight = 200 } },
                Images = new List<HotelImage> { new HotelImage { ImageUrl = "sea.jpg" } }
            };

            _hotelRepoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(hotel);

            // Act
            var result = await _service.GetHotelDtoByIdAsync(2);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Sea View", result!.Name);
        }

        [Fact(DisplayName = "Search hotels calls repository with correct pagination")]
        public async Task SearchHotelsAsync_ShouldCallRepositoryWithPagination()
        {
            // Arrange
            var hotels = new List<Hotel>
            {
                new Hotel
                {
                    Id = 3,
                    Name = "City Lights",
                    City = new City { Name = "New York" },
                    StarRating = 5,
                    Rooms = new List<Room> { new Room { PricePerNight = 300 } },
                    Images = new List<HotelImage> { new HotelImage { ImageUrl = "nyc.jpg" } }
                }
            };

            _hotelRepoMock
                .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(hotels);

            var query = new SearchHotelsQuery
            {
                Page = 1,
                PageSize = 10
            };

            // Act
            var result = await _service.SearchHotelsAsync(query);

            // Assert
            Assert.Single(result);
            Assert.Equal("City Lights", result.First().Name);

            _hotelRepoMock.Verify(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        [Fact(DisplayName = "Get featured hotels returns mapped DTOs")]
        public async Task GetFeaturedHotelsAsync_ShouldReturnFeaturedHotelDtos()
        {
            // Arrange
            var hotels = new List<Hotel>
            {
                new Hotel
                {
                    Id = 4,
                    Name = "Luxury Stay",
                    City = new City { Name = "London" },
                    StarRating = 5,
                    Rooms = new List<Room> { new Room { PricePerNight = 500 } },
                    Images = new List<HotelImage> { new HotelImage { ImageUrl = "luxury.jpg" } }
                }
            };

            _hotelRepoMock.Setup(r => r.GetFeaturedHotelsAsync()).ReturnsAsync(hotels);

            // Act
            var result = await _service.GetFeaturedHotelsAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal("Luxury Stay", result.First().Name);
        }
    }
}

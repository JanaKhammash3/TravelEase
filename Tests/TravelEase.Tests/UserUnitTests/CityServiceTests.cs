using Moq;
using TravelEase.Application.Features.City;
using TravelEase.Application.Interfaces;
using TravelEase.Domain.Entities;

namespace TravelEase.Tests.UserUnitTests;

public class CityServiceTests
{
    private readonly Mock<ICityRepository> _cityRepoMock;
    private readonly CityService _service;

    public CityServiceTests()
    {
        _cityRepoMock = new Mock<ICityRepository>();
        _service = new CityService(_cityRepoMock.Object);
    }

    [Fact(DisplayName = "Get all cities")]
    public async Task GetAllCitiesAsync_ShouldReturnCities()
    {
        var cities = new List<City>
        {
            new() { Id = 1, Name = "Tokyo", Country = "Japan", PostOffice = "100-0001" },
            new() { Id = 2, Name = "Paris", Country = "France", PostOffice = "75001" }
        };

        _cityRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(cities);

        var result = await _service.GetAllCitiesAsync();

        Assert.Equal(2, result.Count);
    }

    [Fact(DisplayName = "Create city")]
    public async Task CreateCityAsync_ShouldAddCity()
    {
        var cmd = new CreateCityCommand
        {
            Name = "Berlin",
            Country = "Germany",
            PostOffice = "10115"
        };

        _cityRepoMock.Setup(r => r.AddAsync(It.IsAny<City>())).Returns(Task.CompletedTask);

        await _service.CreateCityAsync(cmd);

        _cityRepoMock.Verify(r => r.AddAsync(It.Is<City>(
            c => c.Name == "Berlin" && c.Country == "Germany" && c.PostOffice == "10115"
        )), Times.Once);
    }

    [Fact(DisplayName = "Delete city when found")]
    public async Task DeleteCityAsync_ShouldDelete_WhenCityExists()
    {
        var city = new City { Id = 10, Name = "Dubai" };
        _cityRepoMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(city);
        _cityRepoMock.Setup(r => r.DeleteAsync(city)).Returns(Task.CompletedTask);

        await _service.DeleteCityAsync(10);

        _cityRepoMock.Verify(r => r.DeleteAsync(city), Times.Once);
    }

    [Fact(DisplayName = "Delete city when not found — should do nothing")]
    public async Task DeleteCityAsync_ShouldDoNothing_WhenNotFound()
    {
        _cityRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((City)null!);

        await _service.DeleteCityAsync(99);

        _cityRepoMock.Verify(r => r.DeleteAsync(It.IsAny<City>()), Times.Never);
    }

    [Fact(DisplayName = "Update city when found")]
    public async Task UpdateCityAsync_ShouldUpdate_WhenCityExists()
    {
        var city = new City
        {
            Id = 1,
            Name = "OldName",
            Country = "OldCountry",
            PostOffice = "0000"
        };

        var updateCmd = new UpdateCityCommand
        {
            Id = 1,
            Name = "NewName",
            Country = "NewCountry",
            PostOffice = "12345"
        };

        _cityRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(city);
        _cityRepoMock.Setup(r => r.UpdateAsync(city)).Returns(Task.CompletedTask);

        await _service.UpdateCityAsync(updateCmd);

        Assert.Equal("NewName", city.Name);
        Assert.Equal("NewCountry", city.Country);
        Assert.Equal("12345", city.PostOffice);
    }

    [Fact(DisplayName = "Update city when not found — should throw")]
    public async Task UpdateCityAsync_ShouldThrow_WhenCityNotFound()
    {
        var updateCmd = new UpdateCityCommand
        {
            Id = 100,
            Name = "Doesn't",
            Country = "Exist",
            PostOffice = "00000"
        };

        _cityRepoMock.Setup(r => r.GetByIdAsync(100)).ReturnsAsync((City)null!);

        var ex = await Assert.ThrowsAsync<Exception>(() => _service.UpdateCityAsync(updateCmd));
        Assert.Equal("City not found", ex.Message);
    }

    [Fact(DisplayName = "Search cities")]
    public async Task SearchCitiesAsync_ShouldReturnResults()
    {
        var cities = new List<City>
        {
            new() { Name = "Rome" },
            new() { Name = "Riyadh" }
        };

        _cityRepoMock.Setup(r => r.SearchAsync("R", 1, 10)).ReturnsAsync(cities);

        var result = await _service.SearchCitiesAsync("R", 1, 10);

        Assert.Equal(2, result.Count);
    }
}

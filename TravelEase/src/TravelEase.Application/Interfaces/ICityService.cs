using TravelEase.Application.Features.City;
using TravelEase.Domain.Entities;

namespace TravelEase.Application.Interfaces
{
    public interface ICityService
    {
        Task<List<City>> GetAllCitiesAsync();
        Task CreateCityAsync(CreateCityCommand cmd);
        Task DeleteCityAsync(int id);
        Task UpdateCityAsync(UpdateCityCommand cmd);
        Task<List<City>> SearchCitiesAsync(string keyword, int page, int pageSize);
    }
}
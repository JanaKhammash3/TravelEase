using TravelEase.TravelEase.Application.Features.City;

namespace TravelEase.TravelEase.Application.Interfaces
{
    public interface ICityService
    {
        Task<List<Domain.Entities.City>> GetAllCitiesAsync();
        Task CreateCityAsync(CreateCityCommand cmd);
        Task DeleteCityAsync(int id);
        Task UpdateCityAsync(UpdateCityCommand cmd);
        Task<List<Domain.Entities.City>> SearchCitiesAsync(string keyword, int page, int pageSize);
    }
}
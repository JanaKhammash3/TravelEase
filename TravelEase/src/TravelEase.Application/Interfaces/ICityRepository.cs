using TravelEase.TravelEase.Domain.Entities;

namespace TravelEase.TravelEase.Application.Interfaces
{
    public interface ICityRepository
    {
        Task<List<City>> GetAllAsync();
        Task<City?> GetByIdAsync(int id);
        Task AddAsync(City city);
        Task UpdateAsync(City city);
        Task DeleteAsync(City city);
        Task<List<City>> SearchAsync(string keyword, int page, int pageSize);
    }
}

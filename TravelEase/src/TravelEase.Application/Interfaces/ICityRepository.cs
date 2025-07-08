namespace TravelEase.TravelEase.Application.Interfaces
{
    public interface ICityRepository
    {
        Task<List<Domain.Entities.City>> GetAllAsync();
        Task<Domain.Entities.City?> GetByIdAsync(int id);
        Task AddAsync(Domain.Entities.City city);
        Task UpdateAsync(Domain.Entities.City city);
        Task DeleteAsync(Domain.Entities.City city);
        Task<List<Domain.Entities.City>> SearchAsync(string keyword, int page, int pageSize);
    }
}

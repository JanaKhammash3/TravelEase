using Microsoft.EntityFrameworkCore;
using TravelEase.Application.Interfaces;
using TravelEase.Domain.Entities;
using TravelEase.Infrastructure.Data;

namespace TravelEase.Infrastructure.Repositories
{
    public class CityRepository : ICityRepository
    {
        private readonly TravelEaseDbContext _db;

        public CityRepository(TravelEaseDbContext db)
        {
            _db = db;
        }

        public async Task<List<City>> GetAllAsync() => await _db.Cities.ToListAsync();

        public async Task<City?> GetByIdAsync(int id) => await _db.Cities.FindAsync(id);

        public async Task AddAsync(City city)
        {
            _db.Cities.Add(city);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(City city)
        {
            _db.Cities.Update(city);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(City city)
        {
            _db.Cities.Remove(city);
            await _db.SaveChangesAsync();
        }

        public async Task<List<City>> SearchAsync(string keyword, int page, int pageSize)
        {
            return await _db.Cities
                .Where(c => c.Name.Contains(keyword) || c.Country.Contains(keyword))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
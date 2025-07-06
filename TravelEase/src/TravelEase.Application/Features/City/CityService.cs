using Microsoft.EntityFrameworkCore;
using TravelEase.TravelEase.Domain.Entities;
using TravelEase.TravelEase.Infrastructure.Data;

namespace TravelEase.TravelEase.Application.Features.City
{
    public class CityService
    {
        private readonly TravelEaseDbContext _db;

        public CityService(TravelEaseDbContext db)
        {
            _db = db;
        }

        public async Task<List<Domain.Entities.City>> GetAllCitiesAsync()
        {
            return await _db.Cities.ToListAsync();
        }

        public async Task CreateCityAsync(CreateCityCommand cmd)
        {
            var city = new Domain.Entities.City
            {
                Name = cmd.Name,
                Country = cmd.Country,
                PostOffice = cmd.PostOffice
            };

            _db.Cities.Add(city);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteCityAsync(int id)
        {
            var city = await _db.Cities.FindAsync(id);
            if (city != null)
            {
                _db.Cities.Remove(city);
                await _db.SaveChangesAsync();
            }
        }
        public async Task UpdateCityAsync(UpdateCityCommand cmd)
        {
            var city = await _db.Cities.FindAsync(cmd.Id);
            if (city == null) throw new Exception("City not found");

            city.Name = cmd.Name;
            city.Country = cmd.Country;
            city.PostOffice = cmd.PostOffice;

            _db.Cities.Update(city);
            await _db.SaveChangesAsync();
        }
        
        public async Task<List<Domain.Entities.City>> SearchCitiesAsync(string keyword, int page, int pageSize)
        {
            return await _db.Cities
                .Where(c => c.Name.Contains(keyword) || c.Country.Contains(keyword))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }


    }
}
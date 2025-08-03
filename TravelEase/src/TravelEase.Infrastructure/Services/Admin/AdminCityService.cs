using Microsoft.EntityFrameworkCore;
using TravelEase.Application.DTOs.Admin;
using TravelEase.Application.Interfaces.Admin;
using TravelEase.Domain.Entities;
using TravelEase.Infrastructure.Data;

namespace TravelEase.Infrastructure.Services.Admin;

public class AdminCityService : IAdminCityService {
    private readonly TravelEaseDbContext _context;

    public AdminCityService(TravelEaseDbContext context) {
        _context = context;
    }

    public async Task<List<AdminCityDto>> GetAllCitiesAsync() {
        return await _context.Cities
            .Select(c => new AdminCityDto {
                Id = c.Id,
                Name = c.Name,
                Country = c.Country,
                PostOffice = c.PostOffice,
                HotelCount = c.Hotels.Count
            }).ToListAsync();
    }

    public async Task<AdminCityDto> GetByIdAsync(int id) {
        var c = await _context.Cities.Include(c => c.Hotels).FirstOrDefaultAsync(c => c.Id == id);
        return new AdminCityDto {
            Id = c.Id,
            Name = c.Name,
            Country = c.Country,
            PostOffice = c.PostOffice,
            HotelCount = c.Hotels.Count
        };
    }

    public async Task<AdminCityDto> CreateAsync(AdminCityDto dto)
    {
        var city = new City
        {
            Name = dto.Name,
            Country = dto.Country,
            PostOffice = dto.PostOffice,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Cities.Add(city);
        await _context.SaveChangesAsync();
        dto.Id = city.Id;
        return dto;
    }

    public async Task<AdminCityDto> UpdateAsync(int id, AdminCityDto dto)
    {
        var city = await _context.Cities.FindAsync(id);
        if (city == null) return null;

        city.Name = dto.Name;
        city.Country = dto.Country;
        city.PostOffice = dto.PostOffice;
        city.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return dto;
    }


    public async Task<bool> DeleteAsync(int id) {
        var city = await _context.Cities.FindAsync(id);
        if (city == null) return false;
        _context.Cities.Remove(city);
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<List<AdminCityDto>> GetAllAsync(string? name = null, string? country = null, string? postOffice = null, int? minHotels = null)
    {
        var query = _context.Cities
            .Include(c => c.Hotels)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(c => c.Name.Contains(name));

        if (!string.IsNullOrWhiteSpace(country))
            query = query.Where(c => c.Country.Contains(country));

        if (!string.IsNullOrWhiteSpace(postOffice))
            query = query.Where(c => c.PostOffice.Contains(postOffice));

        if (minHotels.HasValue)
            query = query.Where(c => c.Hotels.Count >= minHotels.Value);

        return await query.Select(c => new AdminCityDto
        {
            Id = c.Id,
            Name = c.Name,
            Country = c.Country,
            PostOffice = c.PostOffice,
            HotelCount = c.Hotels.Count
        }).ToListAsync();
    }

}
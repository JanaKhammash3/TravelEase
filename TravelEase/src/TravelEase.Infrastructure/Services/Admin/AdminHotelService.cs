using Microsoft.EntityFrameworkCore;
using TravelEase.TravelEase.Application.DTOs.Admin;
using TravelEase.TravelEase.Application.Interfaces.Admin;
using TravelEase.TravelEase.Domain.Entities;
using TravelEase.TravelEase.Infrastructure.Data;

public class AdminHotelService : IAdminHotelService
{
    private readonly TravelEaseDbContext _context;

    public AdminHotelService(TravelEaseDbContext context)
    {
        _context = context;
    }

    public async Task<List<AdminHotelDto>> GetAllAsync()
    {
        return await _context.Hotels
            .Include(h => h.Rooms)
            .Include(h => h.City)
            .Select(h => new AdminHotelDto
            {
                Id = h.Id,
                Name = h.Name,
                Owner = h.Owner,
                StarRating = h.StarRating,
                CityName = h.City.Name,
                RoomCount = h.Rooms.Count
            })
            .ToListAsync();
    }

    public async Task<AdminHotelDto> GetByIdAsync(int id)
    {
        var h = await _context.Hotels
            .Include(h => h.City)
            .Include(h => h.Rooms)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (h == null) return null;

        return new AdminHotelDto
        {
            Id = h.Id,
            Name = h.Name,
            Owner = h.Owner,
            StarRating = h.StarRating,
            CityName = h.City?.Name,
            RoomCount = h.Rooms?.Count ?? 0
        };
    }

    public async Task<AdminHotelDto> CreateAsync(AdminHotelDto dto)
    {
        var city = await _context.Cities.FirstOrDefaultAsync(c => c.Name == dto.CityName);
        if (city == null) throw new Exception("City not found.");

        var hotel = new Hotel
        {
            Name = dto.Name,
            Owner = dto.Owner,
            StarRating = dto.StarRating,
            CityId = city.Id
        };


        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync();

        dto.Id = hotel.Id;
        return dto;
    }

    public async Task<AdminHotelDto> UpdateAsync(int id, AdminHotelDto dto)
    {
        var hotel = await _context.Hotels.FindAsync(id);
        if (hotel == null) return null;

        var city = await _context.Cities.FirstOrDefaultAsync(c => c.Name == dto.CityName);
        if (city == null) throw new Exception("City not found.");

        hotel.Name = dto.Name;
        hotel.Owner = dto.Owner;
        hotel.StarRating = dto.StarRating;
        hotel.CityId = city.Id;

        await _context.SaveChangesAsync();
        return dto;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var hotel = await _context.Hotels.FindAsync(id);
        if (hotel == null) return false;

        _context.Hotels.Remove(hotel);
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<List<AdminHotelDto>> GetAllAsync(string? name = null, string? city = null, int? minStars = null)
    {
        var query = _context.Hotels
            .Include(h => h.City)
            .Include(h => h.Rooms)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(h => h.Name.Contains(name));

        if (!string.IsNullOrWhiteSpace(city))
            query = query.Where(h => h.City.Name.Contains(city));

        if (minStars.HasValue)
            query = query.Where(h => h.StarRating >= minStars.Value);

        return await query.Select(h => new AdminHotelDto
        {
            Id = h.Id,
            Name = h.Name,
            Owner = h.Owner,
            StarRating = h.StarRating,
            CityName = h.City.Name,
            RoomCount = h.Rooms.Count
        }).ToListAsync();
    }


}

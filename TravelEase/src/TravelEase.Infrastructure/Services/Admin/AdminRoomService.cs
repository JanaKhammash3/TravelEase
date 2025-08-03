using Microsoft.EntityFrameworkCore;
using TravelEase.TravelEase.Application.DTOs.Admin;
using TravelEase.TravelEase.Application.Interfaces.Admin;
using TravelEase.TravelEase.Domain.Entities;
using TravelEase.TravelEase.Infrastructure.Data;

namespace TravelEase.TravelEase.Infrastructure.Services.Admin;

public class AdminRoomService : IAdminRoomService
{
    private readonly TravelEaseDbContext _context;

    public AdminRoomService(TravelEaseDbContext context)
    {
        _context = context;
    }

    public async Task<List<AdminRoomDto>> GetAllAsync()
    {
        return await _context.Rooms
            .Include(r => r.Hotel)
            .Select(r => new AdminRoomDto
            {
                Id = r.Id,
                Number = r.Number,
                CapacityAdults = r.CapacityAdults,
                CapacityChildren = r.CapacityChildren,
                IsAvailable = !r.Bookings.Any(b => b.CheckOut > DateTime.UtcNow),
                HotelName = r.Hotel.Name
            })
            .ToListAsync();
    }

    public async Task<AdminRoomDto> GetByIdAsync(int id)
    {
        var r = await _context.Rooms.Include(r => r.Hotel).Include(r => r.Bookings).FirstOrDefaultAsync(r => r.Id == id);
        if (r == null) return null;

        return new AdminRoomDto
        {
            Id = r.Id,
            Number = r.Number,
            CapacityAdults = r.CapacityAdults,
            CapacityChildren = r.CapacityChildren,
            IsAvailable = !r.Bookings.Any(b => b.CheckOut > DateTime.UtcNow),
            HotelName = r.Hotel?.Name
        };
    }

    public async Task<AdminRoomDto> CreateAsync(AdminRoomDto dto)
    {
        var hotel = await _context.Hotels.FirstOrDefaultAsync(h => h.Name == dto.HotelName);
        if (hotel == null) throw new Exception("Hotel not found.");

        var room = new Room
        {
            Number = dto.Number,
            CapacityAdults = dto.CapacityAdults,
            CapacityChildren = dto.CapacityChildren,
            HotelId = hotel.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        dto.Id = room.Id;
        return dto;
    }

    public async Task<AdminRoomDto> UpdateAsync(int id, AdminRoomDto dto)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null) return null;

        var hotel = await _context.Hotels.FirstOrDefaultAsync(h => h.Name == dto.HotelName);
        if (hotel == null) throw new Exception("Hotel not found.");

        room.Number = dto.Number;
        room.CapacityAdults = dto.CapacityAdults;
        room.CapacityChildren = dto.CapacityChildren;
        room.HotelId = hotel.Id;
        room.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return dto;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null) return false;

        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<List<AdminRoomDto>> GetAllAsync(string? hotelName = null, bool? isAvailable = null, int? minAdults = null, int? minChildren = null)
    {
        var query = _context.Rooms
            .Include(r => r.Hotel)
            .Include(r => r.Bookings)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(hotelName))
            query = query.Where(r => r.Hotel.Name.Contains(hotelName));

        if (minAdults.HasValue)
            query = query.Where(r => r.CapacityAdults >= minAdults.Value);

        if (minChildren.HasValue)
            query = query.Where(r => r.CapacityChildren >= minChildren.Value);

        if (isAvailable.HasValue)
        {
            var now = DateTime.UtcNow;
            query = query.Where(r =>
                isAvailable.Value
                    ? !r.Bookings.Any(b => b.CheckOut > now)
                    : r.Bookings.Any(b => b.CheckOut > now));
        }

        return await query.Select(r => new AdminRoomDto
        {
            Id = r.Id,
            Number = r.Number,
            CapacityAdults = r.CapacityAdults,
            CapacityChildren = r.CapacityChildren,
            IsAvailable = !r.Bookings.Any(b => b.CheckOut > DateTime.UtcNow),
            HotelName = r.Hotel.Name
        }).ToListAsync();
    }


}
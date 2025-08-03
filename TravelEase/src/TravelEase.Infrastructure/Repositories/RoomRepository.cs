using Microsoft.EntityFrameworkCore;
using TravelEase.TravelEase.Application.Features.Room;
using TravelEase.TravelEase.Application.Interfaces;
using TravelEase.TravelEase.Domain.Entities;
using TravelEase.TravelEase.Infrastructure.Data;

namespace TravelEase.TravelEase.Infrastructure.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly TravelEaseDbContext _context;

    public RoomRepository(TravelEaseDbContext context)
    {
        _context = context;
    }

    public async Task<List<Room>> GetAllRoomsAsync() => await _context.Rooms.ToListAsync();
    public async Task<Room?> GetRoomByIdAsync(int id) => await _context.Rooms.FindAsync(id);
    public async Task AddRoomAsync(Room room) { _context.Rooms.Add(room); await _context.SaveChangesAsync(); }
    public async Task UpdateRoomAsync(Room room) { _context.Rooms.Update(room); await _context.SaveChangesAsync(); }
    public async Task DeleteRoomAsync(Room room) { _context.Rooms.Remove(room); await _context.SaveChangesAsync(); }

    public async Task<IEnumerable<Room>> SearchRoomsAsync(SearchRoomsQuery query)
    {
        var rooms = _context.Rooms.AsQueryable();

        if (query.HotelId.HasValue)
            rooms = rooms.Where(r => r.HotelId == query.HotelId.Value);

        if (query.MinPrice.HasValue)
            rooms = rooms.Where(r => r.PricePerNight >= query.MinPrice.Value);

        if (query.MaxPrice.HasValue)
            rooms = rooms.Where(r => r.PricePerNight <= query.MaxPrice.Value);

        if (query.Category.HasValue)
            rooms = rooms.Where(r => (int)r.Category == query.Category.Value);

        if (query.Adults.HasValue)
            rooms = rooms.Where(r => r.CapacityAdults >= query.Adults.Value);

        if (query.Children.HasValue)
            rooms = rooms.Where(r => r.CapacityChildren >= query.Children.Value);

        if (query.CheckIn.HasValue && query.CheckOut.HasValue)
        {
            var checkIn = query.CheckIn.Value;
            var checkOut = query.CheckOut.Value;

            rooms = rooms.Where(r =>
                !_context.Bookings.Any(b =>
                    b.RoomId == r.Id &&
                    (
                        (checkIn >= b.CheckIn && checkIn < b.CheckOut) ||
                        (checkOut > b.CheckIn && checkOut <= b.CheckOut) ||
                        (checkIn <= b.CheckIn && checkOut >= b.CheckOut)
                    )
                )
            );
        }

        // Apply pagination at DB level
        return await rooms
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();
    }

}
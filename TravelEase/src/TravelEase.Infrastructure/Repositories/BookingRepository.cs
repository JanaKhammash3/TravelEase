using Microsoft.EntityFrameworkCore;
using TravelEase.TravelEase.Application.Features.Booking;
using TravelEase.TravelEase.Domain.Entities;
using TravelEase.TravelEase.Infrastructure.Data;

namespace TravelEase.TravelEase.Infrastructure.Repositories;

public class BookingRepository
{
    private readonly TravelEaseDbContext _context;

    public BookingRepository(TravelEaseDbContext context)
    {
        _context = context;
    }

    public async Task<List<Booking>> GetAllAsync() =>
        await _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Room)
            .ThenInclude(r => r.Hotel)
            .ToListAsync();

    public async Task<Booking?> GetByIdAsync(int id) =>
        await _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Room)
            .FirstOrDefaultAsync(b => b.Id == id);

    public async Task AddAsync(Booking booking)
    {
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Booking booking)
    {
        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();
    }
    
    public async Task UpdateAsync(Booking booking)
    {
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();
    }
    public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut)
    {
        return !await _context.Bookings
            .AnyAsync(b => b.RoomId == roomId &&
                           b.CheckIn < checkOut &&
                           b.CheckOut > checkIn); // overlap logic
    }
    
    public async Task<IEnumerable<Booking>> SearchAsync(SearchBookingsQuery query)
    {
        var bookings = _context.Bookings
            .Include(b => b.Room)
            .Include(b => b.User)
            .AsQueryable();

        if (query.UserId.HasValue)
            bookings = bookings.Where(b => b.UserId == query.UserId.Value);

        if (query.RoomId.HasValue)
            bookings = bookings.Where(b => b.RoomId == query.RoomId.Value);

        if (query.CheckIn.HasValue)
            bookings = bookings.Where(b => b.CheckIn.Date == query.CheckIn.Value.Date);

        if (query.CheckOut.HasValue)
            bookings = bookings.Where(b => b.CheckOut.Date == query.CheckOut.Value.Date);

        return await bookings.ToListAsync();
    }


}
using Microsoft.EntityFrameworkCore;
using TravelEase.Application.Interfaces;
using TravelEase.TravelEase.Application.Interfaces;
using TravelEase.TravelEase.Domain.Entities;
using TravelEase.TravelEase.Infrastructure.Data;

namespace TravelEase.TravelEase.Infrastructure.Repositories
{
    public class HotelRepository : IHotelRepository
    {
        private readonly TravelEaseDbContext _context;

        public HotelRepository(TravelEaseDbContext context)
        {
            _context = context;
        }

        public async Task<List<Hotel>> GetAllAsync()
        {
            return await _context.Hotels
                .Include(h => h.City)
                .Include(h => h.Rooms)
                .ToListAsync();
        }

        public async Task<Hotel?> GetByIdAsync(int id)
        {
            return await _context.Hotels
                .Include(h => h.City)
                .Include(h => h.Rooms)
                .FirstOrDefaultAsync(h => h.Id == id);
        }

        public async Task AddAsync(Hotel hotel)
        {
            _context.Hotels.Add(hotel);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Hotel hotel)
        {
            _context.Hotels.Update(hotel);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Hotel hotel)
        {
            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Hotel>> GetFeaturedHotelsAsync()
        {
            return await _context.Hotels
                .Where(h => h.IsFeatured)
                .OrderByDescending(h => h.StarRating)
                .Take(5)
                .ToListAsync();
        }

        public IEnumerable<Room> GetAvailableRoomsForHotel(int hotelId, DateTime checkIn, DateTime checkOut, int adults, int children)
        {
            return _context.Rooms
                .Where(r => r.HotelId == hotelId &&
                            r.CapacityAdults >= adults &&
                            r.CapacityChildren >= children &&
                            !_context.Bookings.Any(b =>
                                b.RoomId == r.Id &&
                                (
                                    (checkIn >= b.CheckIn && checkIn < b.CheckOut) ||
                                    (checkOut > b.CheckIn && checkOut <= b.CheckOut) ||
                                    (checkIn <= b.CheckIn && checkOut >= b.CheckOut)
                                )
                            ))
                .ToList();
        }
    }
}
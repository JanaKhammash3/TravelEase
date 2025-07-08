using Microsoft.EntityFrameworkCore;
using TravelEase.TravelEase.Domain.Entities;
using TravelEase.TravelEase.Infrastructure.Data;

namespace TravelEase.TravelEase.Infrastructure.Repositories
{
    public class HotelRepository
    {
        private readonly TravelEaseDbContext _context;

        public HotelRepository(TravelEaseDbContext context)
        {
            _context = context;
        }

        public async Task<List<Hotel>> GetAllAsync() =>
            await _context.Hotels.Include(h => h.City).ToListAsync();

        public async Task<Hotel?> GetByIdAsync(int id) =>
            await _context.Hotels.Include(h => h.City).FirstOrDefaultAsync(h => h.Id == id);

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
        
        public List<Room> GetAvailableRoomsForHotel(int hotelId, DateTime checkIn, DateTime checkOut, int adults, int children)
        {
            return _context.Rooms
                .Where(room =>
                    room.HotelId == hotelId &&
                    room.CapacityAdults >= adults &&
                    room.CapacityChildren >= children &&
                    !_context.Bookings.Any(b =>
                        b.RoomId == room.Id &&
                        (
                            (checkIn >= b.CheckIn && checkIn < b.CheckOut) ||
                            (checkOut > b.CheckIn && checkOut <= b.CheckOut) ||
                            (checkIn <= b.CheckIn && checkOut >= b.CheckOut)
                        )
                    )
                ).ToList();
        }

        
        
    }
}
using Microsoft.EntityFrameworkCore;
using TravelEase.TravelEase.Application.Interfaces;
using TravelEase.TravelEase.Domain.Entities;
using TravelEase.TravelEase.Infrastructure.Data;

namespace TravelEase.TravelEase.Infrastructure.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly TravelEaseDbContext _db;

        public BookingRepository(TravelEaseDbContext db)
        {
            _db = db;
        }

        public async Task<List<Booking>> GetAllAsync() => await _db.Bookings.ToListAsync();

        public async Task<Booking?> GetByIdAsync(int id) => await _db.Bookings.FindAsync(id);

        public async Task AddAsync(Booking booking)
        {
            _db.Bookings.Add(booking);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Booking booking)
        {
            _db.Bookings.Update(booking);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Booking booking)
        {
            _db.Bookings.Remove(booking);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut)
        {
            return !await _db.Bookings.AnyAsync(b =>
                b.RoomId == roomId &&
                (
                    (checkIn >= b.CheckIn && checkIn < b.CheckOut) ||
                    (checkOut > b.CheckIn && checkOut <= b.CheckOut) ||
                    (checkIn <= b.CheckIn && checkOut >= b.CheckOut)
                ));
        }
    }
}
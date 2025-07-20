using Microsoft.EntityFrameworkCore;
using TravelEase.TravelEase.Application.DTOs;
using TravelEase.TravelEase.Application.Features.Booking;
using TravelEase.TravelEase.Application.Interfaces;
using TravelEase.TravelEase.Domain.Entities;
using TravelEase.TravelEase.Infrastructure.Data;

namespace TravelEase.TravelEase.Infrastructure.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly TravelEaseDbContext _context;

        public BookingRepository(TravelEaseDbContext context)
        {
            _context = context;
        }

        public async Task<List<BookingDto>> GetAllAsync()
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Room)
                .ThenInclude(r => r.Hotel)
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    UserEmail = b.User.Email,
                    HotelName = b.Room.Hotel.Name,
                    RoomNumber = b.Room.Number,
                    CheckIn = b.CheckIn,
                    CheckOut = b.CheckOut,
                    TotalPrice = b.TotalPrice,
                    PaymentStatus = b.PaymentStatus
                })
                .ToListAsync();
        }

        public async Task<BookingDto?> GetByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Room)
                .ThenInclude(r => r.Hotel)
                .Where(b => b.Id == id)
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    UserEmail = b.User.Email,
                    HotelName = b.Room.Hotel.Name,
                    RoomNumber = b.Room.Number,
                    CheckIn = b.CheckIn,
                    CheckOut = b.CheckOut,
                    TotalPrice = b.TotalPrice,
                    PaymentStatus = b.PaymentStatus
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<BookingDto>> SearchAsync(SearchBookingsQuery query)
        {
            var bookings = _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Room)
                .ThenInclude(r => r.Hotel)
                .AsQueryable();

            if (query.UserId.HasValue)
                bookings = bookings.Where(b => b.UserId == query.UserId.Value);

            if (query.RoomId.HasValue)
                bookings = bookings.Where(b => b.RoomId == query.RoomId.Value);

            if (query.FromDate.HasValue)
                bookings = bookings.Where(b => b.CheckIn >= query.FromDate.Value);

            if (query.ToDate.HasValue)
                bookings = bookings.Where(b => b.CheckOut <= query.ToDate.Value);

            return await bookings
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    UserEmail = b.User.Email,
                    HotelName = b.Room.Hotel.Name,
                    RoomNumber = b.Room.Number,
                    CheckIn = b.CheckIn,
                    CheckOut = b.CheckOut,
                    TotalPrice = b.TotalPrice,
                    PaymentStatus = b.PaymentStatus
                })
                .ToListAsync();
        }

        public async Task<Booking?> GetBookingEntityByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task AddAsync(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Booking booking)
        {
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut)
        {
            return !await _context.Bookings.AnyAsync(b =>
                b.RoomId == roomId &&
                (
                    (checkIn >= b.CheckIn && checkIn < b.CheckOut) ||
                    (checkOut > b.CheckIn && checkOut <= b.CheckOut) ||
                    (checkIn <= b.CheckIn && checkOut >= b.CheckOut)
                ));
        }
    }
}

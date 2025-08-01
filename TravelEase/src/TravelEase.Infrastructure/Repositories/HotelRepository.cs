using Microsoft.EntityFrameworkCore;
using TravelEase.TravelEase.Application.Interfaces;
using TravelEase.TravelEase.Domain.Entities;
using TravelEase.TravelEase.Infrastructure.Data;
using TravelEase.TravelEase.Application.DTOs;
using System.Linq;

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
                .Include(h => h.Images) 
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
        
        public async Task RecordHotelViewAsync(int userId, int hotelId)
        {
            var view = new HotelView
            {
                UserId = userId,
                HotelId = hotelId,
                ViewedAt = DateTime.UtcNow
            };
            _context.HotelViews.Add(view);
            await _context.SaveChangesAsync();
        }

        public async Task<List<HotelDto>> GetRecentlyVisitedHotelsAsync(int userId, int count = 5)
        {
            var recent = await _context.HotelViews
                .Where(v => v.UserId == userId)
                .OrderByDescending(v => v.ViewedAt)
                .Select(v => new HotelDto
                {
                    Id = v.Hotel.Id,
                    Name = v.Hotel.Name,
                    City = v.Hotel.City.Name, 
                    StarRating = v.Hotel.StarRating,
                    ThumbnailUrl = v.Hotel.ThumbnailUrl
                })

                .DistinctBy(h => h.Id)
                .Take(count)
                .ToListAsync();

            return recent;
        }



        public async Task<List<TrendingCityDto>> GetTrendingCitiesAsync(int count = 5)
        {
            var topCities = await _context.HotelViews
                .GroupBy(v => v.Hotel.City)
                .Select(g => new TrendingCityDto
                {
                    City = g.Key.Name, 
                    VisitCount = g.Count(),
                    ThumbnailUrl = _context.Hotels
                        .Where(h => h.City.Id == g.Key.Id)
                        .Select(h => h.ThumbnailUrl)
                        .FirstOrDefault() ?? ""
                })

                .OrderByDescending(x => x.VisitCount)
                .Take(count)
                .ToListAsync();

            return topCities;
        }
        
        public async Task SaveHotelImageUrlsAsync(int hotelId, List<string> urls)
        {
            var images = urls.Select(url => new HotelImage
            {
                HotelId = hotelId,
                ImageUrl = url
            }).ToList();

            _context.HotelImages.AddRange(images);
            await _context.SaveChangesAsync();
        }


    }
}
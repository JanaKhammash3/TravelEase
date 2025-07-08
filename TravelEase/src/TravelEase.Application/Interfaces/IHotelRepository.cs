using TravelEase.TravelEase.Application.DTOs;
using TravelEase.TravelEase.Domain.Entities;

namespace TravelEase.TravelEase.Application.Interfaces
{
    public interface IHotelRepository
    {
        Task<List<Hotel>> GetAllAsync();
        Task<Hotel?> GetByIdAsync(int id);
        Task AddAsync(Hotel hotel);
        Task UpdateAsync(Hotel hotel);
        Task DeleteAsync(Hotel hotel);
        Task<List<Hotel>> GetFeaturedHotelsAsync();
        IEnumerable<Room> GetAvailableRoomsForHotel(int hotelId, DateTime checkIn, DateTime checkOut, int adults, int children);
        Task RecordHotelViewAsync(int userId, int hotelId);
        Task<List<HotelDto>> GetRecentlyVisitedHotelsAsync(int userId, int count = 5);
        Task<List<TrendingCityDto>> GetTrendingCitiesAsync(int count = 5);

    }
}
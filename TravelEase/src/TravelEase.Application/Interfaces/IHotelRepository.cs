using TravelEase.Application.DTOs;
using TravelEase.Domain.Entities;

namespace TravelEase.Application.Interfaces
{
    public interface IHotelRepository
    {
        Task<List<Hotel>> GetAllAsync(int page = 1, int pageSize = 20);
        Task<Hotel?> GetByIdAsync(int id);
        Task AddAsync(Hotel hotel);
        Task UpdateAsync(Hotel hotel);
        Task DeleteAsync(Hotel hotel);
        Task<List<Hotel>> GetFeaturedHotelsAsync();
        IEnumerable<Room> GetAvailableRoomsForHotel(int hotelId, DateTime checkIn, DateTime checkOut, int adults, int children);
        Task RecordHotelViewAsync(int userId, int hotelId);
        Task<List<HotelDto>> GetRecentlyVisitedHotelsAsync(int userId, int count = 5);
        Task<List<TrendingCityDto>> GetTrendingCitiesAsync(int count = 5);
        Task SaveHotelImageUrlsAsync(int hotelId, List<string> urls);
    }

}
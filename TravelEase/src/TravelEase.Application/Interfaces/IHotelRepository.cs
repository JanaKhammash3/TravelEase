using TravelEase.TravelEase.Domain.Entities;

namespace TravelEase.Application.Interfaces
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
    }
}
using TravelEase.TravelEase.Application.Features.Booking;

namespace TravelEase.TravelEase.Application.Interfaces
{
    public interface IBookingRepository
    {
        Task<List<Booking>> GetAllAsync();
        Task<Booking?> GetByIdAsync(int id);
        Task AddAsync(Booking booking);
        Task UpdateAsync(Booking booking);
        Task DeleteAsync(Booking booking);
        Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut);
        Task<List<Booking>> SearchBookingsAsync(SearchBookingsQuery query);
    }
}
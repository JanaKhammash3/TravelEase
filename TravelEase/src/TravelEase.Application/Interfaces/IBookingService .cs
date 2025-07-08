using TravelEase.TravelEase.Domain.Entities;
using TravelEase.TravelEase.Application.Features.Booking;

namespace TravelEase.TravelEase.Application.Interfaces
{
    public interface IBookingService
    {
        Task<List<Booking>> GetAllAsync();
        Task<Booking?> GetByIdAsync(int id);
        Task CreateAsync(CreateBookingCommand cmd);
        Task DeleteAsync(int id);
        Task UpdateAsync(UpdateBookingCommand cmd);
        Task<List<Booking>> SearchBookingsAsync(SearchBookingsQuery query); // if implemented
    }
}
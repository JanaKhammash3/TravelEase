using TravelEase.TravelEase.Application.DTOs;
using TravelEase.TravelEase.Domain.Entities;
using TravelEase.TravelEase.Application.Features.Booking;

namespace TravelEase.TravelEase.Application.Interfaces
{
    public interface IBookingService
    {
        Task<List<BookingDto>> GetAllAsync();
        Task<BookingDto?> GetByIdAsync(int id);
        Task<List<BookingDto>> SearchBookingsAsync(SearchBookingsQuery query);
        Task CreateAsync(CreateBookingCommand cmd);
        Task DeleteAsync(int id);
        Task UpdateAsync(UpdateBookingCommand cmd);
    }
}
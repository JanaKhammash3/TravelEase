using Microsoft.EntityFrameworkCore.Storage;
using TravelEase.TravelEase.Application.DTOs;
using TravelEase.TravelEase.Application.Features.Booking;
using TravelEase.TravelEase.Domain.Entities;

namespace TravelEase.TravelEase.Application.Interfaces
{
    public interface IBookingRepository
    {
        Task<List<BookingDto>> GetAllAsync();
        Task<BookingDto?> GetByIdAsync(int id);
        Task<List<BookingDto>> SearchAsync(SearchBookingsQuery query);
        Task<Booking?> GetBookingEntityByIdAsync(int id);
        Task AddAsync(Booking booking);
        Task UpdateAsync(Booking booking);
        Task DeleteAsync(Booking booking);
        Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut);
        Task<IDbContextTransaction> BeginSerializableTransactionAsync();

    }
}
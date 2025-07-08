using TravelEase.TravelEase.Application.Features.Booking;
using TravelEase.TravelEase.Infrastructure.Repositories;

namespace TravelEase.TravelEase.Application.Features.Booking;

public class BookingService
{
    private readonly BookingRepository _bookingRepository;

    public BookingService(BookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<List<TravelEase.Domain.Entities.Booking>> GetAllAsync()
    {
        return await _bookingRepository.GetAllAsync();
    }

    public async Task<TravelEase.Domain.Entities.Booking?> GetByIdAsync(int id)
    {
        return await _bookingRepository.GetByIdAsync(id);
    }

    public async Task CreateAsync(CreateBookingCommand cmd)
    {
        var isAvailable = await _bookingRepository.IsRoomAvailableAsync(cmd.RoomId, cmd.CheckIn, cmd.CheckOut);
        if (!isAvailable)
            throw new Exception("❌ Room is not available for the selected dates");

        var booking = new TravelEase.Domain.Entities.Booking
        {
            UserId = cmd.UserId,
            RoomId = cmd.RoomId,
            CheckIn = cmd.CheckIn,
            CheckOut = cmd.CheckOut,
            SpecialRequests = cmd.SpecialRequests
        };

        await _bookingRepository.AddAsync(booking);
    }


    public async Task DeleteAsync(int id)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking != null)
            await _bookingRepository.DeleteAsync(booking);
    }
    
    public async Task UpdateAsync(UpdateBookingCommand cmd)
    {
        var booking = await _bookingRepository.GetByIdAsync(cmd.Id);
        if (booking == null) return;

        booking.CheckIn = cmd.CheckIn;
        booking.CheckOut = cmd.CheckOut;
        booking.SpecialRequests = cmd.SpecialRequests;

        await _bookingRepository.UpdateAsync(booking);
    }
    /*public async Task<IEnumerable<Domain.Entities.Booking>> SearchBookingsAsync(SearchBookingsQuery query)
    {
        return await _bookingRepository.SearchAsync(query);
    }*/


}
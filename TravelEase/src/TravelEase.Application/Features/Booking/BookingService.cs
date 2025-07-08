using TravelEase.TravelEase.Application.Features.Booking;
using TravelEase.TravelEase.Application.Interfaces;
using TravelEase.TravelEase.Domain.Entities;

namespace TravelEase.TravelEase.Application.Features.Booking
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<List<global::Booking>> GetAllAsync()
        {
            return await _bookingRepository.GetAllAsync();
        }

        public async Task<global::Booking?> GetByIdAsync(int id)
        {
            return await _bookingRepository.GetByIdAsync(id);
        }

        public async Task CreateAsync(CreateBookingCommand cmd)
        {
            var isAvailable = await _bookingRepository.IsRoomAvailableAsync(cmd.RoomId, cmd.CheckIn, cmd.CheckOut);
            if (!isAvailable)
                throw new Exception("❌ Room is not available for the selected dates");

            var booking = new global::Booking
            {
                UserId = cmd.UserId,
                RoomId = cmd.RoomId,
                CheckIn = cmd.CheckIn,
                CheckOut = cmd.CheckOut,
                Adults = cmd.Adults,
                Children = cmd.Children,
                SpecialRequests = cmd.SpecialRequests,
                PaymentStatus = "Pending", // can be overridden at checkout
                TotalPrice = 0 // will be set at checkout time
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
            booking.Adults = cmd.Adults;
            booking.Children = cmd.Children;

            await _bookingRepository.UpdateAsync(booking);
        }

        public async Task<List<global::Booking>> SearchBookingsAsync(SearchBookingsQuery query)
        {
            return await _bookingRepository.SearchBookingsAsync(query);
        }
    }
}

using TravelEase.TravelEase.Application.DTOs;
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

        public async Task<List<BookingDto>> GetAllAsync()
        {
            return await _bookingRepository.GetAllAsync();
        }

        public async Task<BookingDto?> GetByIdAsync(int id)
        {
            return await _bookingRepository.GetByIdAsync(id);
        }

        public async Task<List<BookingDto>> SearchBookingsAsync(SearchBookingsQuery query)
        {
            return await _bookingRepository.SearchAsync(query);
        }

        public async Task CreateAsync(CreateBookingCommand cmd)
        {
            using var transaction = await _bookingRepository.BeginSerializableTransactionAsync();

            var isAvailable = await _bookingRepository.IsRoomAvailableAsync(cmd.RoomId, cmd.CheckIn, cmd.CheckOut);
            if (!isAvailable)
                throw new Exception(" Room is not available for the selected dates");

            var booking = new Domain.Entities.Booking
            {
                UserId = cmd.UserId,
                RoomId = cmd.RoomId,
                CheckIn = cmd.CheckIn,
                CheckOut = cmd.CheckOut,
                Adults = cmd.Adults,
                Children = cmd.Children,
                SpecialRequests = cmd.SpecialRequests,
                PaymentStatus = "Pending",
                TotalPrice = 0
            };

            await _bookingRepository.AddAsync(booking);

            await transaction.CommitAsync();
        }


        public async Task DeleteAsync(int id)
        {
            var booking = await _bookingRepository.GetBookingEntityByIdAsync(id);
            if (booking != null)
                await _bookingRepository.DeleteAsync(booking);
        }

        public async Task UpdateAsync(UpdateBookingCommand cmd)
        {
            var booking = await _bookingRepository.GetBookingEntityByIdAsync(cmd.Id);
            if (booking == null) return;

            booking.CheckIn = cmd.CheckIn;
            booking.CheckOut = cmd.CheckOut;
            booking.SpecialRequests = cmd.SpecialRequests;
            booking.Adults = cmd.Adults;
            booking.Children = cmd.Children;

            await _bookingRepository.UpdateAsync(booking);
        }
    }
}

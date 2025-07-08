using TravelEase.TravelEase.Application.Interfaces;

namespace TravelEase.TravelEase.Application.Features.Booking
{
    public class BookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<List<Domain.Entities.Booking>> GetAllAsync()
        {
            return await _bookingRepository.GetAllAsync();
        }

        public async Task<Domain.Entities.Booking?> GetByIdAsync(int id)
        {
            return await _bookingRepository.GetByIdAsync(id);
        }

        public async Task CreateAsync(CreateBookingCommand cmd)
        {
            var isAvailable = await _bookingRepository.IsRoomAvailableAsync(cmd.RoomId, cmd.CheckIn, cmd.CheckOut);
            if (!isAvailable)
                throw new Exception("❌ Room is not available for the selected dates");

            var booking = new Domain.Entities.Booking
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
    }
}
using Microsoft.EntityFrameworkCore;
using TravelEase.TravelEase.Infrastructure.Repositories;
using TravelEase.TravelEase.Application.Features.Room;
using TravelEase.TravelEase.Infrastructure.Data;

namespace TravelEase.TravelEase.Application.Features.Hotel
{
    public class HotelService
    {
        private readonly HotelRepository _hotelRepository;
        private readonly TravelEaseDbContext _db;

        public HotelService(HotelRepository hotelRepository, TravelEaseDbContext db)
        {
            _hotelRepository = hotelRepository;
            _db = db;
        }

        public async Task<List<TravelEase.Domain.Entities.Hotel>> GetAllHotelsAsync()
        {
            return await _hotelRepository.GetAllAsync();
        }

        public async Task<TravelEase.Domain.Entities.Hotel?> GetHotelByIdAsync(int id)
        {
            return await _hotelRepository.GetByIdAsync(id);
        }

        public async Task CreateHotelAsync(CreateHotelCommand cmd)
        {
            try
            {
                var hotel = new TravelEase.Domain.Entities.Hotel
                {
                    Name = cmd.Name,
                    CityId = cmd.CityId,
                    Owner = cmd.Owner,
                    Location = cmd.Location,
                    StarRating = cmd.StarRating,
                    Description = cmd.Description
                };

                await _hotelRepository.AddAsync(hotel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ CreateHotelAsync Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        public async Task UpdateHotelAsync(UpdateHotelCommand cmd)
        {
            var hotel = await _hotelRepository.GetByIdAsync(cmd.Id);
            if (hotel == null) return;

            hotel.Name = cmd.Name;
            hotel.CityId = cmd.CityId;
            hotel.Owner = cmd.Owner;
            hotel.Location = cmd.Location;
            hotel.StarRating = cmd.StarRating;
            hotel.Description = cmd.Description;

            await _hotelRepository.UpdateAsync(hotel);
        }

        public async Task DeleteHotelAsync(int id)
        {
            var hotel = await _hotelRepository.GetByIdAsync(id);
            if (hotel != null)
                await _hotelRepository.DeleteAsync(hotel);
        }

        public async Task<IEnumerable<Domain.Entities.Hotel>> SearchHotelsAsync(SearchHotelsQuery query)
        {
            var hotels = await _hotelRepository.GetAllAsync(); // Includes City
            var filtered = hotels.AsQueryable();

            // Textual filters
            if (!string.IsNullOrWhiteSpace(query.Name))
                filtered = filtered.Where(h => h.Name.Contains(query.Name, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(query.CityName))
                filtered = filtered.Where(h => h.City != null && h.City.Name.Contains(query.CityName, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(query.Location))
                filtered = filtered.Where(h => h.Location.Contains(query.Location, StringComparison.OrdinalIgnoreCase));

            if (query.StarRating.HasValue)
                filtered = filtered.Where(h => h.StarRating == query.StarRating.Value);

            // Room availability, capacity, and booking conflict filtering
            if (query.CheckIn.HasValue && query.CheckOut.HasValue)
            {
                var checkIn = query.CheckIn.Value;
                var checkOut = query.CheckOut.Value;
                int requiredRooms = query.NumRooms ?? 1;
                int requiredAdults = query.Adults ?? 2;
                int requiredChildren = query.Children ?? 0;

                filtered = filtered.Where(hotel =>
                    _hotelRepository.GetAvailableRoomsForHotel(
                        hotel.Id, checkIn, checkOut, requiredAdults, requiredChildren
                    ).Count() >= requiredRooms
                );
            }

            return filtered.ToList();
        }

    }
}

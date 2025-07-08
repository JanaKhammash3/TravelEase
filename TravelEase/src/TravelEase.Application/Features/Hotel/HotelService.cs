using Microsoft.EntityFrameworkCore;
using TravelEase.Application.Interfaces;
using TravelEase.TravelEase.Application.Features.Hotel;
using TravelEase.TravelEase.Application.Interfaces;
using TravelEase.TravelEase.Infrastructure.Data;

namespace TravelEase.Application.Features.Hotel
{
    public class HotelService : IHotelService
    {
        private readonly IHotelRepository _hotelRepository;
        private readonly TravelEaseDbContext _db;

        public HotelService(IHotelRepository hotelRepository, TravelEaseDbContext db)
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

        public async Task<IEnumerable<TravelEase.Domain.Entities.Hotel>> SearchHotelsAsync(SearchHotelsQuery query)
        {
            var hotels = await _hotelRepository.GetAllAsync();
            var filtered = hotels.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Name))
                filtered = filtered.Where(h => h.Name.Contains(query.Name, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(query.CityName))
                filtered = filtered.Where(h => h.City != null && h.City.Name.Contains(query.CityName, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(query.Location))
                filtered = filtered.Where(h => h.Location.Contains(query.Location, StringComparison.OrdinalIgnoreCase));

            if (query.StarRating.HasValue)
                filtered = filtered.Where(h => h.StarRating == query.StarRating.Value);

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

        public async Task<List<TravelEase.Domain.Entities.Hotel>> GetFeaturedHotelsAsync()
        {
            return await _hotelRepository.GetFeaturedHotelsAsync();
        }
    }
}

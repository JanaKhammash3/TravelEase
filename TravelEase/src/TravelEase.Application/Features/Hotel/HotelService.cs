using TravelEase.TravelEase.Infrastructure.Repositories;

namespace TravelEase.TravelEase.Application.Features.Hotel
{
    public class HotelService
    {
        private readonly HotelRepository _hotelRepository;

        public HotelService(HotelRepository hotelRepository)
        {
            _hotelRepository = hotelRepository;
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
                throw; // re-throw to let the API return 500 for now
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
            var hotels = await _hotelRepository.GetAllAsync(); // Already includes City
            var filtered = hotels.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Name))
                filtered = filtered.Where(h => h.Name.Contains(query.Name));

            if (!string.IsNullOrWhiteSpace(query.CityName))
                filtered = filtered.Where(h => h.City != null && h.City.Name.Contains(query.CityName));

            if (!string.IsNullOrWhiteSpace(query.Location))
                filtered = filtered.Where(h => h.Location.Contains(query.Location));

            if (query.StarRating.HasValue)
                filtered = filtered.Where(h => h.StarRating == query.StarRating.Value);

            return filtered.ToList();
        }


    }
}
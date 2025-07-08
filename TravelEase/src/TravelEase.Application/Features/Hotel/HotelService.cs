using TravelEase.TravelEase.Application.DTOs;
using TravelEase.TravelEase.Application.Interfaces;
using TravelEase.TravelEase.Domain.Entities;

namespace TravelEase.TravelEase.Application.Features.Hotel
{
    public class HotelService : IHotelService
    {
        private readonly IHotelRepository _hotelRepository;

        public HotelService(IHotelRepository hotelRepository)
        {
            _hotelRepository = hotelRepository;
        }

        public async Task<List<Domain.Entities.Hotel>> GetAllHotelsAsync()
        {
            return await _hotelRepository.GetAllAsync();
        }

        public async Task<Domain.Entities.Hotel?> GetHotelByIdAsync(int id)
        {
            return await _hotelRepository.GetByIdAsync(id);
        }

        public async Task CreateHotelAsync(CreateHotelCommand cmd)
        {
            var hotel = new Domain.Entities.Hotel
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

        public async Task<List<HotelDto>> SearchHotelsAsync(SearchHotelsQuery query)
{
    var hotels = await _hotelRepository.GetAllAsync(); // Already returns List<Hotel>
    var filtered = hotels.AsQueryable();

    if (!string.IsNullOrWhiteSpace(query.Name))
        filtered = filtered.Where(h => h.Name.Contains(query.Name, StringComparison.OrdinalIgnoreCase));

    if (!string.IsNullOrWhiteSpace(query.CityName))
        filtered = filtered.Where(h => h.City != null && h.City.Name.Contains(query.CityName, StringComparison.OrdinalIgnoreCase));

    if (!string.IsNullOrWhiteSpace(query.Location))
        filtered = filtered.Where(h => h.Location.Contains(query.Location, StringComparison.OrdinalIgnoreCase));

    if (query.StarRating.HasValue)
        filtered = filtered.Where(h => h.StarRating == query.StarRating.Value);

    if (query.MinPrice.HasValue)
        filtered = filtered.Where(h => h.Rooms.Any(r => r.PricePerNight >= query.MinPrice.Value));

    if (query.MaxPrice.HasValue)
        filtered = filtered.Where(h => h.Rooms.Any(r => r.PricePerNight <= query.MaxPrice.Value));

    if (!string.IsNullOrWhiteSpace(query.RoomCategory))
        filtered = filtered.Where(h => h.Rooms.Any(r =>
            r.Category.ToString().Equals(query.RoomCategory, StringComparison.OrdinalIgnoreCase)));

    if (!string.IsNullOrWhiteSpace(query.Amenities))
    {
        var amenityList = query.Amenities.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        foreach (var amenity in amenityList)
        {
            filtered = filtered.Where(h => h.Amenities.Contains(amenity, StringComparison.OrdinalIgnoreCase));
        }
    }

    // ✅ Handle date-based room availability AFTER filtering
    if (query.CheckIn.HasValue && query.CheckOut.HasValue)
    {
        var checkIn = query.CheckIn.Value;
        var checkOut = query.CheckOut.Value;
        int requiredRooms = query.NumRooms ?? 1;
        int requiredAdults = query.Adults ?? 2;
        int requiredChildren = query.Children ?? 0;

        // Must filter AFTER materialization since _hotelRepository.GetAvailableRoomsForHotel() is not translatable
        filtered = filtered.Where(hotel =>
            _hotelRepository
                .GetAvailableRoomsForHotel(hotel.Id, checkIn, checkOut, requiredAdults, requiredChildren)
                .Count() >= requiredRooms
        );
    }

    // ✅ Apply pagination and projection
    var result = filtered
        .Skip((query.Page - 1) * query.PageSize)
        .Take(query.PageSize)
        .Select(h => new HotelDto
        {
            Id = h.Id,
            Name = h.Name,
            City = h.City!.Name ?? "",
            StarRating = h.StarRating,
            Price = h.Rooms.Any() ? h.Rooms.Min(r => r.PricePerNight) : 0,
            ThumbnailUrl = h.ThumbnailUrl
        })
        .ToList();

    return result;
}

        public async Task<List<Domain.Entities.Hotel>> GetFeaturedHotelsAsync()
        {
            return await _hotelRepository.GetFeaturedHotelsAsync();
        }

        public async Task RecordHotelViewAsync(int userId, int hotelId)
        {
            await _hotelRepository.RecordHotelViewAsync(userId, hotelId);
        }

        public async Task<List<HotelDto>> GetRecentlyVisitedHotelsAsync(int userId, int count = 5)
        {
            return await _hotelRepository.GetRecentlyVisitedHotelsAsync(userId, count);
        }

        public async Task<List<TrendingCityDto>> GetTrendingCitiesAsync(int count = 5)
        {
            return await _hotelRepository.GetTrendingCitiesAsync(count);
        }
        
        public async Task<List<string>> UploadImagesAsync(int hotelId, List<(string FileName, Stream Content)> files)
        {
            var hotel = await _hotelRepository.GetByIdAsync(hotelId);
            if (hotel == null)
                throw new Exception("Hotel not found");

            var imageUrls = new List<string>();
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "hotel-images");

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            foreach (var file in files)
            {
                var newFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var fullPath = Path.Combine(uploadPath, newFileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.Content.CopyToAsync(stream);
                }

                var imageUrl = $"/hotel-images/{newFileName}";
                hotel.ImageUrls.Add(imageUrl);
                imageUrls.Add(imageUrl);
            }

            await _hotelRepository.UpdateAsync(hotel);
            return imageUrls;
        }


    }
}

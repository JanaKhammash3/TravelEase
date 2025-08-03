using TravelEase.TravelEase.Application.DTOs;
using TravelEase.TravelEase.Application.Features.Hotel;

namespace TravelEase.TravelEase.Application.Interfaces;

public interface IHotelService
{
    Task<List<HotelDto>> GetAllHotelsAsync(int page = 1, int pageSize = 20);
    Task<HotelDto?> GetHotelDtoByIdAsync(int id);
    Task CreateHotelAsync(CreateHotelCommand cmd);
    Task UpdateHotelAsync(UpdateHotelCommand cmd);
    Task DeleteHotelAsync(int id);
    Task<List<HotelDto>> SearchHotelsAsync(SearchHotelsQuery query);
    Task<List<HotelDto>> GetFeaturedHotelsAsync();
    Task<List<HotelDto>> GetRecentlyVisitedHotelsAsync(int userId, int count = 5);
    Task<List<TrendingCityDto>> GetTrendingCitiesAsync(int count = 5);
    Task RecordHotelViewAsync(int userId, int hotelId);
    Task<List<string>> UploadImagesAsync(int hotelId, List<(string FileName, Stream Content)> files);
    Task SaveHotelImageUrlsAsync(int hotelId, List<string> urls);
}

using TravelEase.TravelEase.Application.DTOs;
using TravelEase.TravelEase.Application.Features.Hotel;
using TravelEase.TravelEase.Domain.Entities;

namespace TravelEase.TravelEase.Application.Interfaces
{
    public interface IHotelService
    {
        Task<List<HotelDto>> SearchHotelsAsync(SearchHotelsQuery query);
        Task<List<Hotel>> GetAllHotelsAsync();
        Task<Hotel?> GetHotelByIdAsync(int id);
        Task CreateHotelAsync(CreateHotelCommand cmd);
        Task UpdateHotelAsync(UpdateHotelCommand cmd);
        Task DeleteHotelAsync(int id);
        Task<List<Hotel>> GetFeaturedHotelsAsync();
        Task<List<string>> UploadImagesAsync(int hotelId, List<(string FileName, Stream Content)> files);
        Task SaveHotelImageUrlsAsync(int hotelId, List<string> urls);

    }
}
using TravelEase.TravelEase.Application.Features.Hotel;
using TravelEase.TravelEase.Domain.Entities;

namespace TravelEase.TravelEase.Application.Interfaces
{
    public interface IHotelService
    {
        Task<List<Hotel>> GetAllHotelsAsync();
        Task<Hotel?> GetHotelByIdAsync(int id);
        Task CreateHotelAsync(CreateHotelCommand cmd);
        Task UpdateHotelAsync(UpdateHotelCommand cmd);
        Task DeleteHotelAsync(int id);
        Task<IEnumerable<Hotel>> SearchHotelsAsync(SearchHotelsQuery query);
        Task<List<Hotel>> GetFeaturedHotelsAsync();
    }
}
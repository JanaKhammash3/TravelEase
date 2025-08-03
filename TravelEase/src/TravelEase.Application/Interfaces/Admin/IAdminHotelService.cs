using TravelEase.TravelEase.Application.DTOs.Admin;

namespace TravelEase.TravelEase.Application.Interfaces.Admin;

public interface IAdminHotelService
{
    Task<List<AdminHotelDto>> GetAllAsync();
    Task<AdminHotelDto> GetByIdAsync(int id);
    Task<AdminHotelDto> CreateAsync(AdminHotelDto dto);
    Task<AdminHotelDto> UpdateAsync(int id, AdminHotelDto dto);
    Task<bool> DeleteAsync(int id);
    Task<List<AdminHotelDto>> GetAllAsync(string? name = null, string? city = null, int? minStars = null);

}
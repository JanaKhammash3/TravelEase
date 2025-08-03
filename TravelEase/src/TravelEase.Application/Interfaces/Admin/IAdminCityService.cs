using TravelEase.TravelEase.Application.DTOs.Admin;

namespace TravelEase.TravelEase.Application.Interfaces.Admin;

public interface IAdminCityService {
    Task<List<AdminCityDto>> GetAllCitiesAsync();
    Task<AdminCityDto> GetByIdAsync(int id);
    Task<AdminCityDto> CreateAsync(AdminCityDto dto);
    Task<AdminCityDto> UpdateAsync(int id, AdminCityDto dto);
    Task<bool> DeleteAsync(int id);
    Task<List<AdminCityDto>> GetAllAsync(string? name = null, string? country = null, string? postOffice = null, int? minHotels = null);

}
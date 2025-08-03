using TravelEase.TravelEase.Application.DTOs.Admin;

namespace TravelEase.TravelEase.Application.Interfaces.Admin;

public interface IAdminRoomService
{
    Task<List<AdminRoomDto>> GetAllAsync(string? hotelName = null, bool? isAvailable = null, int? minAdults = null, int? minChildren = null);
    Task<AdminRoomDto> GetByIdAsync(int id);
    Task<AdminRoomDto> CreateAsync(AdminRoomDto dto);
    Task<AdminRoomDto> UpdateAsync(int id, AdminRoomDto dto);
    Task<bool> DeleteAsync(int id);
}
using TravelEase.TravelEase.Application.Features.Room;
using TravelEase.TravelEase.Domain.Entities;

namespace TravelEase.TravelEase.Application.Interfaces;
public interface IRoomRepository
{
    Task<List<Room>> GetAllRoomsAsync();
    Task<Room?> GetRoomByIdAsync(int id);
    Task AddRoomAsync(Room room);
    Task UpdateRoomAsync(Room room);
    Task DeleteRoomAsync(Room room);
    Task<IEnumerable<Room>> SearchRoomsAsync(SearchRoomsQuery query);
}
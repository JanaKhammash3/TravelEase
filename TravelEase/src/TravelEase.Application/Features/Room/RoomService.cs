using TravelEase.Application.Interfaces;
using TravelEase.Domain.Enums;
using TravelEase.TravelEase.Application.Interfaces;

namespace TravelEase.Application.Features.Room
{
    public class RoomService
    {
        private readonly IRoomRepository _roomRepository;

        public RoomService(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public async Task<List<Domain.Entities.Room>> GetAllRoomsAsync()
        {
            return await _roomRepository.GetAllRoomsAsync();
        }

        public async Task<Domain.Entities.Room?> GetRoomByIdAsync(int id)
        {
            return await _roomRepository.GetRoomByIdAsync(id);
        }

        public async Task CreateRoomAsync(CreateRoomCommand cmd)
        {
            var room = new Domain.Entities.Room
            {
                Number = cmd.Number,
                CapacityAdults = cmd.CapacityAdults,
                CapacityChildren = cmd.CapacityChildren,
                PricePerNight = cmd.PricePerNight,
                Category = cmd.Category,
                HotelId = cmd.HotelId
            };

            await _roomRepository.AddRoomAsync(room);
        }

        public async Task DeleteRoomAsync(int id)
        {
            var room = await _roomRepository.GetRoomByIdAsync(id);
            if (room != null)
                await _roomRepository.DeleteRoomAsync(room);
        }

        public async Task UpdateRoomAsync(UpdateRoomCommand cmd)
        {
            var room = await _roomRepository.GetRoomByIdAsync(cmd.Id);
            if (room == null) throw new Exception("Room not found");

            room.Number = cmd.Number;
            room.CapacityAdults = cmd.CapacityAdults;
            room.CapacityChildren = cmd.CapacityChildren;
            room.PricePerNight = cmd.PricePerNight;
            room.Category = (RoomCategory)cmd.Category;
            room.HotelId = cmd.HotelId;

            await _roomRepository.UpdateRoomAsync(room);
        }

        public async Task<IEnumerable<Domain.Entities.Room>> SearchRoomsAsync(SearchRoomsQuery query)
        {
            return await _roomRepository.SearchRoomsAsync(query);
        }
    }
}

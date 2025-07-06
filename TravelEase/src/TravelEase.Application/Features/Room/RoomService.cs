using TravelEase.TravelEase.Domain.Entities;
using TravelEase.TravelEase.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using TravelEase.TravelEase.Domain.Enums;

namespace TravelEase.TravelEase.Application.Features.Room
{
    public class RoomService
    {
        private readonly TravelEaseDbContext _db;

        public RoomService(TravelEaseDbContext db)
        {
            _db = db;
        }

        public async Task<List<Domain.Entities.Room>> GetAllRoomsAsync()
        {
            return await _db.Rooms.ToListAsync();
        }

        public async Task<Domain.Entities.Room?> GetRoomByIdAsync(int id)
        {
            return await _db.Rooms.FindAsync(id);
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

            _db.Rooms.Add(room);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateRoomAsync(int id, CreateRoomCommand cmd)
        {
            var room = await _db.Rooms.FindAsync(id);
            if (room == null) return;

            room.Number = cmd.Number;
            room.CapacityAdults = cmd.CapacityAdults;
            room.CapacityChildren = cmd.CapacityChildren;
            room.PricePerNight = cmd.PricePerNight;
            room.Category = cmd.Category;
            room.HotelId = cmd.HotelId;

            await _db.SaveChangesAsync();
        }

        public async Task DeleteRoomAsync(int id)
        {
            var room = await _db.Rooms.FindAsync(id);
            if (room != null)
            {
                _db.Rooms.Remove(room);
                await _db.SaveChangesAsync();
            }
        }
        
        public async Task UpdateRoomAsync(UpdateRoomCommand cmd)
        {
            var room = await _db.Rooms.FindAsync(cmd.Id);
            if (room == null) throw new Exception("Room not found");

            room.Number = cmd.Number;
            room.CapacityAdults = cmd.CapacityAdults;
            room.CapacityChildren = cmd.CapacityChildren;
            room.PricePerNight = cmd.PricePerNight;
            room.Category = (RoomCategory)cmd.Category;
            room.HotelId = cmd.HotelId;

            _db.Rooms.Update(room);
            await _db.SaveChangesAsync();
        }
        
        public async Task<IEnumerable<Domain.Entities.Room>> SearchRoomsAsync(SearchRoomsQuery query)
        {
            var rooms = _db.Rooms.AsQueryable();

            if (query.HotelId.HasValue)
                rooms = rooms.Where(r => r.HotelId == query.HotelId.Value);

            if (query.CapacityAdults.HasValue)
                rooms = rooms.Where(r => r.CapacityAdults >= query.CapacityAdults.Value);

            if (query.CapacityChildren.HasValue)
                rooms = rooms.Where(r => r.CapacityChildren >= query.CapacityChildren.Value);

            if (query.MinPrice.HasValue)
                rooms = rooms.Where(r => r.PricePerNight >= query.MinPrice.Value);

            if (query.MaxPrice.HasValue)
                rooms = rooms.Where(r => r.PricePerNight <= query.MaxPrice.Value);

            if (query.Category.HasValue)
                rooms = rooms.Where(r => (int)r.Category == query.Category.Value);

            return await rooms.ToListAsync();
        }


    }
}
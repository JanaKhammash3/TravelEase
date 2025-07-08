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

            if (query.MinPrice.HasValue)
                rooms = rooms.Where(r => r.PricePerNight >= query.MinPrice.Value);

            if (query.MaxPrice.HasValue)
                rooms = rooms.Where(r => r.PricePerNight <= query.MaxPrice.Value);

            if (query.Category.HasValue)
                rooms = rooms.Where(r => (int)r.Category == query.Category.Value);

            // Total guest filtering
            if (query.Adults.HasValue)
                rooms = rooms.Where(r => r.CapacityAdults >= query.Adults.Value);

            if (query.Children.HasValue)
                rooms = rooms.Where(r => r.CapacityChildren >= query.Children.Value);

            // Optional: Check availability for selected date range — requires Booking table logic
            if (query.CheckIn.HasValue && query.CheckOut.HasValue)
            {
                var checkIn = query.CheckIn.Value;
                var checkOut = query.CheckOut.Value;

                rooms = rooms.Where(r =>
                    !_db.Bookings.Any(b =>
                        b.RoomId == r.Id &&
                        (
                            (checkIn >= b.CheckIn && checkIn < b.CheckOut) ||
                            (checkOut > b.CheckIn && checkOut <= b.CheckOut) ||
                            (checkIn <= b.CheckIn && checkOut >= b.CheckOut)
                        )
                    )
                );
            }

            return await rooms.ToListAsync();
        }



    }
}
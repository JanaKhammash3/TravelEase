using TravelEase.TravelEase.Domain.Enums;

namespace TravelEase.TravelEase.Application.Features.Room
{
    public class CreateRoomCommand
    {
        public string Number { get; set; }
        public int CapacityAdults { get; set; }
        public int CapacityChildren { get; set; }
        public decimal PricePerNight { get; set; }
        public RoomCategory Category { get; set; }
        public int HotelId { get; set; }
    }
}
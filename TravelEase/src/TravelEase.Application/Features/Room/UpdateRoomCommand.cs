namespace TravelEase.Application.Features.Room
{
    public class UpdateRoomCommand
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public int CapacityAdults { get; set; }
        public int CapacityChildren { get; set; }
        public decimal PricePerNight { get; set; }
        public int Category { get; set; }
        public int HotelId { get; set; }
    }
}
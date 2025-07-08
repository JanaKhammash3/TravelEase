namespace TravelEase.TravelEase.Application.Features.Room
{
    public class SearchRoomsQuery
    {
        public int? HotelId { get; set; }
        public int? CapacityAdults { get; set; }
        public int? CapacityChildren { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? Category { get; set; }
        
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public int? NumRooms { get; set; }
        public int? Adults { get; set; }
        public int? Children { get; set; }
    }
}
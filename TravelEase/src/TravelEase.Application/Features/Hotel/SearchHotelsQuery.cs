namespace TravelEase.TravelEase.Application.Features.Hotel
{
    public class SearchHotelsQuery
    {
        public string? Name { get; set; }
        public string? CityName { get; set; }
        public int? StarRating { get; set; }
        public string? Location { get; set; }
        
        public DateTime? CheckIn { get; set; } = DateTime.Today;
        public DateTime? CheckOut { get; set; } = DateTime.Today.AddDays(1);
        public int? Adults { get; set; } = 2;
        public int? Children { get; set; } = 0;
        public int? NumRooms { get; set; } = 1;
    }
}
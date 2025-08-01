namespace TravelEase.TravelEase.Application.Features.Hotel
{
    public class SearchHotelsQuery
    {
        public string? Name { get; set; }
        public string? CityName { get; set; }
        public string? Location { get; set; }
        public int? StarRating { get; set; }

        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public int? NumRooms { get; set; }
        public int? Adults { get; set; }
        public int? Children { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Amenities { get; set; } 
        public string? RoomCategory { get; set; } // Ex: "Luxury", "Budget"

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
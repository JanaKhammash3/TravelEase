namespace TravelEase.TravelEase.Application.Features.Hotel
{
    public class SearchHotelsQuery
    {
        public string? Name { get; set; }
        public string? CityName { get; set; }
        public int? StarRating { get; set; }
        public string? Location { get; set; }
    }
}
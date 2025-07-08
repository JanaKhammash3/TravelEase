namespace TravelEase.TravelEase.Application.DTOs
{
    public class TrendingCityDto
    {
        public string City { get; set; } = null!;
        public int VisitCount { get; set; }
        public string ThumbnailUrl { get; set; } = null!;
    }
}
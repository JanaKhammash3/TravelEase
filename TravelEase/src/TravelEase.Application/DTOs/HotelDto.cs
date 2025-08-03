namespace TravelEase.Application.DTOs
{
    public class HotelDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string City { get; set; } = null!;
        public int StarRating { get; set; }
        public decimal Price { get; set; }
        public string ThumbnailUrl { get; set; } = null!;
        public double Latitude { get; set; }
        public double Longitude { get; set; }

    }
}
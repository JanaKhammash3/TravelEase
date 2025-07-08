namespace TravelEase.TravelEase.Domain.Entities
{
    public class Hotel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CityId { get; set; }
        public string Owner { get; set; }
        public string Location { get; set; }
        public int StarRating { get; set; }
        public string? Description { get; set; }

        // ✅ Required for Featured Deals
        public string ThumbnailUrl { get; set; } = string.Empty;
        public decimal OriginalPrice { get; set; }
        public decimal DiscountedPrice { get; set; }
        public bool IsFeatured { get; set; } = false;

        public City? City { get; set; }
        public List<Room> Rooms { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();

        // Optional: Computed average from Reviews
        public double AverageRating => Reviews.Any() ? Reviews.Average(r => r.Rating) : 0.0;
    }
}
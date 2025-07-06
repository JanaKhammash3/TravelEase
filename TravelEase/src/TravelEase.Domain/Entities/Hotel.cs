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

        public City? City { get; set; }
        public List<Room> Rooms { get; set; } = new();
        public List<Review> Reviews { get; set; } = new(); // ✅ Add this
    }
}
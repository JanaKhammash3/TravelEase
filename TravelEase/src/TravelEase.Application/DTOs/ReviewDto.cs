namespace TravelEase.Application.DTOs
{
    public class ReviewDto
    {
        public int HotelId { get; set; }
        public int UserId { get; set; }
        public int Rating { get; set; } // 1 to 5
        public string Comment { get; set; } = string.Empty;
    }
}
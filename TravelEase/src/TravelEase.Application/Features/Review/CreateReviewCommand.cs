namespace TravelEase.TravelEase.Application.Features.Review
{
    public class CreateReviewCommand
    {
        public int HotelId { get; set; }
        public int UserId { get; set; }
        public int Rating { get; set; } // e.g., 1-5 stars
        public string Comment { get; set; } = string.Empty;
    }
}
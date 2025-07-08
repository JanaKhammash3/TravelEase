namespace TravelEase.TravelEase.Application.DTOs
{
    public class CartItemDto
    {
        public int UserId { get; set; }
        public int RoomId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
    }
}
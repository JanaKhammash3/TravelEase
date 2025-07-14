namespace TravelEase.TravelEase.Application.DTOs
{
    public class BookingConfirmationDto
    {
        public int BookingId { get; set; }
        public string ConfirmationNumber { get; set; }
        public string HotelName { get; set; }
        public string HotelAddress { get; set; }
        public string RoomDetails { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentStatus { get; set; }

        // ✅ Add this property for email delivery
        public string UserEmail { get; set; }
    }
}